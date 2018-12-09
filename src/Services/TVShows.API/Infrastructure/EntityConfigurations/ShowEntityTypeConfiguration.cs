namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ShowEntityTypeConfiguration
        : IEntityTypeConfiguration<Show>
    {
        public void Configure(EntityTypeBuilder<Show> builder)
        {
            // Mapping for table
            builder.ToTable(name: "Show");

            // Set key for entity
            builder.HasKey(keyExpression: p => p.Id);

            // Set mapping for columns
            builder.Property(propertyExpression: p => p.Name).IsRequired();
            builder.Property(propertyExpression: p => p.TVMazeId).IsRequired();

            // Set concurrency token for entity
            builder.Property(propertyExpression: p => p.Timestamp);
        }
    }
}