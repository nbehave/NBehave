@ECHO OFF

@tools\nant\nant.exe -buildfile:NBehave.build %*

REM @ECHO OFF

REM IF "%1" == "" (
REM 	@%windir%\microsoft.net\framework\v3.5\msbuild.exe NBehave.proj
REM ) ELSE (
REM 	@%windir%\microsoft.net\framework\v3.5\msbuild.exe /t:%* NBehave.proj
REM )
