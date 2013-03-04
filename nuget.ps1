param($frameworkVersion = "4.0")

Include ".\BuildProperties.ps1"
Include ".\ExtraArgs.ps1"
Include ".\buildframework\psake_ext.ps1"

task Init -precondition { return $frameworkVersion -eq "4.0" }
task Default -depends  NuGet, NuGet-Spec, NuGet-Fluent -precondition { return $frameworkVersion -eq "4.0" }

task NugetClean {
	Remove-Item $artifactsDir\*.nuget
}

function resharper_install_scripts($version) {
	$content = get-content ".\nuget\Resharper.scripts\Install.ps1"
	$out = $content -replace "%version%", "$version"
	set-content -Path ".\build\Install.ps1" -Value $out
	$content = get-content ".\nuget\Resharper.scripts\UnInstall.ps1"
	$out = $content -replace "%version%", "$version"
	set-content -Path ".\build\UnInstall.ps1" -Value $out
}

task NuGet -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.runners.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\nbehave.samples.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	resharper_install_scripts "6.0"
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper60.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	resharper_install_scripts "6.1"
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper611.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	resharper_install_scripts "7.0"
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper701.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	resharper_install_scripts "7.1"
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper71.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper711.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
	Exec { .\src\.nuget\nuget.exe pack nuget\NBehave.Resharper712.nuspec -Version $nugetVersionNumber -OutputDirectory $artifactsDir}
}

task NuGet-Fluent -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Fluent.NUnit.nuspec" -Version $nugetVersionNumber -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.XUnit.nuspec"   -Version $nugetVersionNumber -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MbUnit.nuspec"  -Version $nugetVersionNumber -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Fluent.MsTest.nuspec"  -Version $nugetVersionNumber -OutputDirectory $artifactsDir }
}

task NuGet-Spec -depends NugetClean -precondition{ return $frameworkVersion -eq "4.0" } {
	$ver = (nunitVersion)
	Exec { .\src\.nuget\nuget.exe pack ".\nuget\NBehave.Spec.NUnit.nuspec"  -Version $nugetVersionNumber -OutputDirectory $artifactsDir -Properties nunitVersion=$ver }
	$ver = (xunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.XUnit.nuspec"  -Version $nugetVersionNumber  -OutputDirectory $artifactsDir -Properties xunitVersion=$ver }
	$ver = (mbunitVersion)
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MbUnit.nuspec"  -Version $nugetVersionNumber -OutputDirectory $artifactsDir -Properties mbunitVersion=$ver }
	Exec { .\src\.nuget\nuget.exe pack "nuget\NBehave.Spec.MsTest.nuspec"  -Version $nugetVersionNumber -OutputDirectory $artifactsDir }
}

task Publish -precondition{ return $frameworkVersion -eq "4.0" } {
  Get-ChildItem -Path $artifactsDir -Filter *.nupkg | Select FullName | foreach-object {
  	$package = $_.FullName
  	Write-Host "Publishing package: $package"
 		Exec { src\.nuget\NuGet.exe "push" $package $apiKey }
 	}
}

