@echo off
cls
powershell.exe .\Run-build.ps1 -buildFile:build.ps1 %1 %2 %3 %4 %5 %6 %7 %8 %9
powershell.exe .\Run-build.ps1 -buildFile:Artifacts.ps1 %1 %2 %3 %4 %5 %6 %7 %8 %9
powershell.exe .\Run-build.ps1 -buildFile:.\nuget.ps1 %1 %2 %3 %4 %5 %6 %7 %8 %9 