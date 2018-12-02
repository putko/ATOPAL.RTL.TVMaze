using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    public class ShowEntityTypeConfiguration
          : IEntityTypeConfiguration<Show>
    {
        public void Configure(EntityTypeBuilder<Show> builder)
        {
            // Mapping for table
            builder.ToTable("Show");

            // Set key for entity
            builder.HasKey(p => p.Id);

            // Set mapping for columns
            builder.Property(p => p.Name).IsRequired();

            // Set concurrency token for entity
            builder.Property(p => p.Timestamp).IsConcurrencyToken();

        }
    }
}
