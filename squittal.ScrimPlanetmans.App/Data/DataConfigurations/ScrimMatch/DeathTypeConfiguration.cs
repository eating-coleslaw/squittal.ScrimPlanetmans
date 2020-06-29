using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class DeathTypeConfiguration : IEntityTypeConfiguration<DeathType>
    {
        public void Configure(EntityTypeBuilder<DeathType> builder)
        {
            builder.ToTable("DeathType");

            builder.HasKey(e => e.Type);
        }
    }
}
