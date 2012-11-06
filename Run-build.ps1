param(
  $task = "default",
  $buildFile = ".\build.ps1",
  $build = "-devlocal0"
)

$env:EnableNuGetPackageRestore = $TRUE
$scriptPath = Split-Path -parent $MyInvocation.MyCommand.path

Write-Host "task $task"
Write-Host "buildFile $buildFile"
Write-Host "build $build"


function WriteExtraArguments() {
  Write-Host "==> FOO"
  Write-Host "args: $args"
  $s = $script:args
  Write-Host "All: $s"
  $argName = ""
  $content = ""
  foreach ( $arg in $s ) {
    write-host "-- $arg"
    if ($argName -eq "") {
      $m = $arg -match "\w+"
      $argName = $matches[0]
    }
    else {
      $value = $arg
      $content += "`$$argName = `"$value`"`r`n"
      $argName = ""
    }
  }
  write-host "content: $content"
  $content > "$scriptPath\ExtraArgs.ps1"
}

function Build($framework, $taskToRun) {
  invoke-psake $buildFile -framework '4.0x86' -t $taskToRun -parameters @{"build"="$build";"frameworkVersion"="$framework"}
  if ($psake.build_success -eq $FALSE) {
    $errMsg = $psake.build_error_message
    $replace = @{ "\|" = "||"; "`n" = "|n"; "`r" = "|r"; "'" = "|'"; "\[" = "|["; "\]"  = "|]"; '0x0085' = "|x"; '0x2028' = "|l"; '0x2029' = "|p" }
    $replace.GetEnumerator() | ForEach-Object {  $errMsg = $errMsg -replace $_.Key, $_.Value }
    write-host "##teamcity[message text='build failed: $errMsg' errorDetails='$errMsg' status='ERROR']"
    throw $errMsg
  }
}


# Install psake
$psakeVersion = "4.2.0.1"
& $scriptPath\src\.nuget\NuGet.exe install psake -OutputDirectory buildframework -Version $psakeVersion

remove-module psake -ea 'SilentlyContinue'
Import-Module (join-path $scriptPath ".\buildframework\psake.$psakeVersion\tools\psake.psm1")

if (-not(test-path $buildFile)) {
    $buildFile = (join-path $scriptPath $buildFile)
}

WriteExtraArguments
Build "3.5" "Init"
Build "3.5" $task
Build "4.0" $task