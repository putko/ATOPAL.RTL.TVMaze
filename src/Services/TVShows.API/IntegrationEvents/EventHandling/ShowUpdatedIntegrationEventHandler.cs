namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.EventHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.Events;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;

    public class ShowUpdatedIntegrationEventHandler : IIntegrationEventHandler<ShowUpdatedIntegrationEvent>
    {
        private readonly IGenericRepository<Show> _repository;

        public ShowUpdatedIntegrationEventHandler(IGenericRepository<Show> repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(paramName: nameof(repository));
        }

        public async Task Handle(ShowUpdatedIntegrationEvent @event)
        {
            // start handling the updated show by checking if it is already in database
            var show = await this._repository.GetById(id: @event.Payload.Id);
            if (show == null)
            {
                // Create a new show with the given information.
                show = new Show();
                this.MapShow(@event: @event, show: show);
                this._repository.Insert(entity: show);
            }
            else if (@event.Payload.Updated != show.Timestamp)
            {
                // check if the timestamp is different because our redis cache may be restarted. 
                this.MapShow(@event: @event, show: show);
                this._repository.Update(entity: show);
            }
        }

        private void MapShow(ShowUpdatedIntegrationEvent @event, Show show)
        {
            show.TVMazeId = @event.Payload.Id;
            show.Name = @event.Payload.Name;
            show.Timestamp = @event.Payload.Updated;
            // checked if casts are gathered.
            if (@event.Payload.Embedded?.Cast != null)
            {
                // if the version in the db does not contains any cast info, just create...
                if (show.Persons == null)
                {
                    show.Persons = new List<ShowPerson>();
                }
                // if it contains, check they are not excluded in the new version
                else
                {
                    // remove if the corresponding person is not in the new cast.
                    for (var i = show.Persons.Count - 1; i >= 0; i--)
                    {
                        var showToCheck = show.Persons[index: i];
                        var found = @event.Payload.Embedded.Cast.Any(predicate: x =>
                            x.Person.Id == showToCheck.Person.TVMazeId);
                        if (!found)
                        {
                            show.Persons.RemoveAt(index: i);
                        }
                    }
                }

                foreach (var cast in @event.Payload.Embedded.Cast)
                {
                    // Find the existing persons
                    var foundShowPerson =
                        show.Persons.FirstOrDefault(predicate: x => x.Person.TVMazeId == cast.Person.Id);
                    if (foundShowPerson == null)
                    {
                        // Create if the person was not included in the old version. 
                        var person = new Person
                        {
                            Name = cast.Person.Name,
                            TVMazeId = cast.Person.Id,
                            BirthDate = cast.Person.Birthday
                        };

                        // build the relation.
                        show.Persons.Add(item: new ShowPerson
                        {
                            Person = person,
                            Show = show
                        });
                    }
                    else
                    {
                        // update if a person is found. 
                        foundShowPerson.Person.Name = cast.Person.Name;
                        foundShowPerson.Person.TVMazeId = cast.Person.Id;
                        foundShowPerson.Person.BirthDate = cast.Person.Birthday;
                    }
                }
            }
        }
    }
}