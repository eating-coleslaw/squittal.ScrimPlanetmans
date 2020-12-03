using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimDamageAssistConfiguration : IEntityTypeConfiguration<ScrimDamageAssist>
    {
        public void Configure(EntityTypeBuilder<ScrimDamageAssist> builder)
        {
            builder.ToTable("ScrimDamageAssist");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.Timestamp,
                e.AttackerCharacterId,
                e.VictimCharacterId
            });

            builder.Property(e => e.ExperienceGainAmount).HasDefaultValue(0);
            builder.Property(e => e.Points).HasDefaultValue(0);

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.AttackerParticipatingPlayer);
            builder.Ignore(e => e.VictimParticipatingPlayer);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.Zone);
        }
    }
}
