@REM ********
@REM To pass parameters
@REM .\buildframework\FAKE\tools\FAKE.exe build.fsx Compile
@REM ********
@echo off
cls

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

packages\FAKE\tools\FAKE.exe build.fsx %*
