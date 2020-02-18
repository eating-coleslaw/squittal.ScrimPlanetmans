using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetItemCategoryRuleConfiguration : IEntityTypeConfiguration<RulesetItemCategoryRule>
    {
        public void Configure(EntityTypeBuilder<RulesetItemCategoryRule> builder)
        {
            builder.ToTable("RulesetItemCategoryRule");

            builder.HasKey(e => new
            {
                e.RulesetId,
                e.ItemCategoryId
            });

            builder.Ignore(e => e.ItemCategory);

            builder.HasOne(rule => rule.Ruleset)
                .WithMany(ruleset => ruleset.ItemCategoryRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.Property(e => e.Points).HasDefaultValue(0);
        }
    }
}
