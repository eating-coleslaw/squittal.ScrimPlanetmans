namespace squittal.ScrimPlanetmans.Services
{
    public interface ILocallyBackedStore
    {
        string BackupSqlScriptFileName { get; }

        void RefreshStoreFromBackup();
    }
}
