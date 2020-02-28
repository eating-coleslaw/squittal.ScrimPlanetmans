@echo off

echo Starting SQL Server service...
call :StartSqlService
if %ERRORLEVEL% EQU 2 (
    echo Failed to start SQL Server service. Try re-running this file as administrator.
    echo Exiting...
    pause
    exit
)
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start SQL Server service. Exiting...
    pause
    exit
)

echo Starting squittal.LivePlanetmans app...
call :StartDotnetApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start the app. Exiting...
    pause
    exit
)
pause


:StartSqlService
SC query MSSQL$SQLEXPRESS | FIND "STATE" | FIND "RUNNING" 2>&1 > nul
if errorlevel 1 (net start MSSQL$SQLEXPRESS) else (echo The SQL Server service is already running... & echo.)
exit /B

:StartDotnetApp
cd "%~dp0"
dotnet run --verbosity m --project "..\squittal.ScrimPlanetmans.App\squittal.ScrimPlanetmans.App.csproj"
exit /B