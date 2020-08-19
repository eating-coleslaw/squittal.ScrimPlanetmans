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

            builder.Ignore(e => e.MapRegion);

            builder.HasOne(e => e.MapRegion)
                .WithOne()
                .HasPrincipalKey<MapRegion>(r => r.FacilityId)
                .HasForeignKey<RulesetFacilityRule>(e => e.FacilityId);

            //builder.Ignore(p => p.MapRegion);

            //builder.HasOne(e => e.MapRegion)
            //    .WithOne<MapRegion>(r =>);
            //.HasPrincipalKey<MapRegion>(r => new { r.Id, r.FacilityId })
            //.HasForeignKey<RulesetFacilityRule>(e => new { e.MapRegionId, e.FacilityId });

            //builder.HasOne(e => e.MapRegion)
            //    .WithOne()
            //    .HasForeignKey<RulesetFacilityRule>(e => new { e.MapRegionId, e.FacilityId });

            //builder.HasOne(e => e.MapRegion)
            //    .WithOne()
            //    .HasForeignKey(typeof(MapRegion), new string[] { "Id", "FacilityId" }); // r => new { r.MapRegionId, r.FacilityId });
                //.HasForeignKey(typeof(RulesetFacilityRule), new string[] { "MapRegionId", "FacilityId" }); // r => new { r.MapRegionId, r.FacilityId });

            builder.HasOne(e => e.Ruleset)
                .WithMany(r => r.RulesetFacilityRules)
                .HasForeignKey(e => e.RulesetId);
        }
    }
}
