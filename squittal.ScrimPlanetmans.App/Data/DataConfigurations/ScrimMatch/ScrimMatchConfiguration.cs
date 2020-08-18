using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchConfiguration : IEntityTypeConfiguration<Models.ScrimMatch>
    {
        public void Configure(EntityTypeBuilder<Models.ScrimMatch> builder)
        {
            builder.ToTable("ScrimMatch");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.RulesetId).HasDefaultValue(-1);

            builder.HasOne(r => r.Ruleset)
                .WithOne();
        }
    }
}
