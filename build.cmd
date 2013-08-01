@REM ********
@REM To pass parameters
@REM .\buildframework\FAKE\tools\FAKE.exe build.fsx Compile
@REM ********
@ECHO OFF
CLS
IF EXIST .\buildframework\FAKE GOTO DotNetZip
.\src\.nuget\NuGet.exe install FAKE -OutputDirectory .\buildframework\ -ExcludeVersion -Pre

:DotNetZip
IF EXIST .\src\packages\DotNetZip GOTO RunBuild
.\src\.nuget\NuGet.exe install DotNetZip -OutputDirectory .\src\packages\ -ExcludeVersion

:RunBuild
.\buildframework\FAKE\tools\FAKE.exe build.fsx %1 %2 %3 %4 %5 %6 %7 %8 %9