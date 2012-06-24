param($build = "", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -depends Clean, Version
task Default -depends Test #, ILMerge

task Clean {
	if ($true -eq (Test-Path "$buildDir")) {
		Get-ChildItem $buildDir\**\*.* -Recurse | where { $_.mode -notmatch "d"} | Sort-Object mode | ForEach-Object { Remove-Item $_.FullName }
		Write-Host "Files removed."
		Remove-Item $buildDir -Recurse
	}
	New-Item $buildDir -type directory
	New-Item $testReportsDir -type directory
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
	$snk = "$sourceDir\NBehave.snk"
	$buildDirFramework = "$buildDir\Debug-$frameworkVersion"
	$directory = "$buildDirFramework\NBehave"
	$resharperDir = "$buildDirFramework\Resharper"
	$out = "NBehave.Narrator.Framework.dll"
	$assemblies = @("$directory\NBehave.Narrator.Framework.dll", "$directory\Gherkin.dll", "$directory\NBehave.Gherkin.dll")

	Run-ILMerge $snk $directory $out $assemblies
	Remove-Item "$directory\Gherkin.dll"
	Remove-Item "$directory\NBehave.Gherkin.dll"
}

Task Test -depends Compile {
	new-item $testReportsDir -type directory -ErrorAction SilentlyContinue

	$arguments = Get-Item "$testDir\*Specifications*.dll"
	Exec { .\tools\nunit\nunit-console-x86.exe $arguments /xml:$testReportsDir\UnitTests-$frameworkVersion.xml}
}