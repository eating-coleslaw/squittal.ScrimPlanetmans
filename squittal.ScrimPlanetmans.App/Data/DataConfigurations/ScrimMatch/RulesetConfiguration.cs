using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetConfiguration : IEntityTypeConfiguration<Ruleset>
    {
        public void Configure(EntityTypeBuilder<Ruleset> builder)
        {
            builder.ToTable("Ruleset");

            builder.HasKey(e => e.Id);

            //builder.Property(e => e.IsActive).HasDefaultValue(false);
            builder.Property(e => e.IsCustomDefault).HasDefaultValue(false);
            builder.Property(e => e.IsDefault).HasDefaultValue(false);
            builder.Property(e => e.DefaultRoundLength).HasDefaultValue(900);
            //builder.Property(e => e.DefaultMatchTitle).HasDefaultValue(string.Empty);
        }
    }
}
