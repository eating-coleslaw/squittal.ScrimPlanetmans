USE [PlanetmansDbContext];

SET NOCOUNT ON;

DECLARE @infantryDomainId int = 1,
        @groundDomainId   int = 3,
        @airDomainId      int = 4;

-- Column exists and caller has permission to view the object
If COL_LENGTH('ItemCategory', 'Domain') IS NOT NULL
BEGIN
  -- Infantry Item Categories
  UPDATE ItemCategory
  SET Domain = @infantryDomainId
    WHERE Id IN (219,	 -- Heavy Crossbow
                 220,	 -- Amphibious Rifle
                 223,	 -- Amphibious Sidearm
                 224); -- Anti-Materiel Rifle

  -- Ground Vehicle Item Categories
  UPDATE ItemCategory
  SET Domain = @groundDomainId
    WHERE Id IN (211,  -- Colossus Primary Weapon
                 212,  -- Colossus Front Right Weapon
                 213,  -- Colossus Front Left Weapon
                 214,  -- Colossus Rear Right Weapon
                 215,  -- Colossus Rear Left Weapon
                 216,  -- Javelin Primary Weapon
                 217,  -- Chimera Primary Weapons
                 218,  -- Chimera Secondary Weapons	5
                 221,  -- Corsair Front Turret	5
                 222); -- Corsair Rear Turret	5

  -- Air Vehicle Item Categories
  UPDATE ItemCategory
  SET Domain = @airDomainId
    WHERE Id IN (208,  -- Bastion Point Defense	5
                 209,  -- Bastion Bombard	5
                 210);  -- Bastion Weapon System	5
END;

SET NOCOUNT OFF;



