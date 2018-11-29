using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TVShows.API.Infrastructure;
using TVShows.API.Model;
using TVShows.API.ViewModel;

namespace TVShows.API.Controllers
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
            long totalItems = await repository.Get().CountAsync();

            List<Show> itemsOnPage = await repository.Get()
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            PaginatedItemsViewModel<Show> model = new PaginatedItemsViewModel<Show>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }
    }
}
