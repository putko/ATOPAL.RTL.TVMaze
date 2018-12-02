using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks
{
    public class TVMazeUpdateChecker
        : BackgroundService
    {
        private readonly ILogger<TVMazeUpdateChecker> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;

        public TVMazeUpdateChecker(IOptions<BackgroundTaskSettings> settings,
                                         IEventBus eventBus,
                                         ILogger<TVMazeUpdateChecker> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"TVMazeUpdateChecker task is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 TVMazeUpdateChecker background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"TVMazeUpdateChecker background task is doing background work.");

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }

            _logger.LogDebug($"TVMazeUpdateChecker background task is stopping.");

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogDebug($"Checking tvmaze updates..");
        }
    }
}
