using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks
{
    public class TVMazeScrapperService
        : BackgroundService
    {
        private readonly ILogger<TVMazeScrapperService> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly TVMazeUpdater _updater;
        public TVMazeScrapperService(IOptions<BackgroundTaskSettings> settings,
                                    ILogger<TVMazeScrapperService> logger,
                                    TVMazeUpdater updater)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"TVMazeUpdateChecker task is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 TVMazeUpdateChecker background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"TVMazeUpdateChecker background task is doing background work.");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                await _updater.CheckForUpdates();
                stopWatch.Stop();
                long delay = Math.Max(_settings.UpdateCheckInterval - stopWatch.ElapsedMilliseconds, 0L);
                await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
            }

            _logger.LogDebug($"TVMazeUpdateChecker background task is stopping.");

            await Task.CompletedTask;
        }
    }
}
