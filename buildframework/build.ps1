$buildFile = "default.ps1"
$docs = $false
$properties = @{}
$parameters = @{}

remove-module psake -ea 'SilentlyContinue'
$scriptPath = Split-Path -parent $MyInvocation.MyCommand.path
import-module (join-path $scriptPath psake.psm1)
if (-not(test-path $buildFile))
{
    $buildFile = (join-path $scriptPath $buildFile)
} 
invoke-psake $buildFile "Clean" "3.5" $docs $parameters $properties
invoke-psake $buildFile "Increment" "3.5" $docs $parameters $properties
invoke-psake $buildFile "RunBuild" "3.5" $docs $parameters $properties
invoke-psake $buildFile "RunBuild" "4.0" $docs $parameters $properties
invoke-psake $buildFile "BuildInstaller" "3.5" $docs $parameters $properties