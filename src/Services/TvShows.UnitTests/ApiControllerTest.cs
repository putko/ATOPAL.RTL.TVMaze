using Xunit;

namespace TvShows.UnitTests
{
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Controllers;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class ApiControllerTest
    {
        private readonly Mock<IGenericRepository<Show>> _repositoryMock;

        public ApiControllerTest()
        {
            _repositoryMock = new Mock<IGenericRepository<Show>>();
        }

        [Fact]
        public async Task Get_tvshow_items_success()
        {
            var fakeTVshows = this.GetFakeTVshows();

            Func<IQueryable<Show>, IOrderedQueryable<Show>> orderBy = shows => shows.OrderBy(x => x.Name);

            string includeProperties = string.Empty;
            _repositoryMock.Setup(moq => moq.Get(It.IsAny<Expression<Func<Show, bool>>>(), orderBy, includeProperties))
                .Returns(fakeTVshows);
            // Act
            ShowsController controller = new ShowsController(_repositoryMock.Object);
            IActionResult actionResult = await controller.Get(10, 0);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.Model);
        }

        public IQueryable<Show> GetFakeTVshows()
        {
            List<Show> showList = new List<Show>()
            {
                new Show(){ Name = "Rose", Id = 2  },
                new Show(){ Name = "Mark",Id = 3 },
                new Show(){ Name = "Andrew", Id = 4  },
                new Show(){ Name = "Sam",Id = 5 },
                new Show(){ Name = "Lisa", Id = 6 },
                new Show(){ Name = "Lucy", Id = 7  }
            };

            IEnumerable<Show> result = from e in showList
                                       select e;
            return result.AsQueryable();
        }
    }
    public static class EfExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(
            this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source is IAsyncEnumerable<TSource>))
                return Task.FromResult(source.ToList());
            return source.ToListAsync();
        }
    }
}
