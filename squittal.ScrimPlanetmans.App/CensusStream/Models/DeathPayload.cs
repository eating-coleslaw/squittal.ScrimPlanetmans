namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class DeathPayload : PayloadBase
    {
        public string AttackerCharacterId { get; set; }
        public int? AttackerFireModeId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public int? CharacterLoadoutId { get; set; }
        public bool IsHeadshot { get; set; }
    }
}
