param($version = "0.1.0.0", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

Task Init -depends Clean
Task Distribute -depends DistributeVSPlugin, DistributeBinaries, DistributeResharper, DistributeExamples, DistributeZip
Task Default -depends Distribute, BuildInstaller

task Clean { 
	$dirs = @($installerDir, $artifactsDir, "$buildDir\Examples")
	$dirs | ForEach-Object {		
		if ($true -eq (Test-Path $_)) { Write-Host "DIR: $_" }
		if ($true -eq (Test-Path $_)) { Remove-Item $_ -Recurse }
		New-Item $_ -type directory
	}
}

Task DistributeVSPlugin -precondition { return $frameworkVersion -eq "4.0" } {	
	new-item -path "$installerDir\v$frameworkVersion" -name "VSPlugin" -type directory -ErrorAction SilentlyContinue
	$destination = "$installerDir\v$frameworkVersion\VSPlugin"
	$source = "$buildDir\Debug-$frameworkVersion\VSPlugin"
	
	Get-ChildItem "$source\*.*" -Include *.dll, *.vsixmanifest, *.pkgdef -Exclude $exclusions | Copy-Item -Destination $destination
	
	$namespaces = @{ "vsx" = "http://schemas.microsoft.com/developer/vsx-schema/2010"}
	$xpath = "/vsx:Vsix/vsx:Identifier/vsx:"
	xmlPoke "$destination\extension.vsixmanifest" $xpath"InstalledByMsi" "true" $namespaces
	xmlPoke "$destination\extension.vsixmanifest" $xpath"Version" $version $namespaces
}

Task DistributeBinaries {
	new-item "$installerDir\v$frameworkVersion" -type directory -ErrorAction SilentlyContinue

	$destination = "$installerDir\v$frameworkVersion"
	$source = "$buildDir\Debug-$frameworkVersion\NBehave"
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude $exclusions | Copy-Item -Destination $destination
}

Task DistributeResharper {
	new-item -path "$installerDir\v$frameworkVersion" -name "ReSharper" -type directory -ErrorAction SilentlyContinue
	$destination = "$installerDir\v$frameworkVersion\ReSharper"
	$source = "$buildDir\Debug-$frameworkVersion\ReSharper"
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude $exclusions | Copy-Item -Destination $destination
}

Task DistributeZip -depends DistributeBinaries, DistributeResharper, DistributeVSPlugin {
	$destFile = "$installerDir\NBehave.$version.zip"
	Exec { .\tools\7zip\7za.exe a -r "$destFile" "$installerDir\v$frameworkVersion" }
}

Task DistributeExamples -precondition { return $frameworkVersion -eq "3.5" } {
	$examplesDestDir = "$buildDir\Examples\"
	$examplesSourceDir = "$sourceDir\NBehave.Examples"
	
	New-Item -Path $buildDir -Name "Examples" -type directory -ErrorAction SilentlyContinue
	
	$items = Get-ChildItem $examplesSourceDir -Recurse 
	$items = $items | where {$_.FullName -notmatch "obj" -and $_.FullName -notmatch "bin"} 
	$items | Copy-Item -Destination {Join-Path $examplesDestDir $_.FullName.Substring($examplesSourceDir.length)}

	$examplesDestDirLib = "$examplesDestDir\lib\"
	New-Item $examplesDestDirLib -type directory
	Get-ChildItem "$installerDir\v$frameworkVersion\*.*" -Include *NBehave*, *.dll -Exclude $exclusions | Copy-Item -Destination $examplesDestDirLib
	
	$namespaces = @{ "xsi" = "http://schemas.microsoft.com/developer/msbuild/2003"}
	$xpath = "//xsi:Reference/xsi:HintPath/../@Include"
	
	$path = "$examplesDestDir\NBehave.Examples.csproj" 
	$nodes = xmlList $path $xpath $namespaces
	
	foreach($node in $nodes)
	{
		$xpath = "//xsi:Reference[@Include='$node']/xsi:HintPath"
		$pathToReference = xmlPeek $path $xpath $namespaces
		$dllFile = [regex]::Match($pathToReference, "(?<dllfile>(\w+\.)+dll$)").Groups["dllfile"].Value
		
		$xpath = "//xsi:Reference[@Include='$node']/xsi:HintPath"
		xmlPoke $path $xpath "lib\$dllfile" $namespaces
	}
	Exec { .\tools\7zip\7za.exe a -r "$installerDir\NBehave.Examples.zip" $examplesDestDir }
}

Task BuildInstaller -depends Distribute -precondition { return $frameworkVersion -eq "4.0" } {
	Exec { .\tools\nsis\makensis.exe /DVERSION=$version "$sourceDir\Installer\NBehave.nsi"}
	Move-Item "$installerDir\*.exe" "$artifactsDir"
}