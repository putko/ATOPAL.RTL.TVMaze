using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.EntityConfigurations
{
    public class ShowPersonEntityTypeConfiguration
          : IEntityTypeConfiguration<ShowPerson>
    {
        public void Configure(EntityTypeBuilder<ShowPerson> builder)
        {
            // Mapping for table
            builder.ToTable("ShowPerson");

            builder.HasKey(showPerson => new { showPerson.PersonId, showPerson.ShowId });
            builder
              .HasOne(showPerson => showPerson.Person)
              .WithMany(p => p.Shows)
              .HasForeignKey(showPerson => showPerson.PersonId);

            builder
                .HasOne(showPerson => showPerson.Show)
                .WithMany(s => s.Persons)
                .HasForeignKey(showPerson => showPerson.ShowId);
        }
    }
}
