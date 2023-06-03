USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Vehicle' )
BEGIN
    
  CREATE TABLE #Staging_Vehicle
  ( Id             int NOT NULL,
    Name           nvarchar(max) NULL,
    TypeId         int NULL,
    TypeName       nvarchar(max) NULL,
    Description    nvarchar(max) NULL,
    Cost           int NULL,
    CostResourceId int null,
    ImageId        nvarchar(max) NULL );

  INSERT INTO #Staging_Vehicle VALUES
    (1, N'Flash', 5, N'Four Wheeled Ground Vehicle', N'The Flash is an ATV capable of getting the driver and one passenger quickly to the front lines.  The passenger is able to use most of their personal equipment while riding on the back.', 0, 4, 3975),
    (2, N'Sunderer', 5, N'Four Wheeled Ground Vehicle', N'The Sunderer is a large transport capable of holding 12 soldiers.  It can also be outfitted with several support functions such as a mobile respawn station, vehicle repair or vehicle ammo resupply.', 0, 4, 4002),
    (3, N'Lightning', 5, N'Four Wheeled Ground Vehicle', N'The Lightning is a light one man tank.  The Lightning can be outfitted with various types of turrets allowing it to fill anti-air and anti-armor roles.', 0, 4, 3984),
    (4, N'Magrider', 2, N'Hover Tank', N'The Magrider is a hover tank used exclusively by the Vanu Sovereignty.  Its hover capabilities allow it ability strafe and climb more types of terrain beyond other tanks.', 0, 4, 3987),
    (5, N'Vanguard', 5, N'Four Wheeled Ground Vehicle', N'The Vanguard is a sturdy tank used exclusively by the New Conglomerate.  The vanguard favors heavier front armor compared to other tanks, but this comes at the cost of speed.', 0, 4, 4005),
    (6, N'Prowler', 5, N'Four Wheeled Ground Vehicle', N'The Prowler is a versatile tank used exclusively by the Terran Republic.  It boasts a higher top speed and its twin barrel cannons are capable of inflicting damage quicker than the other tanks.', 450, 4, 3993),
    (7, N'Scythe', 1, N'Light Aircraft', N'The Scythe is a one man air superiority fighter in use exclusively by the Vanu Sovereignty.  Vanu technology allows the Scythe to be a more stable aircraft when hovering and decelerating.', 0, 4, 3999),
    (8, N'Reaver', 1, N'Light Aircraft', N'The Reaver is a one man air superiority fighter in use exclusively by the New Conglomerate.  Its powerful engines combined with its great afterburner capacity make this aircraft the fastest empire specific fighter in the skies.', 0, 4, 3996),
    (9, N'Mosquito', 1, N'Light Aircraft', N'The Mosquito is a one man air superiority fighter in use exclusively by the Terran Republic.  Decades of refinement have made this aircraft the quickest and most agile of all the empire-specific aircrafts.', 0, 4, 3990),
    (10, N'Liberator', 1, N'Light Aircraft', N'The Liberator is a three man gun ship.  The bombardier weapon is very powerful against ground targets and it has a tail gun to ward off enemy fighters.', 0, 4, 3981),
    (11, N'Galaxy', 1, N'Light Aircraft', N'The Galaxy is an air transport that can be used to hot drop up to 11 soldiers.  It has four weapon mounts that can be used for defense or outfitted into a gunship role.', 0, 4, 3978),
    (12, N'Harasser', 5, N'Four Wheeled Ground Vehicle', N'The Harasser is an agile 3 man buggy that is available to all empires.  The driver and gunner are both seated inside the cockpit; while the third passenger stands on the rear of the buggy and has access to his own weapons.', 0, 4, 8687),
    (13, N'Drop Pod', 8, N'Drop Pod', N'Released from orbit, Drop Pods are used to rapidly deploy troops to a battlefield.  Soldiers in the drop pod have some control on the pod''s direction and in some cases can use it as an effective weapon, damaging those it lands on.', 9999, 3, 7349),
    (14, N'Valkyrie', 1, N'Light Aircraft', N'The Valkyrie is a six person aircraft used for close air support and transportation. It is crewed by a pilot and a forward gunner with room for four passengers that are able to use their personal equipment while in flight.', 0, 4, 79698),
    (15, N'ANT', 5, N'Four Wheeled Ground Vehicle', N'ANT Prototype Description', 0, 4, 84726),
    (100, N'Xiphos Anti-Personnel Phalanx Turret', 7, N'Umovable Turret', N'The Xiphos Anti-Personnel Phalanx Turret sprays 20mm rounds with its twin rotary guns that are damaging to infantry and light armor.', 9999, 3, 10950),
    (101, N'MANA Anti-Personnel Turret', 7, N'Umovable Turret', N'MANA (Manned Aggressor Neutralizing Assault) Anti-Personnel turret is deployed by engineers and is effective against soft and medium armored targets.', 0, 0, 4040),
    (102, N'MANA Anti-Vehicle Turret', 7, N'Umovable Turret', N'MANA (Manned Aggressor Neutralizing Assault) Anti-Vehicle turret is deployed by engineers and is effective against large armored targets.', 0, 0, 8053),
    (103, N'Spitfire Auto-Turret', 7, N'Umovable Turret', N'Spitfire Auto-Turrets are placed by engineers and automatically engage hostile infantry within 50 meters.', 0, 0, 0),
    (104, N'Spitfire Auto-Turret', 7, N'Umovable Turret', N'Spitfire Auto-Turrets are placed by engineers and automatically engage hostile infantry within 50 meters.', 0, 0, 0),
    (105, N'AA SpitFire Turret', 7, N'Umovable Turret', N'Spitfire Auto-Turrets are placed by engineers and automatically engage hostile infantry within 50 meters.', 0, 0, 0),
    (150, N'Aspis Anti-Aircraft Phalanx Turret ', 7, N'Umovable Turret', N'The Aspis Phalanx Turret rapidly fires 20mm shells that create a burst of flak when within proximity of hostile aircraft.', 9999, 3, 4046),
    (151, N'Spear Anti-Vehicle Phalanx Turret', 7, N'Umovable Turret', N'The Spear Anti-Vehicle Phalanx Turret is capable of rapidly firing 120mm AP tank rounds capable of damaging heavily armored targets.', 9999, 3, 4052),
    (160, N'Spear Anti-Vehicle Tower', 7, N'Umovable Turret', N'Deployed Spear Phalanx Turret will not be available until the new placement system is complete', 0, 0, 4052),
    (161, N'Aspis Anti-Aircraft Phalanx Tower', 7, N'Umovable Turret', N'The Aspis Phalanx Tower rapidly fires high velocity anti-aircraft rounds that are effective against lightly armored targets.', 9999, 3, 4046),
    (162, N'Xiphos Anti-Personnel Tower', 7, N'Umovable Turret', N'The Xiphos Anti-Personnel Phalanx Tower sprays 20mm rounds with its twin rotary guns that are damaging to infantry and light armor.', 9999, 1, 10950),
    (163, N'Glaive IPC', 7, N'Umovable Turret', N'The Glaive Ionized Particle Cannon is unmanned mid-range artillery that disrupts defensive shield systems but only inflicts minor damage to personnel and equipment due to its lack of conventional payload. The IPC requires AI module support as well as a manually fired targeting dart to acquire a target.', 0, 0, 4052),
    (1013, N'Recon Drone', 10, N'Observer Camera', N'A remote control aerial drone that can be used to spot enemy forces.', 9999, 3, 13477),
    (2006, N'Spear Anti-Vehicle Phalanx Turret', 7, N'Umovable Turret', NULL, 0, 0, 4052),
    (2007, N'Colossus', 5, N'Four Wheeled Ground Vehicle', N'The Colossus Heavy Tank is a ground-based mobile fortress, with reinforced armor plating and multiple hardpoints for dealing with any threat.', 999, 4, 4005),
    (2008, N'Drop Pod', 8, N'Drop Pod', N'Released from orbit, Drop Pods are used to rapidly deploy troops to a battlefield.  Soldiers in the drop pod have some control on the pod''s direction and in some cases can use it as an effective weapon, damaging those it lands on.', 9999, 3, 7349),
    (2009, N'Xiphos Anti-Personnel Phalanx Turret', 7, N'Umovable Turret', N'The Xiphos Anti-Personnel Phalanx Turret sprays 20mm rounds with its twin rotary guns that are damaging to infantry and light armor.', 9999, 3, 10950),
    (2010, N'Flash', 5, N'Four Wheeled Ground Vehicle', N'<font color="#dc9c36">Single use consumable. Carry up to 2 on the field between resupplies. 40 max.</font><br>The Flash XS-1 is a lightly armored, single-passenger vehicle designed for rapid deployment anywhere on the battlefield.', 50, 4, 3975),
    (2011, N'Forward Station', 7, N'Umovable Turret', NULL, 0, 0, 0),
    (2019, N'Bastion Fleet Carrier', 1, N'Light Aircraft', N'The Bastion is a multi-purpose airship with multiple weapon systems, soldier respawn capabilities and fighter launch pads.', 450, 4, 92694),
    (2021, N'Glaive IPC', 7, N'Umovable Turret', N'The Glaive Ionized Particle Cannon is unmanned mid-range artillery that disrupts defensive shield systems but only inflicts minor damage to personnel and equipment due to its lack of conventional payload. The IPC requires AI module support as well as a manually fired targeting dart to acquire a target.', 0, 0, 4052),
    (2033, N'Javelin', 2, N'Hover Tank', N'An experimental recon vehicle with hover capabilities, allowing it to quickly traverse all types of terrain.', 100, 4, 92337),
    (2036, N'Pumpkin Patch', 7, N'Umovable Turret', NULL, 0, 0, 0),
    (2039, N'Deliverer Prototype', 5, N'Four Wheeled Ground Vehicle', N'A prototype ANT design reconfigured for carrying infantry and MAX units.', 0, 4, 84726),
    (2040, N'Wasp Prototype', 1, N'Light Aircraft', N'This is a prototype configuration of the Valkyrie transport vehicle that can harvest and deposit Cortium while providing close-range anti-vehicle support.', 250, 4, 79698),
    (2122, N'Mosquito Interceptor', 1, N'Light Aircraft', N'The Mosquito is a one man air superiority fighter in use exclusively by the Terran Republic.  Decades of refinement have made this aircraft the quickest and most agile of all the empire-specific aircrafts.', 0, 4, 3990),
    (2123, N'Reaver Interceptor', 1, N'Light Aircraft', N'The Reaver is a one man air superiority fighter in use exclusively by the New Conglomerate.  Its powerful engines combined with its great afterburner capacity make this aircraft the fastest empire specific fighter in the skies.', 0, 4, 3996),
    (2124, N'Scythe Interceptor', 1, N'Light Aircraft', N'The Scythe is a one man air superiority fighter in use exclusively by the Vanu Sovereignty.  Vanu technology allows the Scythe to be a more stable aircraft when hovering and decelerating.', 0, 4, 3999),
    (2125, N'Javelin', 2, N'Hover Tank', N'An experimental recon vehicle with hover capabilities, allowing it to quickly traverse all types of terrain.', 100, 4, 92337);

  MERGE [dbo].[Vehicle] as target
    USING ( SELECT Id, Name, TypeId, TypeName, Description, Cost, CostResourceId, ImageId FROM #Staging_Vehicle ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET Name = source.Name,
                 TypeId = source.TypeId,
                 TypeName = source.TypeName,
                 Description = source.Description,
                 Cost = source.Cost,
                 CostResourceId = source.CostResourceId,
                 ImageId = source.ImageId
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [Name], [TypeId], [TypeName], [Description], [Cost], [CostResourceId], [ImageId] )
      VALUES ( source.Id, source.Name, source.TypeId, source.TypeName, source.Description, source.Cost, source.CostResourceId, source.ImageId );
  
  DROP TABLE #Staging_Vehicle;

END;