using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents
{
    public class ShowUpdatedIntegrationEvent : IntegrationEvent
    {
        public Show Payload { get; set; }

        public ShowUpdatedIntegrationEvent(Show payload)
        {
            this.Payload = payload;
        }
    }
}
