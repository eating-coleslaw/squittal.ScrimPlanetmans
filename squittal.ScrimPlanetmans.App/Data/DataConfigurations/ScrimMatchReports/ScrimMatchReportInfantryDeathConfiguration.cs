using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryDeathConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryDeath>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryDeath> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryDeaths");

            builder.HasNoKey();

            builder.Property(p => p.Points).HasDefaultValue(0);
            builder.Property(p => p.IsHeadshot).HasDefaultValue(false);
            builder.Property(p => p.DamageAssists).HasDefaultValue(0);
            builder.Property(p => p.ConcussionGrenadeAssists).HasDefaultValue(0);
            builder.Property(p => p.EmpGrenadeAssists).HasDefaultValue(0);
            builder.Property(p => p.FlashGrenadeAssists).HasDefaultValue(0);
            builder.Property(p => p.SpotAssists).HasDefaultValue(0);

        }
    }
}
