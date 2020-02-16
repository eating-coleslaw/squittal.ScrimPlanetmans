using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimActionModelConfiguration : IEntityTypeConfiguration<ScrimActionModel>
    {
        public void Configure(EntityTypeBuilder<ScrimActionModel> builder)
        {
            builder.ToTable("ScrimAction");

            builder.HasKey(e => e.Action);
        }
    }
}
