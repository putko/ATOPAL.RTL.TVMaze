namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using global::RabbitMQ.Client.Exceptions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Polly;

    public class EventBusRabbitMq : IEventBus, IDisposable
    {
        private const string BrokerName = "rtl_tvshows_event_bus";
        private readonly ILifetimeScope _container;
        private readonly ILogger<EventBusRabbitMq> _logger;

        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly int _retryCount;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly string CONTAINER_SCOPE_NAME = "rtl_tvshows_event_bus";

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger,
            ILifetimeScope container, IEventBusSubscriptionsManager subsManager, string queueName = null,
            int retryCount = 5)
        {
            this._persistentConnection =
                persistentConnection ?? throw new ArgumentNullException(paramName: nameof(persistentConnection));
            this._logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
            this._subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            this._queueName = queueName;
            this._consumerChannel = this.CreateConsumerChannel();
            this._container = container;
            this._retryCount = retryCount;
            this._subsManager.OnEventRemoved += this.SubsManager_OnEventRemoved;
        }

        public void Dispose()
        {
            this._consumerChannel?.Dispose();

            this._subsManager.Clear();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!this._persistentConnection.IsConnected)
            {
                this._persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount: this._retryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(value: Math.Pow(x: 2, y: retryAttempt)),
                    onRetry: (ex, time) => { this._logger.LogWarning(message: ex.ToString()); });

            using (var channel = this._persistentConnection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: EventBusRabbitMq.BrokerName,
                    type: "direct");

                var message = JsonConvert.SerializeObject(value: @event);
                var body = Encoding.UTF8.GetBytes(s: message);

                policy.Execute(action: () =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: EventBusRabbitMq.BrokerName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            this.DoInternalSubscription(eventName: eventName);
            this._subsManager.AddDynamicSubscription<TH>(eventName: eventName);
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = this._subsManager.GetEventKey<T>();
            this.DoInternalSubscription(eventName: eventName);
            this._subsManager.AddSubscription<T, TH>();
        }

        public void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            this._subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            this._subsManager.RemoveDynamicSubscription<TH>(eventName: eventName);
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!this._persistentConnection.IsConnected)
            {
                this._persistentConnection.TryConnect();
            }

            using (var channel = this._persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: this._queueName,
                    exchange: EventBusRabbitMq.BrokerName,
                    routingKey: eventName);

                if (this._subsManager.IsEmpty)
                {
                    this._queueName = string.Empty;
                    this._consumerChannel.Close();
                }
            }
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = this._subsManager.HasSubscriptionsForEvent(eventName: eventName);
            if (!containsKey)
            {
                if (!this._persistentConnection.IsConnected)
                {
                    this._persistentConnection.TryConnect();
                }

                using (var channel = this._persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: this._queueName,
                        exchange: EventBusRabbitMq.BrokerName,
                        routingKey: eventName);
                }
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!this._persistentConnection.IsConnected)
            {
                this._persistentConnection.TryConnect();
            }

            var channel = this._persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusRabbitMq.BrokerName,
                type: "direct");

            channel.QueueDeclare(queue: this._queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            var consumer = new EventingBasicConsumer(model: channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(bytes: ea.Body);

                await this.ProcessEvent(eventName: eventName, message: message);

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: this._queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                this._consumerChannel.Dispose();
                this._consumerChannel = this.CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (this._subsManager.HasSubscriptionsForEvent(eventName: eventName))
            {
                using (var scope = this._container.BeginLifetimeScope(tag: this.CONTAINER_SCOPE_NAME))
                {
                    var subscriptions = this._subsManager.GetHandlersForEvent(eventName: eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler =
                                scope.ResolveOptional(serviceType: subscription.HandlerType) as
                                    IDynamicIntegrationEventHandler;
                            if (handler == null)
                            {
                                continue;
                            }

                            dynamic eventData = JObject.Parse(json: message);
                            await handler.Handle(eventData: eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(serviceType: subscription.HandlerType);
                            if (handler == null)
                            {
                                continue;
                            }

                            var eventType = this._subsManager.GetEventTypeByName(eventName: eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(value: message, type: eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task) concreteType.GetMethod(name: "Handle")
                                ?.Invoke(obj: handler, parameters: new[] {integrationEvent});
                        }
                    }
                }
            }
        }
    }
}