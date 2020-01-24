namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusItemModel
    {
        public int ItemId { get; set; }
        public int? ItemTypeId { get; set; }
        public int? ItemCategoryId { get; set; }
        public bool IsVehicleWeapon { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int? FactionId { get; set; }
        public int MaxStackSize { get; set; }
        public int? ImageId { get; set; }
    }
}
