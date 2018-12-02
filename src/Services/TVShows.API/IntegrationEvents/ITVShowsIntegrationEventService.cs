using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents
{
    public interface ITVShowsIntegrationEventService
    {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
