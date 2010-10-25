Include "buildframework\versioning.ps1"
Include "buildframework\psake_ext.ps1"
Include "buildframework\msbuild_ext.ps1"

Properties {
	$project_dir         = Split-Path $psake.build_script_file
	$solution_dir        = "$project_dir\src"
	$build_dir           = "$project_dir\build"
	$test_dir_3_5        = "$build_dir\Debug-3.5\UnitTests"
	$test_dir_4_0        = "$build_dir\Debug-4.0\UnitTests"
	$project_name        = "NBehave"
	$version             = "0.5.1.1"
	$project_config      = "release"
}

Task default -Depends RunBuild

Task RunBuild {
	Invoke-Task "Clean"
	Invoke-Task "Version"
	Invoke-Task "Compile"
	Invoke-Task "Test"
	Invoke-Task "Distribute"
	Invoke-Task "BuildInstaller"
}

Task Clean {
	if(Test-Path $build_dir)
	{
		Remove-Item $build_dir -Recurse
	}	
}

Task Version {
	Generate-Assembly-Info true $project_name $project_name $project_name $version "$solution_dir\CommonAssemblyInfo.cs"
}

Task Compile {
	BuildSolution "src\NBehave.sln" "3.5"
	BuildSolution "src\NBehave.sln" "4.0"
}

Task Test {
	new-item -path "${build_dir}" -name "test-reports" -type directory -ErrorAction SilentlyContinue
	
	$arguments = Get-Item "$test_dir_3_5\*Specifications*.dll"
	Exec { tools\nunit\nunit-console-x86.exe $arguments /xml:${build_dir}\test-reports\UnitTests-3.5.xml}
	
	$arguments = Get-Item "$test_dir_4_0\*Specifications*.dll"
	Exec { tools\nunit\nunit-console-x86.exe $arguments /xml:${build_dir}\test-reports\UnitTests-4.0.xml}
}

Task Distribute -depends DistributeVSPlugin, DistributeBinaries, DistributeExamples

Task DistributeVSPlugin {
	new-item -path "${build_dir}" -name "plugin" -type directory -ErrorAction SilentlyContinue
	$destination = "$build_dir\plugin\"
	$source = "$build_dir\Debug-4.0\VSPlugin"
	
	Get-ChildItem "$source\*.*" -Include *.dll, *.vsixmanifest, *.pkgdef | Copy-Item -Destination $destination
	
	$namespaces = @{ "vsx" = "http://schemas.microsoft.com/developer/vsx-schema/2010"}
	$xpath = "/vsx:Vsix/vsx:Identifier/vsx:"
	xmlPoke "$destination\extension.vsixmanifest" $xpath"InstalledByMsi" "true" $namespaces
	xmlPoke "$destination\extension.vsixmanifest" $xpath"Version" $version $namespaces
}

Task DistributeBinaries {
	new-item -path "${build_dir}" -name "dist" -type directory -ErrorAction SilentlyContinue
	new-item -path "${build_dir}" -name "dist\v3.5" -type directory -ErrorAction SilentlyContinue

	$destination = "$build_dir\dist\v3.5"
	$source = "$build_dir\Debug-3.5\NBehave"
	
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude *.pdb, *Microsoft* | Copy-Item -Destination $destination
	
	new-item -path "${build_dir}" -name "dist\v4.0" -type directory -ErrorAction SilentlyContinue
	
	$destination = "$build_dir\dist\v4.0"
	$source = "$build_dir\Debug-4.0\NBehave"
	Get-ChildItem "$source\*.*" -Include *NBehave*, *.dll -Exclude *.pdb, *Microsoft* | Copy-Item -Destination $destination
}

Task BuildInstaller {
	Exec { tools\nsis\makensis.exe /DVERSION=$version "$solution_dir\Installer\NBehave.nsi"}
}