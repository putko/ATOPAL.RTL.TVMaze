using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents
{
    public interface ITVMazeScrapperIntegrationEventService
    {
        void PublishThroughEventBus(IntegrationEvent evt);
    }
}
