using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class PlayerLogoutConfiguration : IEntityTypeConfiguration<PlayerLogout>
    {
        public void Configure(EntityTypeBuilder<PlayerLogout> builder)
        {
            builder.ToTable("PlayerLogoutEvent");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.CharacterId
            });
        }
    }
}
