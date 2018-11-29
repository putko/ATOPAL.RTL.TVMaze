using BuildingBlocks.EventBus.Events;

namespace TVShows.API.IntegrationEvents.Events
{
    public class ShowUpdatedIntegrationEvent : IntegrationEvent
    {
        public int ShowId { get; private set; }

        public string Name { get; private set; }
        public byte[] Timestamp { get; private set; }

        public ShowUpdatedIntegrationEvent(int showId, string name, byte[] timestamp)
        {
            this.ShowId = showId;
            this.Name = name;
            this.Timestamp = timestamp;
        }
    }
}
