// include Fake libs
#I "./packages/FAKE/tools"

#r "FakeLib.dll"

open Fake
open Fake.Testing.NUnit3
open System
open System.IO

// params from teamcity
let buildNumber = getBuildParamOrDefault "buildNumber" "0"
let buildTag = getBuildParamOrDefault "buildTag" "devlocal" // For release, set this to "release"
let frameworkVersions = ["4.6"]

let version             = "0.7.0"
let assemblyVersion     = version + "." + buildNumber
let assemblyInfoVersion = match buildTag.ToLower() with
                           | "release"  -> version + "." + buildNumber
                           | _          -> version + "-" + buildTag + buildNumber.PadLeft(4, '0')
let nugetVersionNumber  = assemblyInfoVersion

let rootDir             = "./" |> FullName
let sourceDir           = (rootDir + "/src") |> FullName
let packageTemplateDir  = (rootDir + "/nuget") |> FullName
let buildDir            = (rootDir + "/build") |> FullName
let testReportsDir      = (buildDir + "/test-reports") |> FullName
let artifactsDir        = (buildDir + "/artifacts") |> FullName
let nugetPackageDir     = (rootDir + "/packages") |> FullName
let nugetExe = (nugetPackageDir + "/NuGet.CommandLine/tools/NuGet.exe") |> FullName
let nugetAccessKey      = getBuildParamOrDefault "nugetAccessKey" "NotSet"

let getpackageFolder dirFilter runnerFilter =
  Directory.GetDirectories(nugetPackageDir, dirFilter, SearchOption.AllDirectories)
  |> Seq.map (fun d -> Path.GetFileName(d))
  |> Seq.filter (fun d -> not <| d.ToLower().StartsWith(runnerFilter))
  |> Seq.sort
  |> Seq.head

let nunitVersion =
  File.ReadAllLines(Path.Combine(rootDir, "paket.dependencies"))
  |> Array.find(fun x -> x.Trim().ToLower().Contains("nunit"))
  |> fun x -> x.Trim().Split([|' '|]).[3]

let xunitVersion =
  File.ReadAllLines(Path.Combine(rootDir, "paket.dependencies"))
  |> Array.find(fun x -> x.Trim().ToLower().Contains("xunit"))
  |> fun x -> x.Trim().Split([|' '|]).[3]

let mbUnitVersion =
  File.ReadAllLines(Path.Combine(rootDir, "paket.dependencies"))
  |> Array.find(fun x -> x.Trim().ToLower().Contains("mbunit"))
  |> fun x -> x.Trim().Split([|' '|]).[3]

let gurkburkVersion =
  File.ReadAllLines(Path.Combine(rootDir, "paket.dependencies"))
  |> Array.find(fun x -> x.Trim().ToLower().Contains("gurkburk"))
  |> fun x -> x.Trim().Split([|' '|]).[3]

let dotnetcliVersion = "2.1.105"
let mutable dotnetExePath = "/Users/morganpersson/.local/share/dotnetcore/dotnet"

// let appReferences = !! "/**/*.csproj"
let appReferences = [
  "src/NBehave/NBehave.csproj"
  "src/NBehave.Console/NBehave.Console.csproj"
  "src/NBehave.Fluent.Framework/NBehave.Fluent.Framework.csproj"
  "src/NBehave.Spec.MbUnit/NBehave.Spec.MbUnit.csproj"
  "src/NBehave.Spec.MSTest/NBehave.Spec.MSTest.csproj"
  "src/NBehave.Spec.NUnit/NBehave.Spec.NUnit.csproj"
  "src/NBehave.Spec.Xunit/NBehave.Spec.Xunit.csproj"
]
let testReferences = [
  "src/NBehave.Specifications/NBehave.Specifications.csproj"
  "src/NBehave.Console.Specifications/NBehave.Console.Specifications.csproj"
  "src/NBehave.Fluent.Framework.Specifications/NBehave.Fluent.Framework.Specifications.csproj"
  "src/NBehave.Spec.MbUnit.Specifications/NBehave.Spec.MbUnit.Specifications.csproj"
  "src/NBehave.Spec.MSTest.Specifications/NBehave.Spec.MSTest.Specifications.csproj"
  "src/NBehave.Spec.NUnit.Specifications/NBehave.Spec.NUnit.Specifications.csproj"
  "src/NBehave.Spec.Xunit.Specifications/NBehave.Spec.Xunit.Specifications.csproj"
]


// --------------------------------------------------------------------------------------
// Helpers
// --------------------------------------------------------------------------------------
let deleteObjectDirs () =
  DeleteDirs (!! "src/**/obj")
  DeleteDirs (!! "src/**/bin")

let upToVersion v =
  let ver = Version(v)
  match (v.Split([|'.'|]).Length) with
  | 1 -> sprintf "%i" (ver.Major + 1)
  | 2 -> sprintf "%i.0" (ver.Major + 1)
  | 3 -> sprintf "%i.%i.0" ver.Major (ver.Minor + 1)
  | _ -> sprintf "%i.%i.0.0" ver.Major (ver.Minor + 1)

let run' timeout cmd args dir =
    if execProcess (fun info ->
        info.FileName <- cmd
        if not (String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout |> not then
        failwithf "Error while running '%s' with args: %s" cmd args

let run = run' System.TimeSpan.MaxValue

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

let compileAnyCpu frameworkVer outputPathPrefix proj =
  build (fun f ->
    { f with
        MaxCpuCount = Some (Some Environment.ProcessorCount)
        ToolsVersion = Some "14.0"
        Verbosity = Some MSBuildVerbosity.Minimal
        Properties =  [ ("Configuration", "Debug");
                        ("TargetFrameworkVersion", "v" + frameworkVer)
                        ("OutputPath", Path.Combine(buildDir, outputPathPrefix + frameworkVer))
                      ]
        Targets = ["Build"]
    }) proj

// --------------------------------------------------------------------------------------
// Targets
// --------------------------------------------------------------------------------------


Target "Clean Artifacts" (fun _ ->
  CleanDirs [artifactsDir]
)

Target "Clean" (fun _ ->
  killMSBuild()
  deleteObjectDirs ()
  DeleteDir buildDir
  CleanDir testReportsDir
)

// Target "InstallDotNetCLI" (fun _ ->
//     dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
// )

// Target "Restore" (fun _ ->
//     appReferences
//     |> Seq.iter (fun p ->
//         let dir = System.IO.Path.GetDirectoryName p
//     //     runDotnet dir "restore"
//         DotNetCli.Restore (fun p -> { p with WorkingDir = dir } )
//     )
// )

// Target "Build" (fun _ ->
//     appReferences
//     |> Seq.iter (fun p ->
//         let dir = System.IO.Path.GetDirectoryName p
//         printfn "--DIR-- '%s'" dir
//         runDotnet dir "build"
//     )
// )

Target "Set teamcity buildnumber" (fun _ ->
  SetBuildNumber nugetVersionNumber
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
)

Target "Compile" (fun _ ->
  frameworkVersions
  |> Seq.iter (fun v ->
    appReferences
    |> Seq.iter (fun p ->
        // let dir = System.IO.Path.GetDirectoryName p
        // printfn "--DIR-- '%s'" dir
        // runDotnet dir "build"
        compileAnyCpu v "" p
    )
  )
)

Target "Compile Tests" (fun _ ->
  frameworkVersions
  |> Seq.iter (fun v ->
    testReferences
    |> Seq.iter (fun p ->
        compileAnyCpu v "specs_" p
    )
  )
)


Target "Test" (fun _ ->
  frameworkVersions
  |> Seq.iter(fun frameworkVer ->
        let testDir = (Path.Combine(buildDir, "specs_" + frameworkVer)) |> FullName
        let testDlls = !! (testDir + "/*.Specifications.dll")
        // let xmlFile = (Path.Combine(testReportsDir, "specs_" + frameworkVer + ".xml")) |> FullName
        NUnit3 (fun p ->
          {p with
            ToolPath = (Path.Combine(nugetPackageDir, "NUnit.ConsoleRunner", "tools", "nunit3-console.exe")) |> FullName
            WorkingDir = testDir
            // OutputFile = xmlFile
            // OutputDir = testReportsDir
            Framework = NUnit3Runtime.Default //frameworkVer
            Labels = LabelsLevel.On
            TimeOut = TimeSpan(0, 5, 0)
            TeamCity = buildServer = TeamCity
          }) testDlls
        // sendTeamCityNUnitImport xmlFile
      )
)

let nugetParams p =
  { p with
      ToolPath = nugetExe
      Version = nugetVersionNumber
      OutputPath = artifactsDir
      WorkingDir = artifactsDir
      AccessKey = nugetAccessKey
      NoDefaultExcludes = true
  }

Target "Package" (fun _ ->
  let properties = [
    "nunitMinVersion", nunitVersion; "nunitMaxVersion", upToVersion nunitVersion;
    "xunitMinVersion", xunitVersion; "xunitMaxVersion", upToVersion xunitVersion;
    "mbunitMinVersion", mbUnitVersion; "mbunitMaxVersion", upToVersion mbUnitVersion;
    "gurkburkMinVersion", gurkburkVersion; "gurkburkMaxVersion", upToVersion gurkburkVersion
  ]

  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.runners.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.samples.nuspec"))

  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.nunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.xunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.fluent.mbunit.nuspec"))
  NuGetPack nugetParams (Path.Combine(packageTemplateDir, "nbehave.fluent.mstest.nuspec"))

  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.nunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.xunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.mbunit.nuspec"))
  NuGetPack (fun p -> { (nugetParams p) with Properties = properties } )
            (Path.Combine(packageTemplateDir, "nbehave.spec.mstest.nuspec"))
)

Target "Publish" (fun _ ->
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
    printfn "%s / %s" project nugetVersionNumber
    NuGetPublish (fun p -> nugetParams p project) //pkg
    ()
  let files = Directory.GetFiles(artifactsDir, "*.nupkg")
  files |> Array.iter publish
)


// Dependencies
"Clean Artifacts"
  ==> "Clean"
  ==> "AssemblyInfo"
  ==> "Set teamcity buildnumber"
  // ==> "Restore"
  // ==> "Build"
  ==> "Compile"
  ==> "Compile Tests"
  ==> "Test"

"Clean Artifacts"
  ==> "Package"
  ==> "Publish"


// Start build
RunTargetOrDefault "Test"
