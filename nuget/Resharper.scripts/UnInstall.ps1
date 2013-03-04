param($installPath, $toolsPath, $package, $project)
$userProfileDir = get-content env:UserProfile

Write-Host "IMPORTANT! To uninstall this plugin you have to remove some files manually because Visual Studio and/or R# locks them." -BackgroundColor Black -ForegroundColor Yellow
Write-Host "After this script has finished, the package is uninstalled but not the plugin." -BackgroundColor Black -ForegroundColor Yellow
Write-Host "If you want to uninstall the plugin you have to manually delete the folder '$userProfileDir\AppData\Local\JetBrains\ReSharper\v%version%\Plugins\NBehave'" -BackgroundColor Black -ForegroundColor Yellow
Write-Host "Press any key to uninstall package"  -BackgroundColor Black -ForegroundColor Yellow

$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")