@echo off

cd "%~dp0"
cd "../squittal.ScrimPlanetmans.App/bin/Release/netcoreapp3.1/publish/"
set appDir=%cd%
echo Site content directory: %appDir%

echo Deleting existing squittal.ScrimPlanetmans site...
call :DeleteSite
REM if %ERRORLEVEL% NEQ 0 (
REM     echo Failed to register the site.
REM     pause
REM     exit
REM )
REM pause
REM exit

echo Registering squittal.ScrimPlanetmans site...
call :RegisterSite
if %ERRORLEVEL% NEQ 0 (
    echo Failed to register the site.
    pause
    exit
)
pause
exit

:DeleteSite
pushd "%PROGRAMFILES%\IIS Express"
appcmd delete site squittal.ScrimPlanetmans
popd
exit /B

:RegisterSite
pushd "%PROGRAMFILES%\IIS Express"

appcmd add site /name:squittal.ScrimPlanetmans /id:44346 /bindings:"http/*:44346:localhost" /physicalPath:"%appDir%"
appcmd set site /site.name:squittal.ScrimPlanetmans /[path='/'].applicationPool:Clr4IntegratedAppPool
appcmd set site /site.name:squittal.ScrimPlanetmans /+bindings.[protocol='https',bindingInformation='*:44347:localhost']

popd
exit /B