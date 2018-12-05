using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents
{
    public class TVMazeScrapperIntegrationEventService : ITVMazeScrapperIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;

        public TVMazeScrapperIntegrationEventService(IEventBus eventBus, Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public void PublishThroughEventBus(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);
        }
    }
}
