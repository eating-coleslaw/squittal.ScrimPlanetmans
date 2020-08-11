using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetActionRuleConfiguration : IEntityTypeConfiguration<RulesetActionRule>
    {
        public void Configure(EntityTypeBuilder<RulesetActionRule> builder)
        {
            builder.ToTable("RulesetActionRule");

            builder.HasKey(e => new
            {
                e.RulesetId,
                e.ScrimActionType
            });

            builder.HasOne(rule => rule.Ruleset)
                .WithMany(ruleset => ruleset.RulesetActionRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.DeferToItemCategoryRules).HasDefaultValue(false);
        }
    }
}
