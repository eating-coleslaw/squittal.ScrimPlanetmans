@echo off

cd "%~dp0"
cd "../squittal.ScrimPlanetmans.App/"
set appDir=%cd%
echo Site content directory: %appDir%

echo Registering squittal.ScrimPlanetmans site...
call :RegisterSite
if %ERRORLEVEL% NEQ 0 (
    echo Failed to register the site.
    pause
    exit
)
pause

:RegisterSite
pushd "%PROGRAMFILES%\IIS Express"

appcmd add site /name:squittal.ScrimPlanetmans /id:44345 /bindings:"http/*:5000:localhost" /physicalPath:"%appDir%"
appcmd set site /site.name:squittal.ScrimPlanetmans /[path='/'].applicationPool:Clr4IntegratedAppPool
appcmd set site /site.name:squittal.ScrimPlanetmans /+bindings.[protocol='https',bindingInformation='*:5001:localhost']

popd
exit /B