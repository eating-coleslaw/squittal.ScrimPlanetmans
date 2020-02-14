using static squittal.ScrimPlanetmans.Data.DbContextHelper;

namespace squittal.ScrimPlanetmans.Data
{
    public interface IDbContextHelper
    {
        DbContextFactory GetFactory();
    }
}
