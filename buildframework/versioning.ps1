function Generate-Assembly-Info
{
param(
	[string]$clsCompliant = "true",
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$file = $(throw "file is a required parameter.")
)
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyInformationalVersionAttribute(""$version"")]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	Write-Output $asmInfo > $file
}

function GetVersion([string]$file, [bool]$increment)
{
	$versionNumber = Get-Content $file
	$versionArray = $versionNumber.Split(".")
	$buildNumber = [int]($versionArray[3])
	
	#Only increment once for both 3.5 and 4.0 builds
	if($increment)
	{
		$buildNumber = [int]($versionArray[3]) + 1
	}
	
	$versionArray[3] = ($buildNumber).ToString()
	$finalVersion = ([string]$versionArray).Replace(" ",".")
	Write-Output $finalVersion > $file
	return $finalVersion	
}