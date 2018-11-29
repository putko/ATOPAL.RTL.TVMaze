using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TVShows.API.Infrastructure.EntityConfigurations;
using TVShows.API.Model;

namespace TVShows.API.Infrastructure
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
        //public TVShowContext CreateDbContext(string[] args)
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();
        //    var builder = new DbContextOptionsBuilder<TVShowContext>();
        //    var connectionString = configuration.GetConnectionString("ConnectionString");
        //    builder.UseSqlServer(connectionString);
        //    return new TVShowContext(builder.Options);
        //}
    }
}
