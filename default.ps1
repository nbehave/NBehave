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
	#Invoke-Task "Test"
	#Invoke-Task "Distribute"
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
	
	$arguments = GetTestAssemblies $test_dir_3_5
	Exec { tools\nunit\nunit-console-x86.exe $arguments /xml:${build_dir}\test-reports\UnitTests-3.5.xml}
	
	$arguments = GetTestAssemblies $test_dir_4_0
	Exec { tools\nunit\nunit-console-x86.exe $arguments /xml:${build_dir}\test-reports\UnitTests-4.0.xml}
}

function GetTestAssemblies
{
	param([string]$directory)
	
	return Get-Item "$directory\*Specifications*.dll"
}
