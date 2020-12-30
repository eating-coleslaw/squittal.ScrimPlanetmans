using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetItemPlanetsideClassRuleConfiguration : IEntityTypeConfiguration<RulesetItemPlanetsideClassRule>
    {
        public void Configure(EntityTypeBuilder<RulesetItemPlanetsideClassRule> builder)
        {
            builder.ToTable("RulesetItemPlanetsideClassRule");

            builder.HasKey(e => new
            {
                e.RulesetId,
                e.ItemId,
                e.PlanetsideClass
            });

            builder.Ignore(e => e.Item);
            builder.Ignore(e => e.ItemCategory);

            builder.HasOne(rule => rule.Ruleset)
                .WithMany(ruleset => ruleset.RulesetItemPlanetsideClassRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.HasOne(rule => rule.Item)
                .WithOne();

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.IsBanned).HasDefaultValue(false);
        }
    }
}