@echo off

echo Stopping the SQL Server service...
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

:StopSqlService
SC query MSSQL$SQLEXPRESS | FIND "STATE" | FIND "STOPPED" 2>&1 > nul
if errorlevel 1 (net stop MSSQL$SQLEXPRESS) else (echo The SQL Server service isn't running... & echo.)
exit /B