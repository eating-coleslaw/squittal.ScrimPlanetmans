using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ConstructedTeamFactionPreferenceConfiguration : IEntityTypeConfiguration<ConstructedTeamFactionPreference>
    {
        public void Configure(EntityTypeBuilder<ConstructedTeamFactionPreference> builder)
        {
            builder.ToTable("ConstructedTeamFactionPreference");

            builder.HasKey(e => new
            {
                e.ConstructedTeamId,
                e.PreferenceOrdinalValue
            });

            //builder.HasOne(faction => faction.ConstructedTeam)
            //    .WithMany(team => team.FactionPreferences)
            //    .HasForeignKey(faction => faction.ConstructedTeamId);
        }
    }
}
