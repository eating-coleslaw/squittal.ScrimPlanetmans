using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.DataConfigurations;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;

namespace squittal.ScrimPlanetmans.Data
{
    public class PlanetmansDbContext : DbContext
    {
        public PlanetmansDbContext(DbContextOptions<PlanetmansDbContext> options)
            : base(options)
        {
        }

        #region Census DbSets
        //public DbSet<Character> Characters { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Loadout> Loadouts { get; set; }
        //public DbSet<Outfit> Outfits { get; set; }
        //public DbSet<OutfitMember> OutfitMembers { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Zone> Zones { get; set; }
        #endregion

        #region Stream Event DbSets
        //public DbSet<Death> Deaths { get; set; }
        //public DbSet<PlayerLogin> PlayerLogins { get; set; }
        //public DbSet<PlayerLogout> PlayerLogouts { get; set; }
        #endregion

        #region Scrim Match DbSets
        public DbSet<ScrimAction> ScrimActions { get; set; }
        public DbSet<Ruleset> Rulesets { get; set; }
        public DbSet<RulesetActionRule> RulesetActionRule { get; set; }
        public DbSet<RulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Census Configuration
            //builder.ApplyConfiguration(new CharacterConfiguration());
            builder.ApplyConfiguration(new FactionConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new ItemCategoryConfiguration());
            builder.ApplyConfiguration(new LoadoutConfiguration());
            //builder.ApplyConfiguration(new OutfitConfiguration());
            //builder.ApplyConfiguration(new OutfitMemberConfiguration());
            builder.ApplyConfiguration(new ProfileConfiguration());
            builder.ApplyConfiguration(new WorldConfiguration());
            builder.ApplyConfiguration(new ZoneConfiguration());
            #endregion

            #region Stream Configuration
            //builder.ApplyConfiguration(new DeathConfiguration());
            //builder.ApplyConfiguration(new PlayerLoginConfiguration());
            //builder.ApplyConfiguration(new PlayerLogoutConfiguration());
            #endregion

            #region Scrim Match DbSets
            builder.ApplyConfiguration(new ScrimActionConfiguration());
            builder.ApplyConfiguration(new RulesetConfiguration());
            builder.ApplyConfiguration(new RulesetActionRuleConfiguration());
            builder.ApplyConfiguration(new RulesetItemCategoryRuleConfiguration());
            #endregion
        }
    }
}
