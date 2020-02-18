using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimActionConfiguration : IEntityTypeConfiguration<ScrimAction>
    {
        public void Configure(EntityTypeBuilder<ScrimAction> builder)
        {
            builder.ToTable("ScrimAction");

            builder.HasKey(e => e.Action);
        }
    }
}
