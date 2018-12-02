using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
