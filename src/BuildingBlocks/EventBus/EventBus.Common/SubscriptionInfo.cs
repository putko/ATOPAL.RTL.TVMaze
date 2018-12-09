namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common
{
    using System;

    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            private SubscriptionInfo(bool isDynamic, Type handlerType)
            {
                this.IsDynamic = isDynamic;
                this.HandlerType = handlerType;
            }

            public bool IsDynamic { get; }
            public Type HandlerType { get; }

            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(isDynamic: true, handlerType: handlerType);
            }

            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(isDynamic: false, handlerType: handlerType);
            }
        }
    }
}