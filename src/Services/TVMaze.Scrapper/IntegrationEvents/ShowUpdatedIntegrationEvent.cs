namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents
{
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;

    public class ShowUpdatedIntegrationEvent : IntegrationEvent
    {
        public ShowUpdatedIntegrationEvent(Show payload)
        {
            this.Payload = payload;
        }

        public Show Payload { get; set; }
    }
}