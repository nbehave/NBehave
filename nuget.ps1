param($build = "", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -precondition { return $frameworkVersion -eq "4.0" }
task Default -depends  NuGet, NuGet-Spec, NuGet-Fluent -precondition { return $frameworkVersion -eq "4.0" }

function Add-Dependencies($name, $type, $outFile, $keyValues) {
	Copy-Item "$packageTemplateDir\NBehave.$type.nuspec" $outFile -Force
	$xml = [xml](get-content $outFile)
	$xml.package.metadata.id = $xml.package.metadata.id + $name
	$xml.package.metadata.title = $xml.package.metadata.title + " " + $name
	$xml.package.metadata.tags = $xml.package.metadata.tags + " " + $name.ToLower()
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

function Set-LibName($name) {
	$file = "$tempNugetDir\NBehave.Spec.$name.nuspec"
	$xml = [xml](get-content $file)
	$xml.package.files.file | ForEach-Object {
		$_.src = [string]::Format($_.src, $name)
	}
	$xml.Save($file)
}

function Set-Using($name, $methodAttrib, $namespace) {
	Copy-Item "$packageTemplateDir\*.feature" -Destination $tempNugetDir
	Get-ChildItem "$packageTemplateDir\*.cs" | Copy-Item -Destination $tempNugetDir
	Get-ChildItem "$tempNugetDir\*.cs" | ForEach-Object {
		$file = $_
		$content = [string]::Join("`n", (Get-Content $file))
		if ($content -contains "*{0}*") {
			$content = [string]::Format($content, $name, $methodAttrib, $namespace)
		}
		Set-Content -path "$file" -value $content -Force
	}
}

task Clean {
	if ($true -eq (Test-Path $tempNugetDir)) { Remove-Item $tempNugetDir -Recurse }
	New-Item $tempNugetDir -type Directory
	if ($false -eq (Test-Path $artifactsDir)) { New-Item $artifactsDir -type Directory }
	Remove-Item $artifactsDir\*.nuget
}

task NuGet -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	Exec { .\tools\nuget\nuget.exe pack nuget\nbehave.nuspec -Version $build -OutputDirectory $artifactsDir}
	Exec { .\tools\nuget\nuget.exe pack nuget\nbehave.samples.nuspec -Version $build -OutputDirectory $artifactsDir}
	#Exec { .\tools\nuget\nuget.exe pack nuget\NBehave.Resharper.nuspec -Version $build -OutputDirectory $artifactsDir}
	#Exec { .\tools\nuget\nuget.exe pack nuget\NBehave.VsPlugin.nuspec -Version $build -OutputDirectory $artifactsDir}
}

function Build-FluentPackage($name, $namespace, $metaData) {
	Add-Dependencies $name "Fluent" "$tempNugetDir\NBehave.Fluent.$name.nuspec"  $metaData
	Set-Using $namespace "Specification" $namespace
	Exec { .\tools\nuget\nuget.exe pack "$tempNugetDir\NBehave.Fluent.$name.nuspec"  -Version $build -OutputDirectory $artifactsDir}
}

task NuGet-Fluent -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	Build-FluentPackage "NUnit" "NUnit.Framework" @( @("nunit", "2.5.10.11092"), @("", "") )
	Build-FluentPackage "MbUnit" "MbUnit.Framework" @( @("MbUnit", "3.3.454.0"), @("", "") )
	Build-FluentPackage "Xunit" "Xunit.Framework" @( @("Xunit", "1.8.0.1549"), @("xunit.extensions", "1.8.0.1549") )
	Build-FluentPackage "MsTest" "Microsoft.VisualStudio.TestTools.UnitTesting" @( @("", ""), @("", "") )
}

function Build-SpecPackage($name, $namespace, $metaData) {
	Add-Dependencies $name "Spec" "$tempNugetDir\NBehave.Spec.$name.nuspec"  $metaData
	Set-LibName  $name
	Set-Using $name "Specification" $namespace
	Exec { .\tools\nuget\nuget.exe pack "$tempNugetDir\NBehave.Spec.$name.nuspec"  -Version $build -OutputDirectory $artifactsDir}
}

task NuGet-Spec -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	Build-SpecPackage "NUnit" "NUnit.Framework" @( @("nunit", "2.5.10.11092"), @("", "") )
	Build-SpecPackage "MbUnit" "MbUnit.Framework" @( @("mbunit", "3.3.454.0"),   @("", "") )
	Build-SpecPackage "Xunit" "Xunit" @( @("xunit", "1.8.0.1549") ,  @("xunit.extensions", "1.8.0.1549") )
	Build-SpecPackage "MsTest" "Microsoft.VisualStudio.TestTools.UnitTesting" @( @("", ""), @("", "") )
}
