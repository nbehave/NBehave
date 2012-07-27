param($build = "", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -depends Clean, Version, InstallNunitRunners
task Default -depends Compile, Test

task Clean {
	if ($false -eq (Test-Path "$buildDir")) {
		New-Item $buildDir -type directory
	}
	else {
		Get-ChildItem $buildDir\* -Recurse | Select -ExpandProperty FullName | Sort {$_.Length} -Descending | ForEach-Object { Remove-Item $_ }
		Write-Host "Files removed."
		Get-ChildItem $buildDir -Recurse -Include * | Select -ExpandProperty FullName | Sort {$_.Length} -Descending | ForEach-Object { Remove-Item $_ }
		Write-Host "Folders removed."
	}
	New-Item $testReportsDir -type directory
	if ($false -eq (Test-Path "$sourceDir\packages")) {
		New-Item "$sourceDir\packages" -type directory
	}
}

Task InstallNunitRunners {
	Exec { .\src\.nuget\NuGet.exe install nunit.runners -Version 2.6.0.12051 -OutputDirectory src\packages\ }
}

Task Version {
	if ($build -match "^\-") { $build = "$version$build" }
	write-host "##teamcity[buildNumber '$build']"

	$asmInfo = "$sourceDir\CommonAssemblyInfo.cs"
	$src = Get-Content $asmInfo
	$newSrc = foreach($row in $src) {
		if ($row -match 'Assembly((InformationalVersion)|(Version)|(FileVersion))\s*\(\s*"\d+\.\d+\.\d+.*"\s*\)') {
			if ($row -match 'AssemblyInformationalVersion') {
				$row -replace '\d+\.\d+\.\d+.*.*"', ("$build" + '"')
			}
			else { $row -replace "\d+\.\d+\.\d+.\d+", "$version.0" }
		}
		else { $row }
	}
	Set-Content -path $asmInfo -value $newSrc
}

Task Compile -depends Compile-Console-x86, CompileAnyCpu

Task CompileAnyCpu {
	Exec { msbuild "$sourceDir\NBehave.sln" /p:Configuration=AutomatedDebug-$frameworkVersion /v:m /m /p:TargetFrameworkVersion=v$frameworkVersion /toolsversion:4.0 /t:Rebuild }
}

Task Compile-Console-x86 {
	$params = "Configuration=AutomatedDebug-$frameworkVersion-x86;Platform=x86;OutputPath=$buildDir\Debug-$frameworkVersion\NBehave\"
	Exec { msbuild "$sourceDir\NBehave.Console\NBehave.Console.csproj" /p:$params /p:TargetFrameworkVersion=v$frameworkVersion /v:m /m /toolsversion:$frameworkVersion /t:Rebuild }
	Move-Item "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console.exe" "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console-x86.exe" -Force
}

Task ILMerge -depends Compile {
	$buildDirFramework = "$buildDir\Debug-$frameworkVersion"
	$directory = "$buildDirFramework\NBehave"
	$resharperDir = "$buildDirFramework\Resharper"
	$out = "NBehave.Narrator.Framework.dll"
	$assemblies = @("$directory\NBehave.Narrator.Framework.dll", "$directory\GurkBurk.dll", "$directory\NBehave.Gherkin.dll")

	Run-ILMerge $snk $directory $out $assemblies
	Remove-Item "$directory\GurkBurk.dll"
	Remove-Item "$directory\NBehave.Gherkin.dll"
}

Task Test -depends Compile {
	new-item $testReportsDir -type directory -ErrorAction SilentlyContinue

	$arguments = Get-Item "$testDir\*Specifications*.dll"
	$arguments = $arguments + " /xml:$testReportsDir\UnitTests-$frameworkVersion.xml"
	$basePath =  (get-item src/packages/nunit.runners*).Name
	$nunitExe =  ".\src\packages\$basePath\tools\nunit-console-x86.exe"
	$cmd = $executioncontext.invokecommand.NewScriptBlock("$nunitExe $arguments")
	Exec $cmd
}