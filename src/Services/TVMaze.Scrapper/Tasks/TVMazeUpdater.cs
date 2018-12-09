namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks
{
    using System;
    using System.Threading.Tasks;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.TVMaze;
    using JackLeitch.RateGate;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class TvMazeUpdater
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<TvMazeScrapperService> _logger;
        private readonly IConcurrencyRepository _repository;
        private readonly BackgroundTaskSettings _settings;

        public TvMazeUpdater(ILogger<TvMazeScrapperService> logger,
            IConcurrencyRepository repository,
            IOptions<BackgroundTaskSettings> settings,
            IEventBus eventBus)
        {
            this._logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
            this._repository = repository ?? throw new ArgumentNullException(paramName: nameof(repository));
            this._settings = settings?.Value ?? throw new ArgumentNullException(paramName: nameof(settings));
            this._eventBus = eventBus ?? throw new ArgumentNullException(paramName: nameof(eventBus));
        }

        public void CheckForUpdates()
        {
            this._logger.LogDebug(message: "Checking tv maze updates..");
            var api = new TvMazeApi(settings: this._settings);
            // in each iteration begin with getting the timestamps of all tvshows in the db. 
            // https://www.tvmaze.com/api#updates
            var result = api.GetUpdates();

            // Then consider rate limiting
            // https://www.tvmaze.com/api#rate-limiting
            // Instead of rate limiting directly the execute function of TvMazeApi class, we use rate limiting here to avoid redis timeout failure because of the Parallel foreach. 
            //  redis repository could be implemented  to work with bench operations, just didn't want to postpone the actual operation. 
            // Rate limit is currently 20 calls in 10 seconds, the RateGate component is chosen because it uses SemaphoreSlim to optimize the call rate with the execution time. 
            using (var rateGate = new RateGate(occurrences: this._settings.RateLimitOccurrences,
                timeUnit: TimeSpan.FromSeconds(value: this._settings.RateLimitSeconds)))
            {
                // Run a loop for every update in the tv maze database
                // we dont need more thread to start, then wait in the semaphore queue. So we use MaxDegreeOfParallelism property.
                Parallel.ForEach(source: result,parallelOptions: new ParallelOptions
                    { MaxDegreeOfParallelism = this._settings.RateLimitOccurrences },
                    body: currentElement =>
                    {
                        // Wait for the rate limit...
                        rateGate.WaitToProceed();
                        // check for the timestamp recently obtained matches the value in the redis cache. 
                        var isValid = this._repository.IsConcurrencyValueValid(value: currentElement);
                        if (!isValid)
                        {
                            //  if not, get the show details from api, 
                            // https://www.tvmaze.com/api#shows
                            var updated = api.GetShowDetail(showId: currentElement.Key);

                            // Create Integration Event to be published through the Event Bus
                            var showUpdatedEvent = new ShowUpdatedIntegrationEvent(payload: updated);
                            // Publish the event so the other projects may know about this change. 
                            this._eventBus.Publish(@event: showUpdatedEvent);
                        }
                    });
            }
        }
    }
}