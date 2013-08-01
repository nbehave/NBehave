@REM ********
@REM To pass parameters
@REM .\buildframework\FAKE\tools\FAKE.exe build.fsx target=Compile
@REM ********
@ECHO OFF
CLS
IF EXIST .\buildframework\FAKE GOTO RunBuild
.\src\.nuget\NuGet.exe install FAKE -OutputDirectory .\buildframework\ -ExcludeVersion -Pre

:RunBuild
.\buildframework\FAKE\tools\FAKE.exe build.fsx %1 %2 %3 %4 %5 %6 %7 %8 %9