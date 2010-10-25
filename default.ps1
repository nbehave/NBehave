Include "buildframework\versioning.ps1"
Include "buildframework\psake_ext.ps1"

Properties {
	$project_dir         = Split-Path $psake.build_script_file
	$solution_dir        = "$project_dir\src"
	$build_dir           = "$project_dir\build"
	$test_dir            = "$build_dir\Debug-$framework\UnitTests"
	$project_name        = "NBehave"
	$project_config      = "release"
	$version             = "x.x.x.x"
}

Task default -Depends RunBuild

Task RunBuild {
	Invoke-Task "Version"
	Invoke-Task "Compile"
	Invoke-Task "Test"
	Invoke-Task "Distribute"
	Invoke-Task "ILMerge"
}

Task Clean {
	if(Test-Path $build_dir)
	{
		Remove-Item $build_dir -Recurse
	}	
}

Task Version {
	$version = GetVersion("$project_dir\version")

	Generate-Assembly-Info true $project_name $project_name $project_name $version "$solution_dir\CommonAssemblyInfo.cs"
}

Task Compile {
	Exec { msbuild "src\NBehave.sln" /property:Configuration=AutomatedDebug-$framework /v:m /p:TargetFrameworkVersion=v$framework /toolsversion:$framework }
}

Task Test {
	new-item -path "${build_dir}" -name "test-reports" -type directory -ErrorAction SilentlyContinue
	
	$arguments = Get-Item "$test_dir\*Specifications*.dll"
	Exec { tools\nunit\nunit-console-x86.exe $arguments /xml:${build_dir}\test-reports\UnitTests-$framework.xml}
}

Task Distribute -depends DistributeVSPlugin, DistributeBinaries

Task DistributeVSPlugin -precondition { return $framework -eq "4.0" }{
	
	new-item -path "${build_dir}" -name "plugin" -type directory -ErrorAction SilentlyContinue
	$destination = "$build_dir\plugin\"
	$source = "$build_dir\Debug-$framework\VSPlugin"
	
	Get-ChildItem "$source\*.*" -Include *.dll, *.vsixmanifest, *.pkgdef | Copy-Item -Destination $destination
	
	$namespaces = @{ "vsx" = "http://schemas.microsoft.com/developer/vsx-schema/2010"}
	$xpath = "/vsx:Vsix/vsx:Identifier/vsx:"
	xmlPoke "$destination\extension.vsixmanifest" $xpath"InstalledByMsi" "true" $namespaces
	xmlPoke "$destination\extension.vsixmanifest" $xpath"Version" $version $namespaces
}

Task DistributeBinaries {
	new-item -path "${build_dir}" -name "dist" -type directory -ErrorAction SilentlyContinue
	new-item -path "${build_dir}" -name "dist\v$framework" -type directory -ErrorAction SilentlyContinue

	$destination = "$build_dir\dist\v$framework"
	$source = "$build_dir\Debug-$framework\NBehave"
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude *.pdb, *Microsoft* | Copy-Item -Destination $destination
}

Task BuildInstaller {
	Exec { tools\nsis\makensis.exe /DVERSION=$version "$solution_dir\Installer\NBehave.nsi"}
}

Task ILMerge {
	$key = "$solution_dir\NBehave.snk"
	$directory = "$build_dir\dist\v$framework"
	$name = "NBehave.Narrator.Framework"
	$assemblies = @("$directory\gherkin.dll", "$directory\NBehave.Spec.Framework.dll")
	
	ilmerge $key $directory $name $assemblies "dll"
}