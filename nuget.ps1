param($frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -precondition { return $frameworkVersion -eq "4.0" }
task Default -depends  NuGet, NuGet-Spec, NuGet-Fluent -precondition { return $frameworkVersion -eq "4.0" }

task NugetClean {
	Remove-Item $artifactsDir\*.nuget
}

task NuGet -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.runners.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.samples.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper60.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper61.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
	#Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.VsPlugin.nuspec -Version $buildNumber -OutputDirectory $artifactsDir}
}

task NuGet-Fluent -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Fluent.NUnit.nuspec" -Version $buildNumber -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.XUnit.nuspec"   -Version $buildNumber -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MbUnit.nuspec"  -Version $buildNumber -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MsTest.nuspec"  -Version $buildNumber -OutputDirectory $artifactsDir }
}

task NuGet-Spec -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Spec.NUnit.nuspec"  -Version $buildNumber -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.XUnit.nuspec"  -Version $buildNumber  -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MbUnit.nuspec"  -Version $buildNumber -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MsTest.nuspec"  -Version $buildNumber -OutputDirectory $artifactsDir }
}
