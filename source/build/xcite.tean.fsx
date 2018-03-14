#r @"../tools/fake/FakeLib.dll"

open Fake

// Project to compile
let projName = "xcite.tean"
let csproj = "../" + projName + "/"+ projName + ".csproj"

// Target directories
let buildDir = "../.binaries/"
let targetDir = buildDir + projName + "/"

// Define dependencies
"Clean"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"