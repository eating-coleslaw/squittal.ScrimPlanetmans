@echo off

echo Starting squittal.ScrimPlanetmans app...
call :StartIisApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start the app. Exiting...
    pause
    exit
)
pause

:StartIisApp
pushd "%PROGRAMFILES%\IIS Express"

iisexpress /site:squittal.ScrimPlanetmans

popd
exit /B