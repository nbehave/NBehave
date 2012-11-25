param($frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\ExtraArgs.ps1"
Include ".\buildframework\psake_ext.ps1"

Task Init -depends ArtifactsClean
Task Default -depends Artifacts

task ArtifactsClean {
  $dirs = @($installerDir, $artifactsDir, "$buildDir\Examples")
  $dirs | ForEach-Object {
    if ($true -eq (Test-Path $_)) { clearDir $_ }
    else { New-Item $_ -type directory }
  }
}

Task Artifacts -depends ArtifactsClean {
  if ($true -eq (Test-Path "$buildDir\Debug-$frameworkVersion\VSPlugin\")) {
    Copy-Item "$buildDir\Debug-$frameworkVersion\VSPlugin\*.vsix" "$artifactsDir"
  }
}