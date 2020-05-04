using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimVehicleDestructionConfiguration : IEntityTypeConfiguration<ScrimVehicleDestruction>
    {
        public void Configure(EntityTypeBuilder<ScrimVehicleDestruction> builder)
        {
            builder.ToTable("ScrimVehicleDestruction");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.Timestamp,
                e.AttackerCharacterId,
                e.VictimCharacterId,
                e.AttackerVehicleId,
                e.VictimVehicleId
            });

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.AttackerFaction);
            builder.Ignore(e => e.VictimFaction);
            builder.Ignore(e => e.Weapon);
            builder.Ignore(e => e.WeaponItemCategory);
            builder.Ignore(e => e.AttackerVehicle);
            builder.Ignore(e => e.VictimVehicle);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.Zone);
        }
    }
}