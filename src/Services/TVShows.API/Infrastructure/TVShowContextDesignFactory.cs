namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class TVShowContextDesignFactory : IDesignTimeDbContextFactory<TVShowContext>
    {
        public TVShowContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TVShowContext>()
                .UseSqlServer(
                    connectionString: "Server=localhost,5443;Initial Catalog=TVShowsDB;User Id=sa;Password=Pass@word");

            return new TVShowContext(options: optionsBuilder.Options);
        }
    }
}