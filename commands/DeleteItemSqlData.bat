@echo off

echo Running TruncateItemTables.sql...
call :RunSqlFile
if %ERRORLEVEL% NEQ 0 (
    echo Failed to run the SQL file. Exiting...
    pause
    exit
)
pause
exit

:RunSqlFile
cd "%~dp0"
sqlcmd -E -S %COMPUTERNAME%\SQLEXPRESS -i "..\squittal.ScrimPlanetmans.App\Data\SQL\TruncateItemTables.sql"
echo.
exit /B