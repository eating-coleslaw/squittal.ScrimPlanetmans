using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchParticipatingPlayerConfiguration : IEntityTypeConfiguration<ScrimMatchParticipatingPlayer>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchParticipatingPlayer> builder)
        {
            builder.ToTable("ScrimMatchParticipatingPlayer");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.CharacterId
            });

            builder.Property(e => e.IsFromOutfit).HasDefaultValue(false);
            builder.Property(e => e.IsFromConstructedTeam).HasDefaultValue(false);

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.Faction);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.ConstructedTeam);
        }
    }
}
