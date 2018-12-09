namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.ViewModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Route(template: "api/[controller]")]
    [ApiController]
    [Route(template: "api/v1/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IGenericRepository<Show> repository;

        public ShowsController(IGenericRepository<Show> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(paramName: nameof(repository));
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [ProducesResponseType(type: typeof(PaginatedItemsViewModel<Show>), statusCode: (int) HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0)

        {
            //long totalItems = await repository.Get().CountAsync();

            var items = await this.repository.Get()
                .OrderBy(keySelector: s => s.Name)
                .Include(navigationPropertyPath: p => p.Persons).ThenInclude(navigationPropertyPath: sp => sp.Person)
                .Skip(count: pageSize * pageIndex)
                .Take(count: pageSize)
                .ToListAsync();

            var itemsOnPage = items.Select(selector: i =>
                new
                {
                    Id = i.TVMazeId,
                    i.Name,
                    Cast = i.Persons
                        .OrderByDescending(keySelector: p => p.Person, comparer: PersonDateOfBirthComparer.Default)
                        .Select(selector: p => new
                        {
                            Id = p.Person.TVMazeId,
                            p.Person.Name,
                            Birthday = p.Person.BirthDate
                        })
                });

            return this.Ok(value: itemsOnPage.ToList());
        }
    }
}