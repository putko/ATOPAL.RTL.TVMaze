using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    public class PersonEntityTypeConfiguration
          : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Mapping for table
            builder.ToTable("Person");

            // Set key for entity
            builder.HasKey(p => p.Id);

            // Set mapping for columns
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.BirthDate).IsRequired();
        }
    }
}
