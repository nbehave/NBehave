function SwitchDotNetFrameworkVersion
{
param(
	[string]$version
)
	$framework = $version
	Configure-BuildEnvironment
}