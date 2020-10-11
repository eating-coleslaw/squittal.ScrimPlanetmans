using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetItemRuleConfiguration : IEntityTypeConfiguration<RulesetItemRule>
    {
        public void Configure(EntityTypeBuilder<RulesetItemRule> builder)
        {
            builder.ToTable("RulesetItemRule");

            builder.HasKey(e => new
            {
                e.RulesetId,
                e.ItemId
            });

            builder.Ignore(e => e.Item);
            builder.Ignore(e => e.ItemCategory);

            builder.HasOne(rule => rule.Ruleset)
                .WithMany(ruleset => ruleset.RulesetItemRules)
                .HasForeignKey(rule => rule.RulesetId);

            builder.HasOne(rule => rule.Item)
                .WithOne();

            //builder.HasOne(rule => rule.ItemCategory)
            //    .WithMany();
            //.HasForeignKey(Ruleset)

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.IsBanned).HasDefaultValue(false);
        }
    }
}
