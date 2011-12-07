param($installPath, $toolsPath, $package, $project)

Write-Host "To really uninstall this package you have to do this via the Extension Mananger in the Tools menu." -BackgroundColor Black -ForegroundColor Yellow
Write-Host "Press any key to uninstall package"  -BackgroundColor Black -ForegroundColor Yellow

$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")