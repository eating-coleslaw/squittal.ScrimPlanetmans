using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimReviveConfiguration : IEntityTypeConfiguration<ScrimRevive>
    {
        public void Configure(EntityTypeBuilder<ScrimRevive> builder)
        {
            builder.ToTable("ScrimRevive");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.Timestamp,
                e.MedicCharacterId,
                e.RevivedCharacterId
            });

            builder.Property(e => e.ExperienceGainAmount).HasDefaultValue(0);
            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.EnemyPoints).HasDefaultValue(0);
            builder.Property(e => e.EnemyActionType).HasDefaultValue(ScrimActionType.Unknown);
            builder.Property(e => e.LastKilledByCharacterId).HasDefaultValue(null);

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.MedicParticipatingPlayer);
            builder.Ignore(e => e.RevivedParticipatingPlayer);
            builder.Ignore(e => e.LastKilledByParticipatingPlayer);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.Zone);
        }
    }
}
