param($installPath, $toolsPath, $package, $project)
& "$toolsPath\net40\VsPlugin\NBehave.VS2010.Plugin.vsix"
Write-Host "You need to restart Visual Studio for the plugin to be loaded." -BackgroundColor Black -ForegroundColor Yellow

$file = $project.ProjectItems.Item("VsPlugin.feature")
$file.Open()
$project.DTE.ItemOperations.OpenFile($file.Document.FullName, [EnvDTE.Constants]::vsDocumentKindText)
