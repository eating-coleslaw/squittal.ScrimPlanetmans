@echo off

echo Running Backfill_ItemCategoryDomainAndIsWeapon.sql...
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
sqlcmd -E -S (LocalDB)\MSSQLLocalDB -i "..\squittal.ScrimPlanetmans.App\Data\SQL\MigrationHelpers\Backfill_ItemCategoryDomainAndIsWeapon.sql"
echo.
exit /B