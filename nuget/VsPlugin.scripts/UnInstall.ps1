param($installPath, $toolsPath, $package, $project)

$userProfileDir = get-content env:UserProfile
$pluginDir = "$userProfileDir\AppData\Local\Microsoft\VisualStudio\10.0\Extensions\NBehave"
Remove-Item $pluginDir -Recurse -Force
