namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure
{
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.EntityFrameworkCore;

    public class TVShowContext : DbContext
    {
        public TVShowContext(DbContextOptions<TVShowContext> options) : base(options: options)
        {
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<ShowPerson> ShowPersons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(configuration: new ShowEntityTypeConfiguration());
            builder.ApplyConfiguration(configuration: new PersonEntityTypeConfiguration());
            builder.ApplyConfiguration(configuration: new ShowPersonEntityTypeConfiguration());
        }
    }
}