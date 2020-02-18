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

            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
