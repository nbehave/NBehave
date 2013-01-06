param($frameworkVersion = "4.0", $buildNumber = "0", $buildPrefix = "-devlocal" )


Include ".\BuildProperties.ps1"
Include ".\ExtraArgs.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -depends Clean, Version, InstallNunitRunners
task Default -depends Compile, Test

task Clean {
  if ($false -eq (Test-Path "$buildDir")) {
    New-Item $buildDir -type directory
  }
  else {
    clearDir $buildDir
  }
  New-Item $testReportsDir -type directory
  if ($false -eq (Test-Path $artifactsDir)) { New-Item $artifactsDir -type Directory }
  if ($false -eq (Test-Path "$sourceDir\packages")) {
    New-Item "$sourceDir\packages" -type directory
  }
}

Task InstallNunitRunners {
  Exec { .\src\.nuget\NuGet.exe install nunit.runners -Version 2.6.2 -OutputDirectory src\packages\ }
}

Task Version {
  $b = BuildNumber
  write-host "##teamcity[buildNumber '$b']"

  $asmInfo = "$sourceDir\CommonAssemblyInfo.cs"
  $src = Get-Content $asmInfo
  $newSrc = foreach($row in $src) {
    if ($row -match 'Assembly((InformationalVersion)|(Version)|(FileVersion))\s*\(\s*"\d+\.\d+\.\d+.*"\s*\)') {
      if ($row -match 'AssemblyInformationalVersion') {
        $row -replace '\d+\.\d+\.\d+.*.*"', ("$assemblyInfoVersion" + '"')
      }
      else { $row -replace "\d+\.\d+\.\d+\.\d+", "$assemblyVersion" }
    }
    else { $row }
  }
  Set-Content -path $asmInfo -value $newSrc

  #Version for source.extension.vsixmanifest
  $vsixFile = "$sourceDir\NBehave.VS2010.Plugin\source.extension.vsixmanifest"
  [xml] $xml = Get-Content $vsixFile
  $xml.Vsix.Identifier.Version = "$assemblyVersion"
  $xml.Save($vsixFile)

  #Version for VS plugin package
  $package = "$sourceDir\NBehave.VS2010.Plugin\NBehaveRunnerPackage.cs"
  $src = Get-Content $package
  $newSrc = foreach($row in $src) {
    if ($row -match '\[InstalledProductRegistration\(') { $row -replace "\d+\.\d+\.\d+", "$version" }
    else { $row }
  }
  Set-Content -path $package -value $newSrc
}

Task Compile -depends Compile-Console-x86, CompileAnyCpu

Task CompileAnyCpu {
  Exec { msbuild "$sourceDir\NBehave.sln" /p:Configuration=AutomatedDebug-$frameworkVersion /v:m /m /p:TargetFrameworkVersion=v$frameworkVersion /toolsversion:4.0 /t:Rebuild }
}

Task Compile-Console-x86 {
  $params = "Configuration=AutomatedDebug-$frameworkVersion-x86;Platform=x86;OutputPath=$buildDir\Debug-$frameworkVersion\NBehave\"
  Exec { msbuild "$sourceDir\NBehave.Console\NBehave.Console.csproj" /p:$params /p:TargetFrameworkVersion=v$frameworkVersion /v:m /m /toolsversion:4.0 /t:Rebuild }
  Move-Item "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console.exe" "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console-x86.exe" -Force
}

Task Test -depends Compile {
  new-item $testReportsDir -type directory -ErrorAction SilentlyContinue

  $arguments = Get-Item "$testDir\*Specifications*.dll"
  $arguments = $arguments + " /xml:$testReportsDir\UnitTests-$frameworkVersion.xml"
  $basePath =  ((get-item src/packages/nunit.runners*) | Select Name | sort-object)[0].Name
  $nunitExe =  ".\src\packages\$basePath\tools\nunit-console-x86.exe"
  $cmd = $executioncontext.invokecommand.NewScriptBlock("$nunitExe $arguments")
  Exec $cmd
}