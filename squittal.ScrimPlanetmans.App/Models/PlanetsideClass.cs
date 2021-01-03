namespace squittal.ScrimPlanetmans.Models
{
    public static class PlanetsideClassLoadoutTranslator
    {
        public static PlanetsideClass GetPlanetsideClass(int loadoutId)
        {
            return loadoutId switch
            {
                // NC                
                1 => PlanetsideClass.Infiltrator,
                3 => PlanetsideClass.LightAssault,
                4 => PlanetsideClass.Medic,
                5 => PlanetsideClass.Engineer,
                6 => PlanetsideClass.HeavyAssault,
                7 => PlanetsideClass.MAX,
                
                // TR
                8 => PlanetsideClass.Infiltrator,
                10 => PlanetsideClass.LightAssault,
                11 => PlanetsideClass.Medic,
                12 => PlanetsideClass.Engineer,
                13 => PlanetsideClass.HeavyAssault,
                14 => PlanetsideClass.MAX,
                
                // VS
                15 => PlanetsideClass.Infiltrator,
                17 => PlanetsideClass.LightAssault,
                18 => PlanetsideClass.Medic,
                19 => PlanetsideClass.Engineer,
                20 => PlanetsideClass.HeavyAssault,
                21 => PlanetsideClass.MAX,
                
                // NS
                28 => PlanetsideClass.Infiltrator,
                29 => PlanetsideClass.LightAssault,
                30 => PlanetsideClass.Medic,
                31 => PlanetsideClass.Engineer,
                32 => PlanetsideClass.HeavyAssault,
                45 => PlanetsideClass.MAX,

                _ => PlanetsideClass.HeavyAssault
            };
        }
    }
    
    public enum PlanetsideClass
    {
        HeavyAssault,
        LightAssault,
        Infiltrator,
        Medic,
        Engineer,
        MAX
    }
}
