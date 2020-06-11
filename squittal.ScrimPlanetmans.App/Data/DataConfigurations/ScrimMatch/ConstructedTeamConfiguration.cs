using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ConstructedTeamConfiguration : IEntityTypeConfiguration<ConstructedTeam>
    {
        public void Configure(EntityTypeBuilder<ConstructedTeam> builder)
        {
            builder.ToTable("ConstructedTeam");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.HideFromSelection).HasDefaultValue(false);
        }
    }
}
