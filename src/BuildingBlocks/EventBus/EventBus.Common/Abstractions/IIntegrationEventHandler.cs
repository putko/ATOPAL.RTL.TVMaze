namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions
{
    using System.Threading.Tasks;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;

    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}