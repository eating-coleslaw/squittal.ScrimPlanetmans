@echo off

cd "%~dp0"
cd "../config/"
set appDir=%cd%
echo Site content directory: %appDir%

echo Starting squittal.ScrimPlanetmans app...
call :StartIisApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start the app. Exiting...
    pause
    exit
)
exit

:StartIisApp
pushd "%PROGRAMFILES%\IIS Express"
iisexpress /site:squittal.ScrimPlanetmans /config:"%appDir%\applicationhost.config"
popd
exit /B