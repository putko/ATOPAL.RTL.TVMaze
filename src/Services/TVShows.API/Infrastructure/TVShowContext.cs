using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations;
using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure
{
    public class TVShowContext : DbContext
    {
        public TVShowContext(DbContextOptions<TVShowContext> options) : base(options)
        {
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<ShowPerson> ShowPersons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ShowEntityTypeConfiguration());
            builder.ApplyConfiguration(new PersonEntityTypeConfiguration());
            builder.ApplyConfiguration(new ShowPersonEntityTypeConfiguration());
        }
    }


    public class TVShowContextDesignFactory : IDesignTimeDbContextFactory<TVShowContext>
    {
        public TVShowContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TVShowContext>()
                .UseSqlServer("Server=localhost,5443;Initial Catalog=TVShowsDB;User Id=sa;Password=Pass@word");

            return new TVShowContext(optionsBuilder.Options);
        }
    }
}
