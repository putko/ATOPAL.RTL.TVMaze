namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;

    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly List<Type> _eventTypes;


        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

        public InMemoryEventBusSubscriptionsManager()
        {
            this._handlers = new Dictionary<string, List<SubscriptionInfo>>();
            this._eventTypes = new List<Type>();
        }

        public event EventHandler<string> OnEventRemoved;

        public bool IsEmpty
        {
            get { return !this._handlers.Keys.Any(); }
        }

        public void Clear()
        {
            this._handlers.Clear();
        }

        public void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            this.DoAddSubscription(handlerType: typeof(TH), eventName: eventName, isDynamic: true);
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = this.GetEventKey<T>();

            this.DoAddSubscription(handlerType: typeof(TH), eventName: eventName, isDynamic: false);

            if (!this._eventTypes.Contains(item: typeof(T)))
            {
                this._eventTypes.Add(item: typeof(T));
            }
        }


        public void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = this.FindDynamicSubscriptionToRemove<TH>(eventName: eventName);
            this.DoRemoveHandler(eventName: eventName, subsToRemove: handlerToRemove);
        }


        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var handlerToRemove = this.FindSubscriptionToRemove<T, TH>();
            var eventName = this.GetEventKey<T>();
            this.DoRemoveHandler(eventName: eventName, subsToRemove: handlerToRemove);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = this.GetEventKey<T>();
            return this.GetHandlersForEvent(eventName: key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            return this._handlers[key: eventName];
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = this.GetEventKey<T>();
            return this.HasSubscriptionsForEvent(eventName: key);
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return this._handlers.ContainsKey(key: eventName);
        }

        public Type GetEventTypeByName(string eventName)
        {
            return this._eventTypes.SingleOrDefault(predicate: t => t.Name == eventName);
        }

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!this.HasSubscriptionsForEvent(eventName: eventName))
            {
                this._handlers.Add(key: eventName, value: new List<SubscriptionInfo>());
            }

            if (this._handlers[key: eventName].Any(predicate: s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    message: $"Handler Type {handlerType.Name} already registered for '{eventName}'",
                    paramName: nameof(handlerType));
            }

            if (isDynamic)
            {
                this._handlers[key: eventName].Add(item: SubscriptionInfo.Dynamic(handlerType: handlerType));
            }
            else
            {
                this._handlers[key: eventName].Add(item: SubscriptionInfo.Typed(handlerType: handlerType));
            }
        }


        private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                this._handlers[key: eventName].Remove(item: subsToRemove);
                if (!this._handlers[key: eventName].Any())
                {
                    this._handlers.Remove(key: eventName);
                    var eventType = this._eventTypes.SingleOrDefault(predicate: e => e.Name == eventName);
                    if (eventType != null)
                    {
                        this._eventTypes.Remove(item: eventType);
                    }

                    this.RaiseOnEventRemoved(eventName: eventName);
                }
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = this.OnEventRemoved;
            if (handler != null)
            {
                this.OnEventRemoved(sender: this, e: eventName);
            }
        }


        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return this.DoFindSubscriptionToRemove(eventName: eventName, handlerType: typeof(TH));
        }


        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = this.GetEventKey<T>();
            return this.DoFindSubscriptionToRemove(eventName: eventName, handlerType: typeof(TH));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!this.HasSubscriptionsForEvent(eventName: eventName))
            {
                return null;
            }

            return this._handlers[key: eventName].SingleOrDefault(predicate: s => s.HandlerType == handlerType);
        }
    }
}