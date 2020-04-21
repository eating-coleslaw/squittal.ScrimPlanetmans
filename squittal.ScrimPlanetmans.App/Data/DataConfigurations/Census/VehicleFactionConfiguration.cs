using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.Planetside;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class VehicleFactionConfiguration : IEntityTypeConfiguration<VehicleFaction>
    {
        public void Configure(EntityTypeBuilder<VehicleFaction> builder)
        {
            builder.ToTable("VehicleFaction");

            builder.HasKey(e => new { e.VehicleId, e.FactionId });

            //builder.HasOne(e => e.Vehicle)
            //    .WithMany(e => e.Faction)
            //    .HasForeignKey(e => e.VehicleId);
        }
    }
}
