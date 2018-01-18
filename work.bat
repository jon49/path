set args=%*
fsi "%~dp0fsx\work.fsx" %args%

rem https://stackoverflow.com/a/7006016/632495
if not x%args:me (
cd C:\r\Platform
) else (
cd C:\s
)

rem https://stackoverflow.com/a/14275800/632495
rem @echo off
rem set /p option=Enter (y) if you would like to change directory:
rem if %option%==y (
rem cd C:\r\Platform
rem )

