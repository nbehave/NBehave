@echo off
@tools\nant\nant.exe -v+ -buildfile:NBehave.build %*
REM powershell -NoProfile -ExecutionPolicy unrestricted -Command "& '%~dp0\buildframework\build.ps1' %*"
