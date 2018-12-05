using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IGenericRepository<Show> repository;

        public ShowsController(IGenericRepository<Show> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Show>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery]int pageSize = 10,
                                               [FromQuery]int pageIndex = 0)

        {
            //long totalItems = await repository.Get().CountAsync();

            List<Show> items = await repository.Get()
                .OrderBy(s => s.Name)
                .Include(p => p.Persons).ThenInclude(sp => sp.Person)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var itemsOnPage = items.Select(i =>
            new
            {
                Id = i.TVMazeId,
                i.Name,
                Cast = i.Persons.OrderByDescending(p => p.Person, PersonDateOfBirthComparer.Default).Select(p => new
                {
                    Id = p.Person.TVMazeId,
                    Name = p.Person.Name,
                    Birthday = p.Person.BirthDate,
                })
            });

            return Ok(itemsOnPage.ToList());
        }
    }

    public class PersonDateOfBirthComparer : Comparer<Person>
    {
        public static new PersonDateOfBirthComparer Default { get; } = new PersonDateOfBirthComparer();

        public override int Compare(Person x, Person y)
        {
            // handle nulls

            return Comparer<DateTime>.Default.Compare(x.BirthDate.GetValueOrDefault(), y.BirthDate.GetValueOrDefault());
        }
    }
}
