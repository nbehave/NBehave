param($installPath, $toolsPath, $package, $project)

$userProfileDir = get-content env:UserProfile
$pluginDir = "$userProfileDir\AppData\Local\JetBrains\ReSharper\v6.0\Plugins\NBehave"
New-Item $pluginDir -type directory
Copy-Item "$toolsPath\net35\ReSharper" -Destination $pluginDir -Recurse

Write-Host "You need to restart Visual Studio for the R# plugin to be loaded." -BackgroundColor Black -ForegroundColor Yellow

$file = $prj.ProjectItems.Item("Resharper.feature")
$file.Open()
$project.DTE.ItemOperations.OpenFile($file.Document.FullName, [EnvDTE.Constants]::vsDocumentKindText)
