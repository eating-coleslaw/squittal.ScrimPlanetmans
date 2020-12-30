using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetItemCategoryPlanetsideClassRuleConfiguration : IEntityTypeConfiguration<RulesetItemCategoryPlanetsideClassRule>
    {
        public void Configure(EntityTypeBuilder<RulesetItemCategoryPlanetsideClassRule> builder)
        {
            builder.ToTable("RulesetItemCategoryPlanetsideClassRule");

            builder.HasKey(e => new
            {
                e.RulesetId,
                e.ItemCategoryId,
                e.PlanetsideClass
            });

            builder.Ignore(e => e.ItemCategory);

            builder.HasOne(rule => rule.Ruleset)
                .WithMany(ruleset => ruleset.RulesetItemCategoryPlanetsideClassRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.IsBanned).HasDefaultValue(false);
        }
    }
}