#r @"../tools/fake/FakeLib.dll"

open Fake

// Project to compile
let projName = "xcite.tean"
let csproj = "../" + projName + "/"+ projName + ".csproj"

// Target directories
let buildDir = "../.binaries/"
let targetDir = buildDir + projName + "/"

// Define targets
Target "Clean" (fun _ ->
    CleanDir targetDir
)

Target "Default" (fun _ -> 
    DotNetCli.Pack (fun p -> 
                        { p with 
                            Configuration = "Release"
                            OutputPath = targetDir 
                            Project = csproj 
                        }
                   )
)

// Define dependencies
"Clean"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"