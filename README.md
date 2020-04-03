# squittal.ScrimPlanetmans

 Scoring script & overlay for Planetside 2 scrims

## Requirements

### .NET Core 3.1 SDK

The .NET Core 3.1 SDK and Runtime (the runtime is included with the SDK) is required to build and run the app. [Download the latest version of the .NET Core 3.1 SDK from the downloads page.](https://dotnet.microsoft.com/download "Download .NET Core 3.0")

### SQL Server 2019 Express LocalDb

This is the database provider used to store data for the app.

1. Download and run the installer for the Express edition from [Microsoft's website](https://www.microsoft.com/en-us/sql-server/sql-server-downloads "SQL Server downloads page").

2. The installer will prompt you to choose an installation type. Select the Download Media option.

3. On the following screen, select the LocalDb package and note the download location. Click Download.

4. __Find the download location and run SqlLocalDb.exe.__ Go through it with all of the default options.

### Daybreak Games Service ID

Using a registered Service ID permits unthrottled querying of the Census API. Without one, you're limited to 10 queries per minute (e.g. 10 character lookups). This app greatly exceeds that limit, so you will need to get your own Service ID.

1. Apply for a service ID on the [DBG Census API website](http://census.daybreakgames.com/#devSignup). You'll receive an email notification once your request has been processed (I received mine within a few hours, but your results may vary).

2. Open `Control Panel > System and Security > System > Advanced system settings`. In the __System Properties__ window that opens, select the `Environment Variables...` button on the Advanced tab. The __Environment Variables__ window will open.

3. Under __User veriables for <*Windows username*>__, select `New..`. to add a new user variable with name "DaybreakGamesServiceKey" (without the quotes) and a value of your census API service key. Click OK to accept the new variable.  
   * Note: The "s:" prefix in an API query string is not part of the service key. For example, you would set the environment variable to `example`, not `s:example`.  

### _(For Development Only)_ Visual Studio 2019 (v16.4)

.NET Core 3.1 is only compatible with Visual Studio 2019 (v16.4). Download the free community edition [here](https://visualstudio.microsoft.com/free-developer-offers/ "Visual Studio Preview download page").

## Running the App

The `commands` folder contains files for starting the app and managing various related services.

Several of these must be run as administrator (`Right Click > Run as administrator`) to work correctly. For the files you'll run more often, I'd suggest creating a shortcut set to always run as administrator:  

  1. __Create the shortcut__  
     `Right Click on .bat file > Create shortcut`. You can put move shortcut wherever you'd like.

  2. __Set the shortcut to run as administrator__  
     `Right Click on the shortcut > Shortcut tab > Click the Advanced... button at the bottom > Check the _Run as administrator_ box`.

### Getting Started

__BuildApp.bat__  
Run this if it's the first time running the app, or you just synced changes from the repository.

After a successful build, you'll be prompted to run the app. While the build itself does not require being run as administrator, if you want to run the app from this prompt you must run BuildApp.bat as administrator.

__RunApp.bat__  
Run this as administrator to start the app. In a web browser navigate to the URL displayed after the `Now listening on: ...` console message (e.g. <https://localhost:5001>).

To stop the app _gracefully_, press `Ctrl+C` in the Command Prompt. Enter `Y` at the `Cancel batch job?` prompt.

__Review Database Population__  
The app uses census data stored in a local database for a variety of purposes, the most important of which is <u>scoring</u>. On first startup, the database automatically attempts to populate from the Census API. If that fails, it backup scripts are used to populate the database. It's crucial to verify that the database is properly set up before trying to run a match.

1. On the Database Maintenance page (`/DbAdmin`), everything in the Database Count column should have a non-zero value. If that's not the case, refer to the Maintenance section below for steps to correct a collection.

2. On the Rulesets page (`Rulesets`), there should be an Item Category Rules table and a Scrim Action Rules table, both populated with values. If the Item Category Rules table is empty, verify that the Item Category database collection is populated. If it's empty, populate it from the Census or Backup, then restart the app.

### Match Setup

Configure and monitor scrim matches from the Match Setup page (`/Admin`). It's divided into three sections: Team 1, Team 2, Match Controls.

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
| Database Count | The number of entities for this collection currently in your local database. Should not be zero, and should match the Census Count value (with some exceptions<b>\*</b>). Click the refresh icon to recalculate the count |
| Census Count | The number of entities for this collection returned by the Census API. If zero or a warning icon, then this collection is broken in the Census API. Click the refresh icon to query the Census API for an updated count |
| Update from Census | Click this button to populate the local database with new values from the Census API and update existing values with data from the Census API. If the Census API is broken or returning no results for a collection, the local database will not be truncated or otherwise harmed |
| Update from Backup | Click this button to populate the local database from a hard-coded set of values for the collection. The database collection will be emptied before it's re-populated |

__\*__ The Loadouts collection has more database entities than in the Census API because the Census API is missing the NSO classes.


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
