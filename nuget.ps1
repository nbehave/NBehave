param($build = "", $frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -precondition { return $frameworkVersion -eq "4.0" }
task Default -depends  NuGet, NuGet-Spec, NuGet-Fluent -precondition { return $frameworkVersion -eq "4.0" }

task Clean {
	if ($false -eq (Test-Path $artifactsDir)) { New-Item $artifactsDir -type Directory }
	Remove-Item $artifactsDir\*.nuget
}

task NuGet -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.nuspec -Version $build -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.runners.nuspec -Version $build -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.samples.nuspec -Version $build -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper60.nuspec -Version $build -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper61.nuspec -Version $build -OutputDirectory $artifactsDir}
	#Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.VsPlugin.nuspec -Version $build -OutputDirectory $artifactsDir}
}

task NuGet-Fluent -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Fluent.NUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.XUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MbUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MsTest.nuspec"  -Version $build -OutputDirectory $artifactsDir }
}

task NuGet-Spec -depends Clean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Spec.NUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.XUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MbUnit.nuspec"  -Version $build -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MsTest.nuspec"  -Version $build -OutputDirectory $artifactsDir }
}
