namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions
{
    using System.Threading.Tasks;

    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}