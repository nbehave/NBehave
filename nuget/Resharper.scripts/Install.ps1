param($installPath, $toolsPath, $package, $project)

$userProfileDir = get-content env:UserProfile
$pluginDir = "$userProfileDir\AppData\Local\JetBrains\ReSharper\v%version%\Plugins\NBehave"
New-Item $pluginDir -type directory
Copy-Item "$toolsPath\ReSharper" -Destination $pluginDir -Recurse

Write-Host "You need to restart Visual Studio for the R# plugin to be loaded." -BackgroundColor Black -ForegroundColor Yellow

#$file = $project.ProjectItems.Item("Resharper.feature")
#$file.Open()
#$project.DTE.ItemOperations.OpenFile($file.Document.FullName, [EnvDTE.Constants]::vsDocumentKindText)
