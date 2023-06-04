# squittal.ScrimPlanetmans

An app for running, streaming, and collecting data for Planetside 2 scrims.

![Stream infantry matches with the dynamic, real-time overlay](https://raw.githubusercontent.com/eating-coleslaw/squittal.ScrimPlanetmanImages/main/match_overlay_2.gif "Example of the streaming overlay being used in a match")

> Stream infantry matches with the dynamic, real-time overlay

## Table of Contents

- [Requirements / Installation](#requirements)
- [Running the App](#running-the-app)  
- [Match & Team Setup](#match-setup)
- [Maintenance](#maintenance)
- [Streaming Overlay](#streaming-overlay)
- [Reporting](#reporting)
- [Troubleshooting](#troubleshooting)

## Requirements

### .NET Core 3.1 SDK

The .NET Core 3.1 SDK and Runtime (the runtime is included with the SDK) is required to build and run the app. [Download the latest version of the .NET Core 3.1 SDK from the downloads page.](https://dotnet.microsoft.com/en-us/download/dotnet "Download .NET Core 3.0"). Expand the "Out of support versions" section to see the .NET Core 3.1 option.

### SQL Server Express LocalDb

This is the database provider used to store data for the app.

Windows 10 installation:

1. Download and run the installer for the Express edition from [Microsoft's website](https://www.microsoft.com/en-us/sql-server/sql-server-downloads "SQL Server downloads page").

2. The installer will prompt you to choose an installation type. Select the Download Media option.

3. On the following screen, select the LocalDb package and note the download location. Click Download.

4. __Find the download location and run SqlLocalDb.exe.__ Go through it with all of the default options.

Windows 7 & Windows 8.1 installation:

1. Navigate to the Microsoft SQL Server 2014 Express download page [here]([Microsoft® SQL Server® 2014 Express](https://www.microsoft.com/en-US/download/details.aspx?id=42299) "Microsoft SQL Server 2014 Express download page") and cick the Download button.

2. Select `LocalDB 64BIT\SqlLocalDB.msi` or `LocalDB 32BIT\SqlLocalDB.msi` (64BIT for 64-bit OS, 32BIT for 32-bit OS), then click Next to start the download.

3. __Find the download location and run SqlLocalDb.exe.__ Go through it with all of the default options.

### Daybreak Games Service ID

Using a registered Service ID permits unthrottled querying of the Census API. Without one, you're limited to 10 queries per minute (e.g. 10 character lookups). This app greatly exceeds that limit, so you will need to get your own Service ID.

1. Apply for a service ID on the [DBG Census API website](http://census.daybreakgames.com/#devSignup). You'll receive an email notification once your request has been processed (I received mine within a few hours, but your results may vary).

2. Open `Control Panel > System and Security > System > Advanced system settings`. In the __System Properties__ window that opens, select the `Environment Variables...` button on the Advanced tab. The __Environment Variables__ window will open.

3. Under __User veriables for <*Windows username*>__, select `New..`. to add a new user variable with name "DaybreakGamesServiceKey" (without the quotes) and a value of your census API service key. Click OK to accept the new variable.  
   * Note: The "s:" prefix in an API query string is not part of the service key. For example, you would set the environment variable to `example`, not `s:example`.  

### For Streaming - Create Trusted Dev Certificate

OBS won't show the contents of a webpage if it doesn't have a trusted certificate. You can fix this by generating a self-signed, trusted certificate to enable HTTPS for the localhost site used by the app. For more information, refer to Microsoft's [dotnet dev-certs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs) guide.

_Note: These steps replace the old "install Visual Studio" instructions._

1. Open a new Command Prompt window.

2. Optionally, enter `dotnet dev-certs https --check --trust` to see if a trusted certificate is already present on your local machine. If it says that one was found, then you should be able to skip to step 4.

3. Enter `dotnet dev-certs https --trust` to create a certificate and trust it.

4. Open OBS and configure a browser source with a URL of https://localhost:5001/overlay, following the instructions in the [Streaming Overlay](#streaming-overlay) section below. Disable & re-enable the browser source to refresh it. After enabling, it might take a second or two for the site to render.

## Running the App

The `commands` folder contains files for starting the app and managing various related services.

Several of these must be run as administrator (`Right Click > Run as administrator`) to work correctly. For the files you'll run more often, I'd suggest creating a shortcut set to always run as administrator:  

  1. __Create the shortcut__  
     `Right Click on .bat file > Create shortcut`. You can put move shortcut wherever you'd like.

  2. __Set the shortcut to run as administrator__  
     `Right Click on the shortcut > Shortcut tab > Click the Advanced... button at the bottom > Check the _Run as administrator_ box`.

### Getting Started

1. `BuildApp.bat`  
Run this if it's the first time running the app, or you just synced changes from the repository.  
 After a successful build, you'll be prompted to run the app. While the build itself does not require being run as administrator, if you want to run the app from this prompt you must run BuildApp.bat as administrator.

2. `RunApp.bat`  
Run this as administrator to start the app. In a web browser navigate to the URL displayed after the `Now listening on: ...` console message (e.g. <https://localhost:5001>).  
To stop the app _gracefully_, press `Ctrl+C` in the Command Prompt. Enter `Y` at the `Cancel batch job?` prompt.

#### Streaming Steps

__Review Database Population__  
The app uses census data stored in a local database for a variety of purposes, the most important of which is scoring. On first startup, the database automatically attempts to populate from the Census API. If that fails, it backup scripts are used to populate the database. It's crucial to verify that the database is properly set up before trying to run a match.

1. On the Database Maintenance page (`/DbAdmin`), everything in the Database Count column should have a non-zero value. If that's not the case, refer to the Maintenance section below for steps to correct a collection.

2. On the Rulesets page (`Rulesets`), there should be an Item Category Rules table and a Scrim Action Rules table, both populated with values. If the Item Category Rules table is empty, verify that the Item Category database collection is populated. If it's empty, populate it from the Census or Backup, then restart the app.

### Match Setup

Configure and monitor scrim matches from the Match Setup page (`/Admin`). It's divided into three sections: Team 1, Team 2, Match Controls.

![The match and team setup page](https://raw.githubusercontent.com/eating-coleslaw/squittal.ScrimPlanetmanImages/main/match_and_team_setup_1.png "An example of the match and team setup page, with multiple outfits added to each team")

> Enjoy fine control over how matches and teams are configured

#### Team Setup

In the top box for each team, you can:

* Set the display alias for the team. It will default to the alias of the first outfit added to the team, but can be overriden. Press the `Update` button to save your changes.

* Add outfits (yes, multiple per team if you want!) to the team via their outfit alias (case-insensitive). Only one instance of an outfit can be in a match.

* Add individual players to the team via their character name or character ID. Only one instance of a player can be in a match. You can't add a player as an individual if their outfit is already in the match, but you can add a player and then later add their outfit to the match (the player will remain as "outfitless" in terms of match configuration).

* Toggle showing removal controls for the team. The removal controls let you remove entire outfits or specific players from the team. The Show Removal Controls check box is only visible if there are any players on the team (either in outfits or as outfitless). By default, this is set to false - hide removal controls.

Each outfit added gets a card in their team's column, listing all of their players. All indivually added players are grouped in the team's Other Players card.

## Maintenance

### Update Local Data

The app uses census data stored in a local database for a variety of purposes. This means that new weapons, items, bases, etc. added to the game won't automatically be picked up. The Database Maintenance page (`/DbAdmin`) is your tool for keeping your local database up to date.

#### The Database Maintenance Table

| Column | Description |
|--------|-------------|
| Collection | The name of the data set, generally aligning with the Census API terminology |
| Database Count | The number of entities for this collection currently in your local database. Should not be zero, and should match the Census Count value (with some exceptions __\*__). Click the refresh icon to recalculate the count |
| Census Count | The number of entities for this collection returned by the Census API. If zero or a warning icon, then this collection is broken in the Census API. Click the refresh icon to query the Census API for an updated count |
| Update from Census | Click this button to populate the local database with new values from the Census API and update existing values with data from the Census API. If the Census API is broken or returning no results for a collection, the local database will not be truncated or otherwise harmed |
| Update from Backup | Click this button to populate the local database from a hard-coded set of values for the collection. The database collection will be emptied before it's re-populated |

__\*__ The Loadouts collection has more database entities than in the Census API because the Census API is missing the NSO classes.

## Streaming Overlay

![The real-time, detailed match report](https://raw.githubusercontent.com/eating-coleslaw/squittal.ScrimPlanetmanImages/main/half_time_report_1.png "An example of the real-time, detailed match report, with the HSR column hidden")

> An example of the real-time, detailed match report, with the HSR column hidden

### OBS Setup

Each view of the streaming overlay should be a scene with a Browser source configured like this:

| Browser Source Setup | |
|-|-|
| _URL_                                       | https://localhost:5001/overlay... |
| _Control audio via OBS_                     | Checked                         |
| _Custom CSS_                                | Empty                           |
| _Shutdown source when not visible_          | Checked                         |
| _Refresh browser when scene becomes active_ | Checked                         |

### Customizing the Overlay

The overlay page accepts 5 URL parameters which control whether different overlay components are visible or hidden. By default, all overlay components are visible.

The general formula for using the parameters is `.../overlay?param1=value1&param2=value2&param3=value3...`, where `param#` is an entry from the below table and `value#` is either `true` (show component) or `false` (hide component).

| URL Parameters | Controls... |
|-|-|
| players    | List of players on the left & right side of the page |
| report     | The match stats report in the center of the page     |
| scoreboard | The scoreboard at the top center of the page. Disabling the scoreboard also hides the killfeed. |
| feed       | The killfeed in the center, below the scoreboard. The killfeed is always hidden if the scoreboard is hidden. |
| title      | The match title at the top center of the page        |
| reportHsr  | The HSR column in the match report                   |
| analytic    | Set to true to switch the match stats report to a more detailed version, similar to the Analytic Match Reports (see below)|). Note that the _players_ status components are automatically hidden when this setting is enabled. The data in the report is refreshed on page load and then every 5 seconds. |
| currentRound | Enable to show stats for the current round instead of the match totals. Currently supported by the scoreboard, player status boxes, and analytic report. The real-time report doesn't support this parameter. Omit or set this parameter to false to show match totals (the original/default behavior). |

#### Examples

https://localhost:5001/Overlay?players=false&feed=false

* Hides the players and the killfeed, for a typical half-time or match-end scene.

https://localhost:5001/Overlay?players=false&feed=false&reportHsr=false

* Same as above, except that the HSR column is excluded from the match report.

https://localhost:5001/Overlay?report=false

* Hides the match stats report. This is what you'd want for streaming a match in-progress.

https://localhost:5001/Overlay?feed=false&analytic=true&currendRound=true

* Hides the feed and shows the "fancy" stats report for the current round. This is what you'd want to use after the first round of a match that's using a Conquest-like ruleset.

https://localhost:5001/Overlay?report=false&currentRound=true

* Hides the match report and shows stats for the current round. This is what you'd want to use for streaming an in-progress match that's using a Conquest-like ruleset.

## Reporting

Results from matches, along with several types of events, are stored in the same database as the census data. Feel free to use it for your own reporting needs.

### Analytic Match Reports

Analytic match reports containing infantry-centric metrics are available for every match. Access them from the Reports Browser page (`/Reports`). Find specific matches using the Report Browser's filter controls.

![An analytic match report](https://raw.githubusercontent.com/eating-coleslaw/squittal.ScrimPlanetmanImages/main/analytic_report_1.png "An example of an analytic match report, with the player details panel opened for a player")

> An analytic match report

### Prerequisites

#### Recommended Software

You can use one of the following programs to connect to and query against the database:  
| Program | Info | Platforms | Download |
|-|-|-|-|
| SQL Server Management Studio (SSMS) | Very robust | Windows 8.1/10* | [_download_](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15 "Download SQL Server Managament Studio") |
| Azure Data Studio | More lightweight, fewer features | Windows, macOS, Linux | [_download_](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver15 "Download Azure Data Studio") |

__\*__ See download page for complete list of supported platforms

#### Connect to the Database

|Connection Details | |
|-|-|
| _Connection Type_ | Microsoft SQL Server    |
| _Server_          | (LocalDB)\MSSQLLocalDB  |
| _Authentication_  | Windows Authentication* |
| _Database_        | PlanetmansDbContext     |

  __\*__ I don't know what this Authentication method will be on macOS or Linux, but you shouldn't need to enter a username of password.

### Data Model

__Disclaimer_: I can't guaruntee that the data model information below is completely up-to-date or accurate. Connecting to your database directly is the best way to investigate the data model._

#### ScrimMatch

Each match has one entry in this table, created when you press "Start Match" in Match Setup.

| Column    | Data Type     | Nullable? | Details |
|-----------|---------------|-----------|---------|
| Id        | nvarchar(450) | No        | [__Primary Key__] The match's identifier. This is the same value as the match log filename for the match. Join to the `ScrimMatchId` column in various other tables. |
| StartTime | datetime2(7)  | No        | The timestamp of when "Start Match" was pressed. |
| Title     | nvarchar(max) | Yes       | The most-recent Match Title saved. |

#### ScrimMatchTeamResult

Each team in each match has one row in this table containing a team's final score and other stats for a given match. Typically, rows will first be added at the end of the first round in a match, and then updated at the end of subsequent matches (even if a round is stopped manually). However, adding/removing point adjustments, outfits, or players can also result in a team's match results getting updated.

| Column                | Data Type     | Nullable? | Details |
|-----------------------|---------------|-----------|---------|
| ScrimMatchId          | nvarchar(450) | No        | [__Primary Key__] The match's identifier. |
| TeamOrdinal           | int           | No        | [__Primary Key__] The team's identifier (e.g. 1-first team, 2-second team). |
| Points                | int           | No        | Team's total score for the match. |
| NetScore              | int           | No        | Team's net score for the match. |
| Kills                 | int           | No        | Total number of times the team's players killed someone on another team. |
| Deaths                | int           | No        | Total number of times the team's players died to any match participant or by suicide. |
| Headshots             | int           | No        | Total number of times the team's players killed someone on another team with a headshot. |
| HeadshotDeaths        | int           | No        | Total number of times the team's players died by a headshot from a player on another team. |
| Suicides              | int           | No        | Total number of times the team's players killed themselves. |
| Teamkills             | int           | No        | Total number of times the team's players killed a player on their team. |
| TeamkillDeaths        | int           | No        | Total number of times the team's players were killed by a player on their team. |
| RevivesGiven          | int           | No        | Total number of times the team's players gave a revive to a player on their team. Only accepted revives are counted. |
| RevivesTaken          | int           | No        | Total number of times the team's players accepted a revive from a player on their team. |
| DamageAssists         | int           | No        | Total number of times the team's players received an experience tick with one of the following experience gain IDs: <br>2-Kill Player Assist<br>371-Kill Player Priority Assist<br>372Kill Player High Priority Assist |
| UtilityAssists        | int           | No        | Total number of times the team's players received an experience tick with one of the following experience gain IDs: <br>5-Heal Assis<br>335-Savior Kill (Non MAX)<br>438-Shield Repair<br>439-Squad Shield Repair<br>550-Concussion Grenade Assist<br>551-Concussion Grenade Squad Assist<br>552-EMP Grenade Assist<br>553-EMP Grenade Squad Assist<br>554-Flashbang Assist<br>555-Flashbang Squad Assist<br>1393-Hardlight Cover - Blocking Exp<br>1394-Draw Fire Award<br>36-Spot Kill<br>54-Squad Spot Kill |
| DamageAssistedDeaths  | int           | No        | Total number of times the team's players had a death assisted by a Damage Assist. A player is considered such a victim if they appear in the `other_id` field of a relevant experience gain stream payload. |
| UtilityAssistedDeaths | int           | No        | Total number of times the team's players ad a death assisted by a Utility Assist. A player is considered such a victim if they appear in the `other_id` field of a relevant experience gain stream payload. |
| ObjectiveCaptureTicks | int           | No        | Total number of times the team's players receive an experience tick with one of the following experience gain IDs: <br>16-Control Point - Attack <br>272-Convert Capture Point <br>557-Objective Pulse Capture |
| ObjectiveDefenseTicks | int           | No        | Total number of times the team's players receive an experience tick with one of the following experience gain IDs: <br>15-Control Point - Defend <br>556-Objective Pulse Defend |
| BaseDefenses          | int           | No        | Total number of times the team "capture" the target facility via a defense. This is based on infantry scrim rules (timer starts at half, one team's faction owns the facility), not on base defenses as reported in game. |
| BaseCaptures          | int           | No        | Total number of times the team "capture" the target facility via a capture. This is based on infantry scrim rules (timer starts at half, one team's faction owns the facility), not on base captures as reported in game. |

#### ScrimMatchTeamPointAdjustment

Each point adjustment made to a team's score during a match has a row in this table. The table is updated each time a point adjustment is added or removed in Match Setup. The points in this table have already been factored into a team's Points and Net Score in the `ScrimMatchTeamResult` table.

| Column         | Data Type     | Nullable? | Details |
|----------------|---------------|-----------|---------|
| ScrimMatchId   | nvarchar(450) | No        | [__Primary Key__] The match's identifier. |
| TeamOrdinal    | int           | No        | [__Primary Key__] The team's identifier (e.g. 1-first team, 2-second team). |
| Timestamp      | datetime2(7)  | No        | [__Primary Key__] The timestamp of when the point adjustment was added. |
| Points         | int           | No        | The value of the point adjustment. |
| AdjustmentType | int           | No        | Whether the adjustment was a deduction (points subtracted) or granting (points added) of points. 0-Deduction, 1-Granting.  |
| Rationale      | nvarchar(max) | Yes       | The reason provided for the point adjustment. |

#### ScrimDeath

Each row in this table corresponds to a Death payload from the census stream in which all characters were match participants (i.e. the Death wasn't outside interference). In other words, each player death during a match has a row in this table.

| Column               | Data Type     | Nullable? | Details |
|----------------------|---------------|-----------|---------|
| ScrimMatchId         | nvarchar(450) | No  | [__Primary Key__] The ID of the match in which the death event occured. |
| Timestamp            | datetime2(7)  | No  | [__Primary Key__] The timestamp when the event occured. |
| AttackerCharacterId  | nvarchar(450) | No  | [__Primary Key__] The character ID of the player who killed the Victim player. For suicides, this will be the same as the VictimCharacterId.  |
| VictimCharacterId    | nvarchar(450) | No  | [__Primary Key__] The character ID of the player who died. For suicides, this will be the same as the AttackerCharacterId. |
| ScrimMatchRound      | int           | No  | The round in which the death event occured. |
| ActionType           | int           | No  | The Scrim Action describing this death event (e.g. 101-InfantryKillMax, 102-InfantryTeamkillInfantry). Join to `ScrimAction.Action`. |
| DeathType            | int           | No  | The Death Type describing this death event. Either 0-Kill, 1-Teamkill, or 2-Suicide. Teamkills are based first on the players' Team Ordinals, then on their Faction. Join to `DeathType.Type`. |
| AttackerTeamOrdinal  | int           | No  | Integer corresponding to the attacking player's team. 1 for the first team, 2 for the second team, etc. |
| VictimTeamOrdinal    | int           | No  | Integer corresponding to the victim player's team. 1 for the first team, 2 for the second team, etc. |
| AttackerNameFull     | nvarchar(max) | Yes | Name of the attacking player as it appears in game. |
| AttackerFactionId    | int           | No  | Faction ID of the attacking player. Either 1-VS, 2-NC, 3-TR, 4-NSO. Join to `Faction.Id`. |
| AttackerLoadoutId    | int           | Yes | ID of the PS2 class the attacking player was playing when they killed the victim player. Join to `Loadout.Id`.  |
| AttackerOutfitId     | nvarchar(max) | Yes | ID of the outfit the attacking player is associated with for the death event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| AttackerOutfitAlias  | nvarchar(max) | Yes | The alias (aka tag) of the outfit the attacking player is associated with for the death event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| AttackerIsOutfitless | bit           | No  | Whether the attacking player is associated with an outfit for this death event. 1-Yes if they are, 2-No if they are not (if they were listed under "Other Players"). |
| VictimNameFull       | nvarchar(max) | Yes | Name of the victim player as it appears in game. |
| VictimFactionId      | int           | No  | Faction ID of the victim player. Either 1-VS, 2-NC, 3-TR, 4-NSO. Join to `Faction.Id`. |
| VictimLoadoutId      | int           | Yes | ID of the PS2 class the victim player was playing when they died. Join to `Loadout.Id`. |
| VictimOutfitId       | nvarchar(max) | Yes | ID of the outfit the victim player is associated with for the death event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| VictimOutfitAlias    | nvarchar(max) | Yes | The alias (aka tag) of the outfit the victim player is associated with for the death event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| VictimIsOutfitless   | bit           | No  | Whether the victim player is associated with an outfit for this death event. 1-Yes if they are, 0-No if they are not (if they were listed under "Other Players"). |
| IsHeadshot           | bit           | No  | Whether the victim player was killed by a headshot (1-Headshot, 2-Not Headshot). Events with Death Type 1-Teamkill and 2-Suicie are never counted as headshots, regardless of whether a headshot actually occured. |
| WeaponId             | int           | Yes | Item ID of the weapon to which the victim player died. Join to `Item.Id`. |
| WeaponItemCategoryId | int           | Yes | ID of the item category to which the weapon that the victim player died to belongs. Join to `ItemCategory.Id`.|
| IsVehicleWeapon      | bit           | Yes | Whether the weapon to which the victim player died is a vehicle weapon. |
| AttackerVehicleId    | int           | Yes | ID of the vehicle by which the victim player was killed. Join to `Vehicle.Id`. |
| WorldId              | int           | No  | ID of the PS2 server on which the death event occured. Join to `World.Id`. |
| ZoneId               | int           | No  | ID of the PS2 continent on which the death event occured. Join to `Zone.Id` |
| Points               | int           | No  | The points the attacking player received, or had deducted, for the death event.  |

#### ScrimVehicleDestruction

Each row in this table corresponds to a VehicleDestroy payload from the census stream in which all characters were match participants (i.e. the Vehicle Destruction wasn't outside interference). In other words, the table has a row for each time a vehicle is destroyed during a match. Only destructions of vehicles owned by a match participant are included in the table (e.g. a match participant destroying an unowned flash will not result in a new row in the table).

| Column               | Data Type     | Nullable? | Details |
|----------------------|---------------|-----------|---------|
| ScrimMatchId         | nvarchar(450) | No  | [__Primary Key__] The ID of the match in which the vehicle destruction event occured. |
| Timestamp            | datetime2(7)  | No  | [__Primary Key__] The timestamp when the event occured. |
| AttackerCharacterId  | nvarchar(450) | No  | [__Primary Key__] The character ID of the player who destroyed the victim vehicle. For suicides, this will be the same as the VictimCharacterId.  |
| VictimCharacterId    | nvarchar(450) | No  | [__Primary Key__] The character ID of the player who owned the victim vehicle. For suicides, this will be the same as the AttackerCharacterId. |
| VictimVehicleId      | int           | No  | [__Primary Key__] The ID of the vehicle that was destroyed. Join to `Vehicle.Id`.|
| AttackerVehicleId |  | int           | Yes | The ID of the vehicle by which the victim vehicle was destroyed, if it was destroyed by a vehicle. |
| ScrimMatchRound      | int           | No  | The round in which the vehicle destruction event occured. |
| ActionType           | int           | No  | The Scrim Action describing this vehicle destruction event (e.g. 109-InfantryDestoryEsf, 426-VehicleDestroyLightning). Join to `ScrimAction.Action`. |
| DeathType            | int           | No  | The Death Type describing this vehicle destruction event. Either 0-Kill, 1-Teamkill, or 2-Suicide. Teamkills are based first on the players' Team Ordinals, then on their Factions. Join to `DeathType.Type`. |
| AttackerTeamOrdinal  | int           | No  | Integer corresponding to the attacking player's team. 1 for the first team, 2 for the second team, etc. |
| VictimTeamOrdinal    | int           | No  | Integer corresponding to the victim player's team. 1 for the first team, 2 for the second team, etc. |
| AttackerVehicleClass | int           | Yes | The type of vehicle the attacking player used to destroy the victim vehicle (e.g. 4-Sunderer, 8-ESF). Join to `VehicleClass.Class`. |
| VictimVehicleClass   | int           | Yes | The type of vehicle that was destroyed for the vehicle destruction event. (e.g. 4-Sunderer, 8-ESF). Join to `VehicleClass.Class`. |
| AttackerNameFull     | nvarchar(max) | Yes | Name of the attacking player as it appears in game. |
| AttackerFactionId    | int           | No  | Faction ID of the attacking player. Either 1-VS, 2-NC, 3-TR, 4-NSO. Join to `Faction.Id`. |
| AttackerLoadoutId    | int           | Yes | ID of the PS2 class the attacking player was playing when they destroyed the victim vehicle. Join to `Loadout.Id`.  |
| AttackerOutfitId     | nvarchar(max) | Yes | ID of the outfit the attacking player is associated with for the vehicle destruction event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| AttackerOutfitAlias  | nvarchar(max) | Yes | The alias (aka tag) of the outfit the attacking player is associated with for the vehicle destruction event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| AttackerIsOutfitless | bit           | No  | Whether the attacking player is associated with an outfit for this vehicle destruction event. 1-Yes if they are, 2-No if they are not (if they were listed under "Other Players"). |
| VictimNameFull       | nvarchar(max) | Yes | Name of the victim player as it appears in game. |
| VictimFactionId      | int           | No  | Faction ID of the victim player. Either 1-VS, 2-NC, 3-TR, 4-NSO. Join to `Faction.Id`. |
| VictimLoadoutId      | int           | Yes | ID of the PS2 class the victim player was last known to be playing as when their vehicle was destroyed. Join to `Loadout.Id`. |
| VictimOutfitId       | nvarchar(max) | Yes | ID of the outfit the victim player is associated with for the vehicle destruction event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| VictimOutfitAlias    | nvarchar(max) | Yes | The alias (aka tag) of the outfit the victim player is associated with for the vehicle destruction event. This may be populated even if the player was not added via an outfit (i.e. if they were listed under "Other Players"). |
| VictimIsOutfitless   | bit           | No  | Whether the victim player is associated with an outfit for this vehicle destruction event. 1-Yes if they are, 0-No if they are not (if they were listed under "Other Players"). |
| WeaponId             | int           | Yes | Item ID of the weapon by which the victim vehicle was destroyed. Join to `Item.Id`. |
| WeaponItemCategoryId | int           | Yes | ID of the item category to which the weapon that destroyed the victim vehicle belongs. Join to `ItemCategory.Id`.|
| IsVehicleWeapon      | bit           | Yes | Whether the weapon that destroyed the victim vehicle is a vehicle weapon. |
| WorldId              | int           | No  | ID of the PS2 server on which the vehicle destruction event occured. Join to `World.Id`. |
| ZoneId               | int           | No  | ID of the PS2 continent on which the vehicle destruction event occured. Join to `Zone.Id` |
| Points               | int           | No  | The points the attacking player received, or had deducted, for the vehicle destruction event.  |


## Troubleshooting

If you don't see your issue below, please write up an Issue.

### Failed to Connect: Service ID Not Registered

When attempting to run the app, you see error messages like the following:  

    fail: squittal.ScrimPlanetmans.CensusStream.WebsocketMonitor[91435]
      Failed to establish initial connection to Census. Will not attempt to reconnect.
     System.Net.WebSockets.WebSocketException (0x80004005): The server returned status code '403'when status code '101' was expected.

    ...

    Unhandled exception. DaybreakGames.Census.Exceptions.CensusServerException: Provided Service ID is not registered.  A valid Service ID is required for continued api use. (http://census.daybreakgames.com/#devSignup)

Below are the likely causes of this and how to address them.

#### Your Service Key is Not Valid

Using your service key in place of `example`, open the following query in a browser: [http://census.daybreakgames.com/s:example/count/ps2:v2/item](http://census.daybreakgames.com/s:example/count/ps2:v2/item "Daybreak Games Item census collection count query"). If the result is a message like `{"error":"Provided Service ID is not registered.  A valid Service ID is required for continued api use. (http://census.daybreakgames.com/#devSignup)"}`, instead of a count, then your service key is not valid.

* If you haven't yet received an email from Daybreak Games confirming the activation of your service key, then wait until you've received the confirmation email.
* If your service key has worked in the past, then Daybreak Games may have deactivated it for some reason and you will likely need to [follow up with support](http://census.daybreakgames.com/#service-id] "Daybreak Games census API service key information").  

#### Environment Variable Entry Issue

If your service key is definitely valid, then the problem is probably in your environment variable configuration.

* The `DaybreakGamesServiceKey` variable should be in the section labeled `User variables for <your Windows username>`, not under the section labeled `System variables`.
* The service key value should *not* include `s:`.


## Credits & Technologies

This is a project for me to continue learning C# & .NET, and to improve upon the JS + Node.js scrim streaming overlay.

* Backend is largely straight from Lampjaw's  [Voidwell.com](https://github.com/voidwell/Voidwell.DaybreakGames "Voidwell's backend github repository"), with some small modifications by me.
* Interacting with the Daybreak Games Census API and event streaming service are done with Lampjaw's [DaybreakGames.Census NuGet package](https://github.com/Lampjaw/DaybreakGames.Census "DaybreakGames.Census package github repository").
* Frontend/Client is ASP.NET Core Blazor
