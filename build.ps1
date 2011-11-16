param(
	$version          = "0.1.0.0",
	$frameworkVersion = "4.0"
)

properties {
	$rootDir          = Split-Path $psake.build_script_file
	$sourceDir        = "$rootDir\src"
	$buildDir         = "$rootDir\build"
	$distDir          = "$buildDir\dist"
	$toolsDir         = "$rootDir\tools"
	$testReportsDir   = "$buildDir\test-reports"
	$testDir          = "$buildDir\Debug-$frameworkVersion\UnitTests"
}
Include ".\buildframework\psake_ext.ps1"

task Init -depends Clean, Version
task Default -depends Test
task All -depends Test, Distribute, DistributeInstaller

task Clean { 
	if ($true -eq (Test-Path "$buildDir")) {
		Remove-Item $buildDir -Recurse
	}
	New-Item $buildDir -type directory
	New-Item $testReportsDir -type directory
	New-Item $distDir -type directory
}

Task Version {
	if ($environment -ne "Dev")  {
		$asmInfo = "$sourceDir\CommonAssemblyInfo.cs"
		$src = Get-Content $asmInfo
		$newSrc = foreach($row in $src) { 
			if ($row -match 'Assembly((Version)|(FileVersion))\s*\(\s*"\d+\.\d+\.\d+\.\d+"\s*\)') { 
				$row -replace "\d+\.\d+\.\d+\.\d+", $version 
			}
			else { $row }
		}
		Set-Content -path $asmInfo -value $newSrc			
	}
}

Task Compile -depends Compile-Console-x86, CompileAnyCpu

Task CompileAnyCpu {
	Exec { msbuild "$sourceDir\NBehave.sln" /p:Configuration=AutomatedDebug-$frameworkVersion /v:m /p:TargetFrameworkVersion=v$frameworkVersion /toolsversion:$frameworkVersion /t:Rebuild }
}

Task Compile-Console-x86 {
	$params = "Configuration=AutomatedDebug-$frameworkVersion-x86;Platform=x86;OutputPath=$buildDir\Debug-$frameworkVersion\NBehave\"
	Exec { msbuild "$sourceDir\NBehave.Console\NBehave.Console.csproj" /p:$params /p:TargetFrameworkVersion=v$frameworkVersion /v:m /toolsversion:$frameworkVersion /t:Rebuild }
	Move-Item "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console.exe" "$buildDir\Debug-$frameworkVersion\NBehave\NBehave-Console-x86.exe" -Force
}

Task Test -depends Compile {
	new-item $testReportsDir -type directory -ErrorAction SilentlyContinue
	
	$arguments = Get-Item "$testDir\*Specifications*.dll"
	Exec { .\tools\nunit\nunit-console-x86.exe $arguments /xml:$testReportsDir\UnitTests-$frameworkVersion.xml}
}

Task Distribute -depends DistributeVSPlugin, DistributeBinaries, DistributeResharper, DistributeExamples

Task DistributeVSPlugin -precondition { return $frameworkVersion -eq "4.0" } {	
	new-item -path "$distDir\v$frameworkVersion" -name "VSPlugin" -type directory -ErrorAction SilentlyContinue
	$destination = "$distDir\v$frameworkVersion\VSPlugin"
	$source = "$buildDir\Debug-$frameworkVersion\VSPlugin"
	
	Get-ChildItem "$source\*.*" -Include *.dll, *.vsixmanifest, *.pkgdef, *.pdb | Copy-Item -Destination $destination
	
	$namespaces = @{ "vsx" = "http://schemas.microsoft.com/developer/vsx-schema/2010"}
	$xpath = "/vsx:Vsix/vsx:Identifier/vsx:"
	xmlPoke "$destination\extension.vsixmanifest" $xpath"InstalledByMsi" "true" $namespaces
	xmlPoke "$destination\extension.vsixmanifest" $xpath"Version" $version $namespaces
}

Task DistributeBinaries {
	new-item -path "$buildDir" -name "dist" -type directory -ErrorAction SilentlyContinue
	new-item -path "$buildDir" -name "dist\v$frameworkVersion" -type directory -ErrorAction SilentlyContinue

	$destination = "$distDir\v$frameworkVersion"
	$source = "$buildDir\Debug-$frameworkVersion\NBehave"
	$exclusions = @("*Microsoft*", "log4net.dll", "NAnt.Core.dll", "TestDriven.Framework.dll")
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude $exclusions | Copy-Item -Destination $destination
}

Task DistributeExamples -precondition { return $frameworkVersion -eq "3.5" } {
	$examplesdest_dir = "$buildDir\Examples\"
	$examplessource_dir = "$sourceDir\NBehave.Examples"
	
	Remove-Item $examplesdest_dir -Recurse -ErrorAction SilentlyContinue
	New-Item -Path $buildDir -Name "Examples" -type directory -ErrorAction SilentlyContinue
	
	$items = Get-ChildItem $examplessource_dir -Recurse 
	$items = $items | where {$_.FullName -notmatch "obj" -and $_.FullName -notmatch "bin"} 
	$items | Copy-Item -Destination {Join-Path $examplesdest_dir $_.FullName.Substring($examplessource_dir.length)}

	$examplesdest_dir_lib = "$examplesdest_dir\lib\"
	New-Item -Path $examplesdest_dir -Name "lib" -type directory
	Get-ChildItem "$buildDir\dist\v$frameworkVersion\*.*" -Include *NBehave*, *.dll | Copy-Item -Destination $examplesdest_dir_lib
	
	$namespaces = @{ "xsi" = "http://schemas.microsoft.com/developer/msbuild/2003"}
	$xpath = "//xsi:Reference/xsi:HintPath/../@Include"
	
	$path = "$examplesdest_dir\NBehave.Examples.csproj" 
	$nodes = xmlList $path $xpath $namespaces
	
	foreach($node in $nodes)
	{
		$xpath = "//xsi:Reference[@Include='$node']/xsi:HintPath"
		$pathToReference = xmlPeek $path $xpath $namespaces
		$dllFile = [regex]::Match($pathToReference, "(?<dllfile>(\w+\.)+dll$)").Groups["dllfile"].Value
		
		$xpath = "//xsi:Reference[@Include='$node']/xsi:HintPath"
		xmlPoke $path $xpath "lib\$dllfile" $namespaces
	}
	
	zip "$buildDir\NBehave.Examples.zip" $examplesdest_dir
}

Task DistributeResharper {
	new-item -path "$distDir\v$frameworkVersion" -name "ReSharper" -type directory -ErrorAction SilentlyContinue
	$destination = "$distDir\v$frameworkVersion\ReSharper"
	$source = "$buildDir\Debug-$frameworkVersion\ReSharper"
	$exclusions = @("*JetBrains*", "*Microsoft*", "log4net.dll", "NAnt.Core.dll", "TestDriven.Framework.dll")
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude $exclusions | Copy-Item -Destination $destination
}

Task BuildInstaller -depends Distribute -precondition { return $frameworkVersion -eq "4.0" } {
	Exec { .\tools\nsis\makensis.exe /DVERSION=$version "$sourceDir\Installer\NBehave.nsi"}
}

Task ILMerge -depends -Distribute {
	$key = "$sourceDir\NBehave.snk"
	$directory = "$distDir\v$frameworkVersion"
	$name = "NBehave.Narrator.Framework"
	
	$assemblies = @("$directory\gherkin.dll", 
					"$directory\NBehave.Spec.Framework.dll")
	
	ilmerge $key $directory $name $assemblies "dll"
}
 

