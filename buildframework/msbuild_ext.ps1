function BuildSolution
{
	param([string]$buildfile, [string]$version)
	
	SwitchDotNetFrameworkVersion $version
	Exec { msbuild $buildfile /property:Configuration=AutomatedDebug-$version /v:m /p:TargetFrameworkVersion=v$version /toolsversion:$version }

}