using BuildingBlocks.EventBus.Events;
using System.Threading.Tasks;

namespace TVShows.API.IntegrationEvents
{
    public interface ITVShowsIntegrationEventService
    {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
