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

    }

    public enum ScrimActionType
    {
        None = 0,
        
        // Objectives: 10-99
        FirstBaseCapture        = 10,
        SubsequentBaseCapture   = 11,
        PointControl            = 12,
        PointDefend             = 13,
        ConvertCapturePoint     = 14,
        ObjectiveCapturePulse   = 15,
        ObjectiveDefensePulse   = 16,

        // Infantry: 100 - 199
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

        // Maxes: 200 - 299
        MaxKillInfantry                = 200,
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

        // Support: 300 - 399
        ReviveInfantry              = 300,
        ReviveMax                   = 301,
        //InfantryTakeRevive          = 302,
        //MaxTakeRevive               = 303,
        DamageAssist                = 304,
        UtilityAssist               = 305,

        // Vehicles: 400 - 499        
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


        // Interceptor: 500 - 599
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

        // ESF: 600 - 699
        EsfKillInfantry         = 600,
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

        // Valkyrie: 700 - 799
        ValkyrieKillInfantry        = 700,
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

        // Liberator: 800 - 899
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

        // Galaxy: 900 - 999
        GalaxyKillInfantry          = 900,
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

        // Bastion: 1000 - 1099
        BastionKillInfantry         = 1000,
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

        // Miscellaneous: 5000 - 6099
        Login   = 5000,
        Logout  = 5001,

        // Warnings & Errors: 9000+
        OutsideInterference = 9000,
        Unknown             = 9001
    };
}
