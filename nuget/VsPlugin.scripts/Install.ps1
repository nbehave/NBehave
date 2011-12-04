param($installPath, $toolsPath, $package, $project)
dir "$toolsPath\net40\VsPlugin\"
& "$toolsPath\net40\VsPlugin\NBehave.VS2010.Plugin.vsix"
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")