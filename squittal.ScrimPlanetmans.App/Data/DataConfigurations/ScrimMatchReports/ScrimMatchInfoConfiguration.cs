using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchInfoConfiguration : IEntityTypeConfiguration<ScrimMatchInfo>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchInfo> builder)
        {
            builder.ToView("View_ScrimMatchInfo");

            builder.HasNoKey();
        }
    }
}
