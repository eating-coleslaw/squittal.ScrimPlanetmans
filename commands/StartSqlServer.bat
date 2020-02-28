@echo off

echo Starting SQL Server service...
call :StartSqlService
if %ERRORLEVEL% EQU 2 (
    echo Failed to start SQL Server service. Try re-running this file as administrator.
    echo Exiting...
    pause
    exit
)
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start SQL Server service. Exiting...
    pause
    exit
)
pause


:StartSqlService
SC query MSSQL$SQLEXPRESS | FIND "STATE" | FIND "RUNNING" 2>&1 > nul
if errorlevel 1 (net start MSSQL$SQLEXPRESS) else (echo The SQL Server service is already running...)
exit /B