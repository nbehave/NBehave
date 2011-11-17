@REM ********
@REM To pass parameters you do:
@REM "buildAll.cmd -task:Clean
@REM ********

@echo off
cls
powershell.exe .\Run-build.ps1 %1 %2 %3 %4 %5 %6 %7 %8 %9
powershell.exe .\Run-build.ps1 -buildFile:Artifacts.ps1 %1 %2 %3 %4 %5 %6 %7 %8 %9