@echo off
pushd "%PROGRAMFILES%\IIS Express"

appcmd delete site squittal.ScrimPlanetmans

popd
pause