using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimAction
    {
        [Required]
        public ScrimActionType Action { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        public ScrimActionTypeDomain Domain { get; set; }

        public static ScrimActionTypeDomain GetDomainFromActionType(ScrimActionType action)
        {
            if ((int)action >= 10 && (int)action < 100)
            {
                return ScrimActionTypeDomain.Objective;
            }
            else if ((int)action >= 300 && (int)action < 399)
            {
                return ScrimActionTypeDomain.Support;
            }
            else if ((int)action >= 100 && (int)action < 200)
            {
                return ScrimActionTypeDomain.Infantry;
            }
            else if ((int)action >= 200 && (int)action < 300)
            {
                return ScrimActionTypeDomain.MAX;
            }
            else if ((int)action >= 400 && (int)action < 500)
            {
                return ScrimActionTypeDomain.Vehicle;
            }
            else if ((int)action >= 500 && (int)action < 1999)
            {
                return ScrimActionTypeDomain.AirVehicle;
            }
            else if ((int)action >= 2000 && (int)action < 2999)
            {
                return ScrimActionTypeDomain.GroundVehicle;
            }
            else
            {
                return ScrimActionTypeDomain.Other;
            }
        }
        
        public ScrimActionTypeDomain SetDomain()
        {
            if ((int)Action >= 10 && (int)Action < 100)
            {
                Domain = ScrimActionTypeDomain.Objective;
            }
            else if ((int)Action >= 300 && (int)Action < 399)
            {
                Domain = ScrimActionTypeDomain.Support;
            }
            else if ((int)Action >= 100 && (int)Action < 200)
            {
                Domain = ScrimActionTypeDomain.Infantry;
            }
            else if ((int)Action >= 200 && (int)Action < 300)
            {
                Domain = ScrimActionTypeDomain.MAX;
            }
            else if ((int)Action >= 400 && (int)Action < 500)
            {
                Domain = ScrimActionTypeDomain.Vehicle;
            }
            else if ((int)Action >= 500 && (int)Action < 1999)
            {
                Domain = ScrimActionTypeDomain.AirVehicle;
            }
            else if ((int)Action >= 2000 && (int)Action < 2999)
            {
                Domain = ScrimActionTypeDomain.GroundVehicle;
            }
            else
            {
                Domain = ScrimActionTypeDomain.Other;
            }
            
            return Domain;
        }
    }

    public enum ScrimActionType
    {
        None = 0,

        #region Objectives: 10-99
        FirstBaseCapture        = 10,
        SubsequentBaseCapture   = 11,
        PointControl            = 12,
        PointDefend             = 13,
        ConvertCapturePoint     = 14,
        ObjectiveCapturePulse   = 15,
        ObjectiveDefensePulse   = 16,
        #endregion Objectives: 10-99


        #region Infantry: 100 - 199
        InfantryKillInfantry         = 100,
        InfantryKillMax              = 101,
        InfantryTeamkillInfantry     = 102,
        InfantryTeamkillMax          = 103,
        InfantrySuicide              = 104,

        //InfantryDeath                = 105,
        //InfantryTeamkillDeath        = 190,
        InfantryTeamkillVehicle = 191,
        InfantryKillVehicle = 192,

        InfantryDestroyInterceptor      = 106,
        InfantryTeamDestroyInterceptor  = 107,
        InfantryDestroyEsf              = 108,
        InfantryTeamDestroyEsf          = 109,
        InfantryDestroyValkyrie         = 110,
        InfantryTeamDestroyValkyrie     = 111,
        InfantryDestroyLiberator        = 112,
        InfantryTeamDestroyLiberator    = 113,
        InfantryDestroyGalaxy           = 114,
        InfantryTeamDestroyGalaxy       = 115,
        InfantryDestroyBastion          = 116,
        InfantryTeamDestroyBastion      = 117,

        InfantryDestroyFlash            = 118,
        InfantryTeamDestroyFlash        = 119,
        InfantryDestroyHarasser         = 120,
        InfantryTeamDestroyHarasser     = 121,
        InfantryDestroyAnt              = 122,
        InfantryTeamDestroyAnt          = 123,
        InfantryDestroySunderer         = 124,
        InfantryTeamDestroySunderer     = 125,
        InfantryDestroyLightning        = 126,
        InfantryTeamDestroyLightning    = 127,
        InfantryDestroyMbt              = 128,
        InfantryTeamDestroyMbt          = 129,

        InfantryDestroyColossus         = 130, 
        InfantryTeamDestroyColossus     = 131, 
        #endregion Infantry: 100 - 199


        #region Maxes: 200 - 299
        MaxKillInfantry = 200,
        MaxKillMax                     = 201,
        MaxTeamkillMax                 = 202,
        MaxTeamkillInfantry            = 203,
        MaxSuicide                     = 205,

        //MaxDeath                    = 204,
        //MaxTeamkillDeath            = 290,
        MaxTeamkillVehicle             = 291,
        MaxKillVehicle                 = 292,

        MaxDestroyInterceptor          = 206,
        MaxTeamDestroyInterceptor      = 207,
        MaxDestroyEsf                  = 208,
        MaxTeamDestroyEsf              = 209,
        MaxDestroyValkyrie             = 210,
        MaxTeamDestroyValkyrie         = 211,
        MaxDestroyLiberator            = 212,
        MaxTeamDestroyLiberator        = 213,
        MaxDestroyGalaxy               = 214,
        MaxTeamDestroyGalaxy           = 215,
        MaxDestroyBastion              = 216,
        MaxTeamDestroyBastion          = 217,

        MaxDestroyFlash                 = 218,
        MaxTeamDestroyFlash             = 219,
        MaxDestroyHarasser              = 220,
        MaxTeamDestroyHarasser          = 221,
        MaxDestroyAnt                   = 222,
        MaxTeamDestroyAnt               = 223,
        MaxDestroySunderer              = 224,
        MaxTeamDestroySunderer          = 225,
        MaxDestroyLightning             = 226,
        MaxTeamDestroyLightning         = 227,
        MaxDestroyMbt                   = 228,
        MaxTeamDestroyMbt               = 229,

        MaxDestroyColossus              = 230,
        MaxTeamDestroyColossus          = 231,
        #endregion Maxes: 200 - 299


        #region Support: 300 - 399
        ReviveInfantry              = 300,
        ReviveMax                   = 301,
        EnemyRevivedInfantry        = 302,
        EnemyRevivedMax             = 303,
        //InfantryTakeRevive          = 302,
        //MaxTakeRevive               = 303,
        DamageAssist                = 304,
        UtilityAssist               = 305,
        GrenadeAssist               = 306,
        HealSupportAssist           = 307,
        SpotAssist                  = 308,
        ProtectAlliesAssist         = 309,
        DamageTeamAssist            = 310,
        GrenadeTeamAssist           = 311,
        DamageSelfAssist            = 312,
        GrenadeSelfAssist           = 313,
        #endregion Support: 300 - 399

        #region Vehicles: 400 - 499        
        VehicleKillInfantry                = 400,
        VehicleKillMax                     = 401,
        VehicleKillVehicle                 = 402,
        VehicleTeamkillInfantry            = 403,
        VehicleTeamkillMax                 = 404,
        VehicleTeamkillVehicle             = 405,
        //VehicleSuicide              = 406,
        //VehicleDeath                = 407,
        //VehicleTeamkillDeath        = 408,
        VehicleDestroyInterceptor          = 406,
        VehicleTeamDestroyInterceptor      = 407,
        VehicleDestroyEsf                  = 408,
        VehicleTeamDestroyEsf              = 409,
        VehicleDestroyValkyrie             = 410,
        VehicleTeamDestroyValkyrie         = 411,
        VehicleDestroyLiberator            = 412,
        VehicleTeamDestroyLiberator        = 413,
        VehicleDestroyGalaxy               = 414,
        VehicleTeamDestroyGalaxy           = 415,
        VehicleDestroyBastion              = 416,
        VehicleTeamDestroyBastion          = 417,

        VehicleDestroyFlash                 = 418,
        VehicleTeamDestroyFlash             = 419,
        VehicleDestroyHarasser              = 420,
        VehicleTeamDestroyHarasser          = 421,
        VehicleDestroyAnt                   = 422,
        VehicleTeamDestroyAnt               = 423,
        VehicleDestroySunderer              = 424,
        VehicleTeamDestroySunderer          = 425,
        VehicleDestroyLightning             = 426,
        VehicleTeamDestroyLightning         = 427,
        VehicleDestroyMbt                   = 428,
        VehicleTeamDestroyMbt               = 429,

        VehicleSuicide                      = 430,

        VehicleDestroyColossus              = 431,
        VehicleTeamDestroyColossus          = 432,
        #endregion Vehicles: 400 - 499        

        #region Interceptor: 500 - 599
        /*
        InterceptorKillInfantry         = 500,
        InterceptorKillMax              = 501,
        InterceptorTeamkillInfantry     = 502,
        InterceptorTeamkillMax          = 503,
        InterceptorSuicide              = 504,

        InterceptorKillFlash            = 505,
        InterceptorKillHarasser         = 506,
        InterceptorKillAnt              = 507,
        InterceptorKillSunderer         = 508,
        InterceptorKillLightning        = 509,
        InterceptorKillMbt              = 510,
        InterceptorKillInterceptor      = 511,
        InterceptorKillEsf              = 512,
        InterceptorKillValkyrie         = 513,
        InterceptorKillLiberator        = 514,
        InterceptorKillGalaxy           = 515,
        InterceptorKillBastion          = 516,

        InterceptorTeamkillFlash        = 517,
        InterceptorTeamkillHarasser     = 518,
        InterceptorTeamkillAnt          = 519,
        InterceptorTeamkillSunderer     = 520,
        InterceptorTeamkillLightning    = 521,
        InterceptorTeamkillMbt          = 522,
        InterceptorTeamkillInterceptor  = 523,
        InterceptorTeamkillEsf          = 524,
        InterceptorTeamkillValkyrie     = 525,
        InterceptorTeamkillLiberator    = 526,
        InterceptorTeamkillGalaxy       = 527,
        InterceptorTeamkillBastion      = 528,
        */
        InterceptorSuicide = 529,
        #endregion Interceptor: 500 - 599

        #region ESF: 600 - 699
        /*
        EsfKillInfantry = 600,
        EsfKillMax              = 601,
        EsfTeamkillInfantry     = 602,
        EsfTeamkillMax          = 603,
        EsfSuicide              = 604,

        EsfKillFlash            = 605,
        EsfKillHarasser         = 606,
        EsfKillAnt              = 607,
        EsfKillSunderer         = 608,
        EsfKillLightning        = 609,
        EsfKillMbt              = 610,
        EsfKillInterceptor      = 611,
        EsfKillEsf              = 612,
        EsfKillValkyrie         = 613,
        EsfKillLiberator        = 614,
        EsfKillGalaxy           = 615,
        EsfKillBastion          = 616,

        EsfTeamkillFlash        = 617,
        EsfTeamkillHarasser     = 618,
        EsfTeamkillAnt          = 619,
        EsfTeamkillSunderer     = 620,
        EsfTeamkillLightning    = 621,
        EsfTeamkillMbt          = 622,
        EsfTeamkillInterceptor  = 623,
        EsfTeamkillEsf          = 624,
        EsfTeamkillValkyrie     = 625,
        EsfTeamkillLiberator    = 626,
        EsfTeamkillGalaxy       = 627,
        EsfTeamkillBastion      = 628,
        */
        EsfSuicide              = 629,
        #endregion ESF: 600 - 699

        #region Valkyrie: 700 - 799
        /*
        ValkyrieKillInfantry = 700,
        ValkyrieKillMax             = 701,
        ValkyrieTeamkillInfantry    = 702,
        ValkyrieTeamkillMax         = 703,
        ValkyrieSuicide             = 704,

        ValkyrieKillFlash           = 705,
        ValkyrieKillHarasser        = 706,
        ValkyrieKillAnt             = 707,
        ValkyrieKillSunderer        = 708,
        ValkyrieKillLightning       = 709,
        ValkyrieKillMbt             = 710,
        ValkyrieKillInterceptor     = 711,
        ValkyrieKillEsf             = 712,
        ValkyrieKillValkyrie        = 713,
        ValkyrieKillLiberator       = 714,
        ValkyrieKillGalaxy          = 715,
        ValkyrieKillBastion         = 716,

        ValkyrieTeamkillFlash       = 717,
        ValkyrieTeamkillHarasser    = 718,
        ValkyrieTeamkillAnt         = 719,
        ValkyrieTeamkillSunderer    = 720,
        ValkyrieTeamkillLightning   = 721,
        ValkyrieTeamkillMbt         = 722,
        ValkyrieTeamkillInterceptor = 723,
        ValkyrieTeamkillEsf         = 724,
        ValkyrieTeamkillValkyrie    = 725,
        ValkyrieTeamkillLiberator   = 726,
        ValkyrieTeamkillGalaxy      = 727,
        ValkyrieTeamkillBastion     = 728,
        */
        ValkyrieSuicide             = 729,
        #endregion Valkyrie: 700 - 799

        #region Liberator: 800 - 899
        /*
        LiberatorKillInfantry        = 800,
        LiberatorKillMax             = 801,
        LiberatorTeamkillInfantry    = 802,
        LiberatorTeamkillMax         = 803,
        LiberatorSuicide             = 804,
                                     
        LiberatorKillFlash           = 805,
        LiberatorKillHarasser        = 806,
        LiberatorKillAnt             = 807,
        LiberatorKillSunderer        = 808,
        LiberatorKillLightning       = 809,
        LiberatorKillMbt             = 810,
        LiberatorKillInterceptor     = 811,
        LiberatorKillEsf             = 812,
        LiberatorKillValkyrie        = 813,
        LiberatorKillLiberator       = 814,
        LiberatorKillGalaxy          = 815,
        LiberatorKillBastion         = 816,
                                     
        LiberatorTeamkillFlash       = 817,
        LiberatorTeamkillHarasser    = 818,
        LiberatorTeamkillAnt         = 819,
        LiberatorTeamkillSunderer    = 820,
        LiberatorTeamkillLightning   = 821,
        LiberatorTeamkillMbt         = 822,
        LiberatorTeamkillInterceptor = 823,
        LiberatorTeamkillEsf         = 824,
        LiberatorTeamkillValkyrie    = 825,
        LiberatorTeamkillLiberator   = 826,
        LiberatorTeamkillGalaxy      = 827,
        LiberatorTeamkillBastion     = 828,
        */
        LiberatorSuicide             = 829,
        #endregion Liberator: 800 - 899

        #region Galaxy: 900 - 999
        /*
        GalaxyKillInfantry = 900,
        GalaxyKillMax               = 901,
        GalaxyTeamkillInfantry      = 902,
        GalaxyTeamkillMax           = 903,
        GalaxySuicide               = 904,

        GalaxyKillFlash             = 905,
        GalaxyKillHarasser          = 906,
        GalaxyKillAnt               = 907,
        GalaxyKillSunderer          = 908,
        GalaxyKillLightning         = 909,
        GalaxyKillMbt               = 910,
        GalaxyKillInterceptor       = 911,
        GalaxyKillEsf               = 912,
        GalaxyKillValkyrie          = 913,
        GalaxyKillLiberator         = 914,
        GalaxyKillGalaxy            = 915,
        GalaxyKillBastion           = 916,

        GalaxyTeamkillFlash         = 917,
        GalaxyTeamkillHarasser      = 918,
        GalaxyTeamkillAnt           = 919,
        GalaxyTeamkillSunderer      = 920,
        GalaxyTeamkillLightning     = 921,
        GalaxyTeamkillMbt           = 922,
        GalaxyTeamkillInterceptor   = 923,
        GalaxyTeamkillEsf           = 924,
        GalaxyTeamkillValkyrie      = 925,
        GalaxyTeamkillLiberator     = 926,
        GalaxyTeamkillGalaxy        = 927,
        GalaxyTeamkillBastion       = 928,
        */
        GalaxySuicide               = 929,
        #endregion Galaxy: 900 - 999

        #region Bastion: 1000 - 1099
        /*
        BastionKillInfantry = 1000,
        BastionKillMax              = 1001,
        BastionTeamkillInfantry     = 1002,
        BastionTeamkillMax          = 1003,
        BastionSuicide              = 1004,

        BastionKillFlash            = 1005,
        BastionKillHarasser         = 1006,
        BastionKillAnt              = 1007,
        BastionKillSunderer         = 1008,
        BastionKillLightning        = 1009,
        BastionKillMbt              = 1010,
        BastionKillInterceptor      = 1011,
        BastionKillEsf              = 1012,
        BastionKillValkyrie         = 1013,
        BastionKillLiberator        = 1014,
        BastionKillGalaxy           = 1015,
        BastionKillBastion          = 1016,

        BastionTeamkillFlash        = 1017,
        BastionTeamkillHarasser     = 1018,
        BastionTeamkillAnt          = 1019,
        BastionTeamkillSunderer     = 1020,
        BastionTeamkillLightning    = 1021,
        BastionTeamkillMbt          = 1022,
        BastionTeamkillInterceptor  = 1023,
        BastionTeamkillEsf          = 1024,
        BastionTeamkillValkyrie     = 1025,
        BastionTeamkillLiberator    = 1026,
        BastionTeamkillGalaxy       = 1027,
        BastionTeamkillBastion      = 1028,
        */
        BastionSuicide              = 1029,
        #endregion Bastion: 1000 - 1099

        #region Flash: 2000 - 2099
        FlashSuicide = 2029,
        #endregion Flash: 2000 - 2099

        #region Harasser: 2100 - 2199
        HarasserSuicide = 2129,
        #endregion Harasser: 2100 - 2199

        #region ANT: 2200 - 2299
        AntSuicide = 2229,
        #endregion ANT: 2200 - 2299

        #region Sunderer: 2300 - 2399
        SundererSuicide = 2329,
        #endregion Sunderer: 2300 - 2399

        #region Lightning: 2400 - 2499
        LightningSuicide = 2429,
        #endregion Lightning: 2400 - 2499

        #region MBT: 2500 - 2599
        MbtSuicide = 2529,
        #endregion MBT: 2500 - 2599

        #region Colossus: 2600 - 2699
        ColossusSuicide = 2629,
        #endregion Colossus: 2600 - 2699

        #region Miscellaneous: 5000 - 6099
        Login = 5000,
        Logout  = 5001,
        #endregion Miscellaneous: 5000 - 6099

        #region Warnings & Errors: 9000+
        OutsideInterference = 9000,
        Unknown             = 9001
        #endregion Warnings & Errors: 9000+
    };
}
