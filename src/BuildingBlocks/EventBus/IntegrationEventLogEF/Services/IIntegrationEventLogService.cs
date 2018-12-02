using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
        Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
