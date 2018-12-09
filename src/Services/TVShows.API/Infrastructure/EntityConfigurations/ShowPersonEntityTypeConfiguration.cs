namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ShowPersonEntityTypeConfiguration
        : IEntityTypeConfiguration<ShowPerson>
    {
        public void Configure(EntityTypeBuilder<ShowPerson> builder)
        {
            // Mapping for table
            builder.ToTable(name: "ShowPerson");

            builder.HasKey(keyExpression: showPerson => new {showPerson.PersonId, showPerson.ShowId});
            builder
                .HasOne(navigationExpression: showPerson => showPerson.Person)
                .WithMany(navigationExpression: p => p.Shows)
                .HasForeignKey(foreignKeyExpression: showPerson => showPerson.PersonId);

            builder
                .HasOne(navigationExpression: showPerson => showPerson.Show)
                .WithMany(navigationExpression: s => s.Persons)
                .HasForeignKey(foreignKeyExpression: showPerson => showPerson.ShowId);
        }
    }
}