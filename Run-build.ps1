param(
	$task = "default", 
	$version = "0.1.0.0", 
	$buildFile = ".\build.ps1"
)
function Build($framework, $taskToRun) {
	invoke-psake $buildFile -framework '4.0x86' -t $taskToRun -parameters @{"version"="$version";"frameworkVersion"="$framework"}
	if ($LastExitCode -ne $null) {
		if ($LastExitCode -ne 0) { 
			$msg = "build exited with errorcode $LastExitCode"
			throw $msg 
		}
	}
}

#$scriptPath = Split-Path -parent $MyInvocation.MyCommand.path
$scriptPath = (Get-Location).Path
remove-module psake -ea 'SilentlyContinue'
Import-Module (join-path $scriptPath ".\buildframework\psake.psm1")

if (-not(test-path $buildFile)) {
    $buildFile = (join-path $scriptPath $buildFile)
} 

Build "3.5" "Clean"
Build "3.5" $task
Build "4.0" $task
Build "3.5" "Distribute"
Build "4.0" "BuildInstaller"
