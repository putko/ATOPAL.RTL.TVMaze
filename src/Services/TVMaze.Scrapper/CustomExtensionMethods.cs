namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper
{
    using Autofac;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    using StackExchange.Redis;

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton(implementationFactory: sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ScrapperSettings>>().Value;
                var configurationOptions =
                    ConfigurationOptions.Parse(configuration: settings.ConnectionString, ignoreUnknown: true);

                configurationOptions.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration: configurationOptions);
            });
            return services;
        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqPersistentConnection>(implementationFactory: sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

                var factory = new ConnectionFactory
                {
                    HostName = configuration[key: "EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusUserName"]))
                {
                    factory.UserName = configuration[key: "EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusPassword"]))
                {
                    factory.Password = configuration[key: "EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: configuration[key: "EventBusRetryCount"]);
                }

                return new DefaultRabbitMqPersistentConnection(connectionFactory: factory, logger: logger,
                    retryCount: retryCount);
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration[key: "SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMq>(implementationFactory: sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: configuration[key: "EventBusRetryCount"]);
                }

                return new EventBusRabbitMq(persistentConnection: rabbitMqPersistentConnection, logger: logger,
                    container: iLifetimeScope,
                    subsManager: eventBusSubscriptionsManager, queueName: subscriptionClientName,
                    retryCount: retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}