﻿using BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TVShows.API.IntegrationEvents.Events;
using TVShows.API.Model;

namespace TVShows.API.IntegrationEvents.EventHandling
{
    public class ShowUpdatedIntegrationEventHandler : IIntegrationEventHandler<ShowUpdatedIntegrationEvent>
    {
        private readonly IGenericRepository<Show> _repository;

        public ShowUpdatedIntegrationEventHandler(IGenericRepository<Show> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(ShowUpdatedIntegrationEvent @event)
        {
            IEnumerable<Show> shows = _repository.Get();

            Show show = await _repository.GetByID(@event.ShowId);
            UpdateShow(@event, show);
            this._repository.Update(show);
        }

        private Show UpdateShow(ShowUpdatedIntegrationEvent @event, Show show)
        {
            show.Name = @event.Name;
            show.Timestamp = @event.Timestamp;
            return show;
        }

    }
}
