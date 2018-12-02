using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.IntegrationEventLogEF.Services;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.IntegrationEventLogEF.Utilities;
using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents
{
    public class TVShowsIntegrationEventService : ITVShowsIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly TVShowContext _context;
        private readonly IIntegrationEventLogService _eventLogService;

        public TVShowsIntegrationEventService(IEventBus eventBus, TVShowContext catalogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _context = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_context.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception)
            {
                await _eventLogService.MarkEventAsFailedAsync(evt.Id);
            }            
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_context)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _context.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _context.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}
