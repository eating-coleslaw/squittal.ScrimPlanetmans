@echo off

echo Building the .NET app...
call :BuildDotnetApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to build the app. Exiting...
    pause
    exit
)
call :OnSuccessfulBuild
pause
exit


:OnSuccessfulBuild
choice /M "Build completed successfully. Start the app now?" /T 30 /D N
if %ERRORLEVEL% EQU 1 (
    echo Starting the app...
    cd "%~dp0"
    call "RunApp.bat"
    exit
)
if %ERRORLEVEL% EQU 2 (
    echo Did not start the app. Run RunApp.bat as administrator to start the app.
    pause
    exit
)

:BuildDotnetApp
cd "%~dp0"
dotnet build "..\squittal.ScrimPlanetmans.sln" --verbosity m --configuration Release
exit /B