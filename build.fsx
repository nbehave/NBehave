// include Fake libs
#I "buildframework/FAKE/tools"
#r "FakeLib.dll"

#load "buildProperties.fsx"

open Properties
open Fake
open Fake.RestorePackageHelper
open System
open System.Diagnostics
open System.IO
open System.Net
open System.Text.RegularExpressions
open System.Xml

// Targets
Target "Clean" (fun _ ->
  killMSBuild()
  CleanDir buildDir
  CleanDirs [testReportsDir; artifactsDir]
)

Target "Restore nuget packages" (fun _ ->
  !! (sourceDir + "/**/packages.config")
  |> Seq.iter (RestorePackage (fun p ->
                { p with
                    ToolPath = nugetExe
                    OutputPath = nugetPackageDir
                }))
)

Target "InstallNUnitRunners" (fun _ ->
  let settings = { RestoreSinglePackageDefaults with
                    Version = Some <| Version(2, 6, 2)
                    ExcludeVersion = true
                    ToolPath = nugetExe
                    OutputPath = nugetPackageDir
                 }
  RestorePackageId (fun _ -> settings) "nunit.runners"
)

Target "Set teamcity buildnumber" (fun _ ->
  SetBuildNumber nugetVersionNumber
)

let ReSharperSdkInstall version (urlToSdk:string) =
  let sdkPath = Path.Combine(rootDir, "lib", "ReSharper", version)
  if (Directory.Exists(Path.Combine(sdkPath, "Targets"))) && (Directory.Exists(Path.Combine(sdkPath, "Bin"))) then
    trace (sprintf "R# SDK %s already installed." version)
  else
    // download SDK
    trace (sprintf "Downloading R# SDK %s ..." version)
    let wc = new WebClient()
    let downloadedFile = rootDir + "RSharperSDK-" + version + ".zip"
    wc.DownloadFile(urlToSdk, downloadedFile)
    Unzip (Path.Combine(rootDir, "lib", "ReSharper", version)) downloadedFile
    File.Delete(downloadedFile)

let ReSharperSdkPath version =
  // Search rootDir + "\lib" efter Plugin.Common.Targets och fixa alla
  let sdkPath = Path.Combine(rootDir, "lib", "ReSharper", version)

  let fileName = Path.Combine(sdkPath, "Targets", "Plugin.Common.Targets")
  let xml = XmlDocument()
  xml.Load(fileName)
  let nsmgr = XmlNamespaceManager(xml.NameTable)
  nsmgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003")
  //multiple nodes?
  let node = xml.SelectSingleNode("//x:ReSharperSdk", nsmgr)
  node.InnerText <- sdkPath
  xml.Save(fileName)

Target "Install R# 7 SDK" (fun _ ->
  let version = "7.1.96"
  ReSharperSdkInstall version ("http://download.jetbrains.com/resharper/ReSharperSDK-" + version + ".zip")
  ReSharperSdkPath version
)

Target "Install R# 8 SDK" (fun _ ->
  let version = "8.0.1086"
  ReSharperSdkInstall version ("http://download.jetbrains.com/resharper/ReSharperSDK-" + version + ".zip")
  ReSharperSdkPath version
)

Target "AssemblyInfo" (fun _ ->
  let fileName = (Path.Combine(sourceDir, "CommonAssemblyInfo.cs")) |> FullName
  ReplaceAssemblyInfoVersions (fun p ->
    { p with
        AssemblyVersion = assemblyVersion
        AssemblyFileVersion = assemblyVersion
        AssemblyInformationalVersion = assemblyInfoVersion
        OutputFileName = fileName
    })

  //Version for source.extension.vsixmanifest
  let vsixFile = Path.Combine(sourceDir, "NBehave.VS2010.Plugin", "source.extension.vsixmanifest") |> FullName
  let xml = new XmlDocument()
  xml.Load(vsixFile)
  let nsmgr = new XmlNamespaceManager(xml.NameTable)
  nsmgr.AddNamespace("x", @"http://schemas.microsoft.com/developer/vsx-schema/2010")
  let node = xml.SelectSingleNode(@"x:Vsix/x:Identifier/x:Version", nsmgr)
  node.InnerText <- assemblyVersion
  xml.Save(vsixFile)

  //Version for VS plugin package
  let package = Path.Combine(sourceDir, "NBehave.VS2010.Plugin", "NBehaveRunnerPackage.cs")
  let content = File.ReadAllLines(package)
  let changeVersion (str:string) =
    match str.Contains("[InstalledProductRegistration(") with
      | true  -> Regex.Replace(str, @"\d+\.\d+\.\d+", version)
      | false -> str
  let rows = content |> Seq.map changeVersion
  File.WriteAllLines(package, rows)
)

let outputPath frameworkVer =
  Path.Combine(buildDir, "Debug-" + frameworkVer, "NBehave") |> FullName

let compileAnyCpu frameworkVer =
  let sln = Path.Combine(sourceDir, "NBehave.sln")
  build (fun f ->
          { f with
              MaxCpuCount = Some (Some Environment.ProcessorCount)
              ToolsVersion = Some "4.0"
              Verbosity = Some MSBuildVerbosity.Minimal
              Properties =  [ ("Configuration", "AutomatedDebug-" + frameworkVer);
                              ("TargetFrameworkVersion", "v" + frameworkVer)
                            ]
              Targets = ["Rebuild"]
          }) sln

let compileConsolex86 frameworkVer =
  let sln = Path.Combine(sourceDir, "NBehave.Console", "NBehave.Console.csproj")
  let folder = outputPath frameworkVer
  build (fun f ->
          { f with
              MaxCpuCount = Some (Some Environment.ProcessorCount)
              ToolsVersion = Some "4.0"
              Verbosity = Some MSBuildVerbosity.Minimal
              Properties =  [ ("Configuration", "AutomatedDebug-" + frameworkVer + "-x86")
                              ("TargetFrameworkVersion", "v" + frameworkVer)
                              ("OutputPath", folder)
                            ]
              Targets = ["Rebuild"]
          }) sln
  Rename (Path.Combine(folder, "NBehave-Console-x86.exe")) (Path.Combine(folder, "NBehave-Console.exe"))

Target "Compile" (fun _ ->
  frameworkVersions |> Seq.iter (fun v ->
                                  compileConsolex86 v
                                  compileAnyCpu v)
)

Target "Test" (fun _ ->
  frameworkVersions
  |> Seq.iter(fun frameworkVer ->
                let testDir = (Path.Combine(buildDir, "Debug-" + frameworkVer, "UnitTests")) |> FullName
                let testDlls = !! (testDir + "/*.Specifications.dll")
                let xmlFile = (Path.Combine(testReportsDir, "UnitTests-" + frameworkVer + ".xml")) |> FullName
                NUnit (fun p ->
                        {p with
                          ToolPath = (Path.Combine(nugetPackageDir, "NUnit.Runners", "tools")) |> FullName
                          OutputFile = xmlFile
                          Framework = frameworkVer
                          ShowLabels = false
                        }) testDlls
                sendTeamCityNUnitImport xmlFile
              )
)

Target "VSPlugin artifact" (fun _ ->
  !! (buildDir + "/**/*.vsix")
  |> CopyFiles artifactsDir
)

let resharper_install_scripts (version:string) =
  let content = File.ReadAllText (Path.Combine(packageTemplateDir, "Resharper.scripts", "Install.ps1"))
  let out = Regex.Replace(content, @"\%version\%", version)
  File.WriteAllText (Path.Combine(buildDir, "Install.ps1"), out)
  let content = File.ReadAllText (Path.Combine(packageTemplateDir, "Resharper.scripts", "UnInstall.ps1"))
  let out = Regex.Replace(content, @"\%version\%", version)
  File.WriteAllText (Path.Combine(buildDir, "UnInstall.ps1"), out)

let nugetParams p =
  { p with
      ToolPath = nugetExe
      Version = nugetVersionNumber
      OutputPath = artifactsDir
      WorkingDir = artifactsDir
      AccessKey = nugetAccessKey
      NoDefaultExcludes = true
  }

Target "Create NuGet packages" (fun _ ->
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.runners.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.samples.nuspec"))
)

Target "Create NuGet packages for R#" (fun _ ->
  resharper_install_scripts "7.1"
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.Resharper71.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.Resharper711.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.Resharper712.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.Resharper80.nuspec"))
)

Target "Create NuGet packages Fluent" (fun _ ->
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["nunitVersion", nunitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.nunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["xunitVersion", xunitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.xunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["mbunitVersion", mbUnitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.mbunit.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.fluent.mstest.nuspec"))
)

Target "Create NuGet packages Spec" (fun _ ->
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["nunitVersion", nunitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.nunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["xunitVersion", xunitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.xunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = ["mbunitVersion", mbUnitVersion()] } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.mbunit.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.spec.mstest.nuspec"))
  ()
)

Target "Publish to NuGet" (fun _ ->
  let nugetParams p project =
    { p with
          WorkingDir = rootDir
          ToolPath = nugetExe
          AccessKey = nugetAccessKey
          OutputPath = artifactsDir
          Project = project
          Version = nugetVersionNumber
    }
  let publish pkg =
    let project = Path.GetFileName(pkg).Replace(nugetVersionNumber, "").Replace(Path.GetExtension(pkg), "").TrimEnd([|'.'|])
    //NuGetPublish (fun p -> nuGetParams p project) pkg
    ()
  let files = Directory.GetFiles(artifactsDir, "*.nupkg")
  files |> Array.iter publish
)

Target "Default" (fun _ -> ())

// Dependencies
"Clean"
  ==> "Set teamcity buildnumber"
  // ==> "Install R# 7 SDK"
  // ==> "Install R# 8 SDK"
  ==> "Restore nuget packages"
  ==> "InstallNUnitRunners"
  ==> "AssemblyInfo"
  ==> "Compile"
  ==> "Test"
  // ==> "VSPlugin artifact"
  ==> "Create NuGet packages"
  // ==> "Create NuGet packages for R#"
  ==> "Create NuGet packages Fluent"
  ==> "Create NuGet packages Spec"
  ==> "Default"

// Start build
Run <| getBuildParamOrDefault "target" "Default"
