set args=%*
fsi "%~dp0fsx\work.fsx" %args%

@echo off

rem https://stackoverflow.com/a/7006016/632495
rem if not x%args:me (
cd C:\r\Platform
rem ) else (
rem cd C:\s
rem )

rem https://stackoverflow.com/a/14275800/632495
rem @echo off
rem set /p option=Enter (y) if you would like to change directory:
rem if %option%==y (
rem cd C:\r\Platform
rem )

