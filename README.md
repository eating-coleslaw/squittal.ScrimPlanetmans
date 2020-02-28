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

3. Under __User veriables for *username*__, select `New..`. to add a new user variable with name "DaybreakGamesServiceKey" (without the quotes) and a value of your census API service key. Click OK to accept the new variable.

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

__StopSqlServer.bat__  
Run this as administrator to stop the SQL Server service. You should do this after stopping the app.

__StartSqlServer.bat__  
Run this as administrator to start the SQL Server service. You shouldn't ever need to run this file if you're only running the app (i.e. not doing development), as RuntBat.bat automatically starts the SQL Server service if it's not already running.

## Maintenance

__Update Weapon & Item Data__  
New weapons or items added to the game won't automatically be picked up by the app. Instead, stop the app, run `commands\DeleteItemSqlData.bat` as administrator, then restart the app. When the app restarts, these tables will be repopulated with the new weapons and items.

## Troubleshooting

If you don't see your issue below, please write up an Issue.

### SQL Server Instance Not Found or Not Accessible

When attempting to run the app, you get an error message like this:  
`An error occured initializing the DB.
Microsoft.Data.SqlClient.SqlException (0x80131904): A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: SQL Network Interfaces, error: 26 - Error Locating Server/Instance Specified)`

This means that the SQL Database service has been stopped for some reason. Manually start the service, then try running the app again.

#### Start Service via StopSqlServer.bat

1. `Run squittal.LivePlanetmans\commands\StartSqlServer.bat` (or your `StopSqlServer.bat` shortcut) as administrator.

#### Start Service via Services App

1. Open the __Services__ Windows app.

2. Scroll down to __SQL Server (SQLEXPRESS)__. If the service has stopped, the Status column value will be blank.

3. Select Start from the row's right-click menu to restart the service.

### SQL Server Using Excessive Resources

The SQL Server Windows NT process started suddenly taking up a large amount of CPU, Memory, or Disk resources even though you're not currently running the app.

The SQL database & associated service are independant of the leaderboard app itself, and so they'll continue to run after the app is stopped. Manually stop the SQL Server service after closing the app, and restart it before running the app again.

#### Stop Service via StopSqlServer.bat

1. `Run squittal.LivePlanetmans\commands\StopSqlServer.bat` (or your `StopSqlServer.bat` shortcut) as administrator.

#### Stop Service via Services App

1. Open the __Services__ Windows app.

2. Scroll down to __SQL Server (SQLEXPRESS)__. If the service is running, it will show _Running_ in the Status column.

3. Select `Stop` from the row's right-click menu to stop the service.

#### Stop Service via Task Manager

1. Ending the _SQL Server Windows NT_ process in __Task Manager__ will stop the appropriate services, freeing up system resources.

#### Set Service to Manual Startup

1. If the __SQL Server (SQLEXPRESS)__ service is set to start with your computer, it will show _Automatic_ under the Startup Type column. If you don't want this behavior, right-click and select `Properties`.

2. Set Startup type to _Manual_.

   You will need to ensure the service is running each time before starting the leaderboard app: select `Start` from the right-click menu.

## Credits & Technologies

This is a project for me to learn C# & .NET, re-learn OOP, and practice designing reporting business logic and dashboard UI.

* Backend is largely straight from Lampjaw's  [Voidwell.com](https://github.com/voidwell/Voidwell.DaybreakGames "Voidwell's backend github repository"), with some small modifications by me. Interacting with the Daybreak Games Census API and event streaming service are done with Lampjaw's [DaybreakGames.Census NuGet package](https://github.com/Lampjaw/DaybreakGames.Census "DaybreakGames.Census package github repository").
* Frontend/Client is ASP.NET Core Blazor
