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
                .WithMany(ruleset => ruleset.RulesetItemCategoryRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.HasOne(rule => rule.ItemCategory)
                .WithOne();

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.IsBanned).HasDefaultValue(false);
            builder.Property(e => e.DeferToItemRules).HasDefaultValue(false);
        }
    }
}
