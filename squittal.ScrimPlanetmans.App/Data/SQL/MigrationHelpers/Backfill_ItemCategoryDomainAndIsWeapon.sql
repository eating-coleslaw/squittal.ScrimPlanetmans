USE [PlanetmansDbContext];

SET NOCOUNT ON;

DECLARE @iDefaultDomainValue int = -1,
        @iInfantry           int = 1,
        @iMax                int = 2,
        @iGroundVehicle      int = 3,
        @iAirVehicle         int = 4,
        @iOther              int = 5,
        @iLocked             int = 6;

-- Column exists and caller has permission to view the object
If COL_LENGTH('ItemCategory', 'Domain') IS NOT NULL
BEGIN
  UPDATE ItemCategory
    SET Domain = CASE WHEN Id IN ( 2, 3, 4, 5, 6, 7, 8, 11, 12, 13, 14, 17, 18, 19, 24, 100, 102, 147, 157 )
                        THEN @iInfantry
                      WHEN Id IN ( 9, 10, 16, 20, 21, 22, 23 )
                        THEN @iMax
                      WHEN Id IN ( 109, 114, 118, 119, 120, 123, 124, 129, 130, 131, 132, 144 )
                        THEN @iGroundVehicle
                      WHEN Id IN ( 110, 111, 112, 113, 115, 116, 117, 121, 122, 125, 126, 127, 128, 138 )
                        THEN @iAirVehicle
                      WHEN Id IN ( 15 )
                        THEN @iLocked
                      ELSE @iOther END
    WHERE Domain = @iDefaultDomainValue;
END;

-- Column exists and caller has permission to view the object
IF COL_LENGTH('ItemCategory', 'IsWeaponCategory') IS NOT NULL
BEGIN
  UPDATE ItemCategory
    SET IsWeaponCategory = CASE WHEN Id IN ( 99,  101, 103, 105, 106, 107, 108, 133, 134, 135, 136, 137, 139, 140, 141, 142, 143, 145, 148 )
                                  THEN 0
                                ELSE 1 END;
END;

SET NOCOUNT OFF;