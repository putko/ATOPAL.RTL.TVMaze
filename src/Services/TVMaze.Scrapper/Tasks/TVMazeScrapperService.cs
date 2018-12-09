namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class TvMazeScrapperService
        : BackgroundService
    {
        private readonly ILogger<TvMazeScrapperService> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly TvMazeUpdater _updater;

        public TvMazeScrapperService(IOptions<BackgroundTaskSettings> settings,
            ILogger<TvMazeScrapperService> logger,
            TvMazeUpdater updater)
        {
            this._settings = settings?.Value ?? throw new ArgumentNullException(paramName: nameof(settings));
            this._logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
            this._updater = updater ?? throw new ArgumentNullException(paramName: nameof(updater));
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogDebug(message: "TVMazeUpdateChecker task is starting.");

            stoppingToken.Register(callback: () =>
                this._logger.LogDebug(message: "#1 TVMazeUpdateChecker background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogDebug(message: "TVMazeUpdateChecker background task is doing background work.");
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                this._updater.CheckForUpdates();
                stopWatch.Stop();
                // Find the execution time roughly. (roughtly because stopwatch is effected from cpu core efficiency)
                var delay = Math.Max(val1: this._settings.UpdateCheckInterval - stopWatch.ElapsedMilliseconds,
                    val2: 0L);

                // use the execution time to determine if a delay is needed or execution is needed to repeat immediately. 
                await Task.Delay(delay: TimeSpan.FromMilliseconds(value: delay), cancellationToken: stoppingToken);
            }

            this._logger.LogDebug(message: "TVMazeUpdateChecker background task is stopping.");

            await Task.CompletedTask;
        }
    }
}