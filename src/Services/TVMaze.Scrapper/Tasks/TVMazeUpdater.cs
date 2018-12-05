using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.IntegrationEvents;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.TVMaze;
using JackLeitch.RateGate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks
{
    public class TVMazeUpdater
    {
        private readonly ILogger<TVMazeScrapperService> _logger;
        private readonly IConcurrencyRepository _repository;
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;

        public TVMazeUpdater(ILogger<TVMazeScrapperService> logger,
                                    IConcurrencyRepository repository,
                                    IOptions<BackgroundTaskSettings> settings,
                                    IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task CheckForUpdates()
        {
            _logger.LogDebug($"Checking tvmaze updates..");
            TVMazeApi api = new TVMazeApi(_settings);
            Dictionary<string, long> result = api.GetUpdates();

            using (RateGate rateGate = new RateGate(_settings.RateLimitOccurences, TimeSpan.FromSeconds(_settings.RateLimitSeconds)))
            {
                Parallel.ForEach(result,
                 currentElement =>
                 {
                     rateGate.WaitToProceed();
                     bool isValid = _repository.IsConcurrencyValueValid(currentElement);
                     if (!isValid)
                     {
                         Show updated = api.GetShowDetail(currentElement.Key);
                         //Create Integration Event to be published through the Event Bus
                         ShowUpdatedIntegrationEvent showUpdatedEvent = new ShowUpdatedIntegrationEvent(updated);

                         _eventBus.Publish(showUpdatedEvent);
                     }
                 });
            }
        }

        private IList<string> GetInvalidIds(Dictionary<string, long> updateList)
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, long> item in updateList)
            {
                bool isValid = _repository.IsConcurrencyValueValid(item);
                if (!isValid)
                {
                    result.Add(item.Key);
                }
            }
            return result;
        }
    }
}
