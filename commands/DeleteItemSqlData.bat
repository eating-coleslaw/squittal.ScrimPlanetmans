@echo off

echo Starting SQL Server service...
call :StartSqlService wasSqlRunning
if %ERRORLEVEL% EQU 2 (
    echo Failed to start SQL Server service. Try re-running this file as administrator.
    echo Exiting...
    pause
    exit
)
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start SQL Server service. Exiting...
    pause/
    exit
)

echo Running TruncateItemTables.sql...
call :RunSqlFile
if %ERRORLEVEL% NEQ 0 (
    echo Failed to run the SQL file. Exiting...
    pause
    exit
)

if %wasSqlRunning% EQU 1 (
    echo Keeping SQL Server service in running state. Exiting...
    pause
    exit
)
echo Returning SQL Server service to stopped state...
call :StopSqlService
if %ERRORLEVEL% EQU 2 (
    echo Failed to stop SQL Server service. Try re-running this file as administrator.
    echo Exiting...
    pause
    exit
)
if %ERRORLEVEL% NEQ 0 (
    echo Failed to stop SQL Server service. Exiting...
    pause
    exit
)
pause
exit


:StartSqlService
SC query MSSQL$SQLEXPRESS | FIND "STATE" | FIND "RUNNING" 2>&1 > nul
if errorlevel 1 (net start MSSQL$SQLEXPRESS & set "%~1=0") else (echo The SQL Server service is already running... & set "%~1=1" & echo.)
REM echo.
exit /B

:StopSqlService
SC query MSSQL$SQLEXPRESS | FIND "STATE" | FIND "STOPPED" 2>&1 > nul
if errorlevel 1 (net stop MSSQL$SQLEXPRESS) else (echo The SQL Server service isn't running... & echo.)
exit /B

:RunSqlFile
cd "%~dp0"
sqlcmd -E -S %COMPUTERNAME%\SQLEXPRESS -i "..\squittal.ScrimPlanetmans.App\Data\SQL\TruncateItemTables.sql"
echo.
exit /B