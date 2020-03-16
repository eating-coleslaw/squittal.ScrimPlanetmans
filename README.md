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

4. Find the download location and run SqlLocalDb.exe. Go through it with all of the default options.

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

### What to Do

__BuildApp.bat__  
Run this if it's the first time running the app, or you just synced changes from the repository.

After a successful build, you'll be prompted to run the app. While the build itself does not require being run as administrator, if you want to run the app from this prompt you must run BuildApp.bat as administrator.

__RunApp.bat__  
Run this as administrator to start the app. In a web browser navigate to the URL displayed after the `Now listening on: ...` console message (e.g. <https://localhost:5001>).

To stop the app _gracefully_, press `Ctrl+C` in the Command Prompt. Enter `Y` at the `Cancel batch job?` prompt.

## Maintenance

### Update Weapon & Item Data  

New weapons or items added to the game won't automatically be picked up by the app. Instead, stop the app, run `commands\DeleteItemSqlData.bat` as administrator, then restart the app. When the app restarts, these tables will be repopulated with the new weapons and items.

__Before running the delete command__, verify that the census API Item collection is working. The following query should return a count of around 20,000: [http://census.daybreakgames.com/s:example/count/ps2:v2/item](http://census.daybreakgames.com/s:example/count/ps2:v2/item "Daybreak Games Item census collection count query")

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
