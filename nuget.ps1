param($version = "0.1.0.0", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init
task Default -depends  Core, Spec, Fluent

task Clean { 
	Remove-Item "$artifactsDir\*.nupkg"
}

task Core -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	Exec { .\tools\nuget\nuget.exe pack nuget\NBehave.Core.nuspec -Version $version -OutputDirectory $artifactsDir}
}

function AddDependencies($type, $outFile, $keyValues) {
	Copy-Item "$packageTemplateDir\NBehave.$type.nuspec" $outFile -Force
	$xml = [xml](get-content $outFile)
	$xml.package.metadata.id = $xml.package.metadata.id + $keyValues[0][0]
	$keyValues | ForEach-Object {
		$deps = $xml.package.metadata.dependencies
		$clone = @($deps.dependency)[0].Clone()
		if ($_[0] -ne "") {
			$clone.id = $_[0]
			$clone.version = $_[1]
			$deps.AppendChild($clone)
		}
	}
	$xml.Save($outFile)
}

task Fluent -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	AddDependencies "Fluent" "$buildDir\NBehave.Fluent.nunit.nuspec"  @( @("nunit", "2.5.10.11092"), @("", "") )
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Fluent.nunit.nuspec"  -Version $version -OutputDirectory $artifactsDir}
	AddDependencies "Fluent" "$buildDir\NBehave.Fluent.mbunit.nuspec" @( @("mbunit", "3.3.454.0"),   @("", "") )
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Fluent.mbunit.nuspec" -Version $version -OutputDirectory $artifactsDir}
	AddDependencies "Fluent" "$buildDir\NBehave.Fluent.xunit.nuspec"  @( @("xunit", "1.8.0.1549") ,  @("xunit.extensions", "1.8.0.1549") )
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Fluent.xunit.nuspec"  -Version $version -OutputDirectory $artifactsDir}
	
	# mstest dont have a nuget package, so handle that....
	#Copy-Item "$packageTemplateDir\NBehave.Fluent.nuspec" "$buildDir\NBehave.Fluent.mstest.nuspec" -Force
}

function SetLibName($name) {
	$file = "$buildDir\NBehave.Spec.$name.nuspec"
	$xml = [xml](get-content $file)
	$xml.package.files.file | ForEach-Object {
		Write-Host $_.Name
		$_.src = [string]::Format($_.src, $name)
		Write-Host $_.Name
	}	
	$xml.Save($file)
}

task Spec -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	AddDependencies "Spec" "$buildDir\NBehave.Spec.nunit.nuspec"  @( @("nunit", "2.5.10.11092"), @("", "") )
	SetLibName  "nunit"
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Spec.nunit.nuspec"  -Version $version -OutputDirectory $artifactsDir}
	AddDependencies "Spec" "$buildDir\NBehave.Spec.mbunit.nuspec" @( @("mbunit", "3.3.454.0"),   @("", "") )
	SetLibName "mbunit"
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Spec.mbunit.nuspec" -Version $version -OutputDirectory $artifactsDir}
	AddDependencies "Spec" "$buildDir\NBehave.Spec.xunit.nuspec"  @( @("xunit", "1.8.0.1549") ,  @("xunit.extensions", "1.8.0.1549") )
	SetLibName "xunit"
	Exec { .\tools\nuget\nuget.exe pack "$buildDir\NBehave.Spec.xunit.nuspec"  -Version $version -OutputDirectory $artifactsDir}
	
	# mstest dont have a nuget package, so handle that....
	#Copy-Item "$packageTemplateDir\NBehave.Fluent.nuspec" "$buildDir\NBehave.Fluent.mstest.nuspec" -Force
}
