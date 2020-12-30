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

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.IsBanned).HasDefaultValue(false);
            builder.Property(e => e.DeferToPlanetsideClassSettings).HasDefaultValue(false);
            builder.Property(e => e.InfiltratorIsBanned).HasDefaultValue(false);
            builder.Property(e => e.InfiltratorPoints).HasDefaultValue(false);
            builder.Property(e => e.LightAssaultIsBanned).HasDefaultValue(false);
            builder.Property(e => e.LightAssaultPoints).HasDefaultValue(0);
            builder.Property(e => e.MedicIsBanned).HasDefaultValue(false);
            builder.Property(e => e.MedicPoints).HasDefaultValue(0);
            builder.Property(e => e.EngineerIsBanned).HasDefaultValue(false);
            builder.Property(e => e.EngineerPoints).HasDefaultValue(0);
            builder.Property(e => e.HeavyAssaultIsBanned).HasDefaultValue(false);
            builder.Property(e => e.HeavyAssaultPoints).HasDefaultValue(0);
            builder.Property(e => e.MaxIsBanned).HasDefaultValue(false);
            builder.Property(e => e.MaxPoints).HasDefaultValue(0);
        }
    }
}
