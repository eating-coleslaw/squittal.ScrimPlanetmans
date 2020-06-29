@echo off

echo Publishing the .NET app...
call :PublishDotnetApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to publish the app. Exiting...
    pause
    exit
)
echo Publish completed successfully
pause
exit

:PublishDotnetApp
cd "%~dp0"
dotnet publish "..\squittal.ScrimPlanetmans.sln" --verbosity m --configuration Release
exit /B