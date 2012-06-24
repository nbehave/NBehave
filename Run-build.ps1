param(
	$task = "default",
	$buildFile = ".\build.ps1",
	$version = "0.6.0",
	$versionSuffix = "-dev.local.0"
)

$env:EnableNuGetPackageRestore = $TRUE

Write-Host "buildFile $buildFile"
Write-Host "task $task"
Write-Host "version $version"
Write-Host "versionSuffix $versionSuffix"
$buildNumber = "$version$versionSuffix"
write-host "##teamcity[buildNumber '$buildNumber']"

function Build($framework, $taskToRun) {
	invoke-psake $buildFile -framework '4.0x86' -t $taskToRun -parameters @{"version"="$version";"frameworkVersion"="$framework";"versionSuffix"="$versionSuffix";"environment"="$environment"}
	if ($psake.build_success -eq $FALSE) {
		$errMsg = $psake.build_error_message
		$replace = @{ "\|" = "||"; "`n" = "|n"; "`r" = "|r"; "'" = "|'"; "\[" = "|["; "\]"  = "|]"; '0x0085' = "|x"; '0x2028' = "|l"; '0x2029' = "|p" }
		$replace.GetEnumerator() | ForEach-Object {  $errMsg = $errMsg -replace $_.Key, $_.Value }
		write-host "##teamcity[message text='build failed: $errMsg' errorDetails='$errMsg' status='ERROR']"
		throw $errMsg
	}
}

#$scriptPath = Split-Path -parent $MyInvocation.MyCommand.path
$scriptPath = (Get-Location).Path
remove-module psake -ea 'SilentlyContinue'
Import-Module (join-path $scriptPath ".\buildframework\psake.psm1")

if (-not(test-path $buildFile)) {
    $buildFile = (join-path $scriptPath $buildFile)
}

Build "3.5" "Init"
Build "3.5" $task
Build "4.0" $task