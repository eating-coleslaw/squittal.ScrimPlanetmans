using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetFacilityRuleConfiguration : IEntityTypeConfiguration<RulesetFacilityRule>
    {
        public void Configure(EntityTypeBuilder<RulesetFacilityRule> builder)
        {
            builder.ToTable("RulesetFacilityRule");

            builder.HasKey(e => new { e.RulesetId, e.FacilityId });

            builder.HasOne(e => e.MapRegion)
                .WithOne()
                .HasPrincipalKey<MapRegion>(r => r.FacilityId)
                .HasForeignKey<RulesetFacilityRule>(e => e.FacilityId);

            builder.HasOne(e => e.Ruleset)
                .WithMany(r => r.RulesetFacilityRules)
                .HasForeignKey(e => e.RulesetId);
        }
    }
}
