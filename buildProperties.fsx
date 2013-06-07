module Properties
  #I "buildframework/FAKE/tools"
  #r "FakeLib.dll"

  open Fake
  open System
  open System.Diagnostics
  open System.IO

  // params from teamcity
  let buildNumber       = getBuildParamOrDefault "buildNumber" "0"
  let buildTag          = getBuildParamOrDefault "buildTag" "devlocal" // For release, set this to "release"
  let frameworkVersions = ["3.5"; "4.0"]

  let version             = "0.6.2"
  let assemblyVersion     = version + "." + buildNumber
  let assemblyInfoVersion = match buildTag.ToLower() with
                             | "release"  -> version + "." + buildNumber
                             | _          -> version + "-" + buildTag + buildNumber.PadLeft(4, '0')
  let nugetVersionNumber  = assemblyInfoVersion

  let rootDir             = "./" |> FullName
  let sourceDir           = (rootDir + "/src") |> FullName
  let packageTemplateDir  = (rootDir + "/nuget") |> FullName
  let toolsDir            = (rootDir + "/tools") |> FullName
  let buildDir            = (rootDir + "/build") |> FullName
  let testReportsDir      = (buildDir + "/test-reports") |> FullName
  let artifactsDir        = (buildDir + "/Artifacts") |> FullName
  let nugetExe            = (sourceDir + "/.nuget/NuGet.exe") |> FullName
  let nugetPackageDir     = (sourceDir + "/packages") |> FullName
  let nugetAccessKey      = getBuildParamOrDefault "nugetAccessKey" "NotSet"

  let getpackageFolder dirFilter runnerFilter =
    Directory.GetDirectories(nugetPackageDir, dirFilter, SearchOption.AllDirectories)
    |> Seq.map (fun d -> Path.GetFileName(d))
    |> Seq.filter (fun d -> d.ToLower().StartsWith(runnerFilter) = false)
    |> Seq.sort
    |> Seq.head

  let nunitVersion () =
    (getpackageFolder "NUnit*" "nunit.runners").ToLower().Replace("nunit.", "")

  let xunitVersion () =
    (getpackageFolder "xunit*" "_").ToLower().Replace("xunit.", "")

  let mbUnitVersion () =
    (getpackageFolder "mbunit*" "_").ToLower().Replace("mbunit.", "")
