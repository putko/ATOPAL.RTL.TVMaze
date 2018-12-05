using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.Events;
using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.EventHandling
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
            Show show = await _repository.GetByID(@event.Payload.Id);
            if (show == null)
            {
                show = new Show();
                MapShow(@event, show);
                _repository.Insert(show);
            }
            else if (@event.Payload.Updated != show.Timestamp)
            {
                MapShow(@event, show);
                _repository.Update(show);
            }
        }

        private void MapShow(ShowUpdatedIntegrationEvent @event, Show show)
        {
            show.TVMazeId = @event.Payload.Id;
            show.Name = @event.Payload.Name;
            show.Timestamp = @event.Payload.Updated;
            if (@event.Payload.Embedded != null &&  @event.Payload.Embedded.Cast !=null)
            {
                if (show.Persons == null)
                {
                    show.Persons = new List<ShowPerson>();
                }
                else
                {
                    for (int i = show.Persons.Count - 1; i >= 0; i--)
                    {
                        ShowPerson showToCheck = show.Persons[i];
                        bool found = @event.Payload.Embedded.Cast.Any(x => x.Person.Id == showToCheck.Person.TVMazeId);
                        if (!found)
                        {
                            show.Persons.RemoveAt(i);
                        }
                    }
                }

                foreach (AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain.Cast cast in @event.Payload.Embedded.Cast)
                {
                    // Update the existing persons
                    ShowPerson foundShowPerson = show.Persons.FirstOrDefault(x => x.Person.TVMazeId == cast.Person.Id);
                    if (foundShowPerson == null)
                    {
                        Person person = new Person
                        {
                            Name = cast.Person.Name,
                            TVMazeId = cast.Person.Id,
                            BirthDate = cast.Person.Birthday,
                        };

                        show.Persons.Add(new ShowPerson
                        {
                            Person = person,
                            Show = show,
                        });
                    }
                    else
                    {
                        foundShowPerson.Person.Name = cast.Person.Name;
                        foundShowPerson.Person.TVMazeId = cast.Person.Id;
                        foundShowPerson.Person.BirthDate = cast.Person.Birthday;
                    }
                }
            }
        }
    }
}
