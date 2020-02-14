using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class DeathConfiguration : IEntityTypeConfiguration<Death>
    {
        public void Configure(EntityTypeBuilder<Death> builder)
        {
            builder.ToTable("DeathEvent");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.CharacterId,
                e.AttackerCharacterId
            });

            builder.HasIndex(e => new
            {
                e.Timestamp,
                e.AttackerCharacterId,
                e.DeathEventType
            });

            builder.HasIndex(e => new
            {
                e.Timestamp,
                e.CharacterId,
                e.DeathEventType
            });

            builder
                .Ignore(e => e.Character)
                .Ignore(e => e.AttackerCharacter)
                .Ignore(e => e.CharacterOutfit)
                .Ignore(e => e.AttackerOutfit)
                //.Ignore(e => e.AttackerVehicle)
                .Ignore(e => e.AttackerWeapon);
        }
    }
}
