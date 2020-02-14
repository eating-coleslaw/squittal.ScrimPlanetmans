using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations.Events
{
    public class GainExperienceConfiguration : IEntityTypeConfiguration<GainExperience>
    {
        public void Configure(EntityTypeBuilder<GainExperience> builder)
        {
            builder.ToTable("GainExperienceEvent");

            // TODO: add Primary Keys, maybe index
        }
    }
}
