namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PersonEntityTypeConfiguration
        : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Mapping for table
            builder.ToTable(name: "Person");

            // Set key for entity
            builder.HasKey(keyExpression: p => p.Id);

            // Set mapping for columns
            builder.Property(propertyExpression: p => p.TVMazeId).IsRequired();
            builder.Property(propertyExpression: p => p.Name).IsRequired();
            builder.Property(propertyExpression: p => p.BirthDate);
        }
    }
}