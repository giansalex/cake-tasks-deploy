#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "Squirrel.Windows" 
#addin Cake.Squirrel
#tool Bumpy
#addin Cake.Bumpy

using System.Linq;
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./SquirrelApp/bin") + Directory(configuration);
var version = System.IO.File.ReadAllText("./deploy_version.txt").Trim();

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./SquirrelApp.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      var file = "./SquirrelApp/Properties/AssemblyInfo.cs";
      CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Product = "Giansalex APP",
        Version = version,
        FileVersion = version,
        InformationalVersion = version,
        Copyright = string.Format("Copyright (c) IMM CORP 2014 - {0}", DateTime.Now.Year)
    });
      // Use MSBuild
      MSBuild("./SquirrelApp.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./SquirrelApp.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    MSTest("./CustomLibrary.Tests/bin/" + configuration + "/*.Tests.dll");
});

Task("PackNuget")
    .IsDependentOn("Build")
	.Does(() => {
        var settings = new NuGetPackSettings();
        settings.Version = version;
		NuGetPack("./SquirrelApp/SquirrelApp.nuspec", settings);
	});

Task("Squirrel")
    .IsDependentOn("PackNuget")
	.Does(() => {
        var files = System.IO.Directory.GetFiles("./", "*.nupkg")
                    .OrderByDescending(f => f)
                    .ToArray();

		Squirrel(File(files[0]));
        Zip("./Releases", "publish.zip");
	});

Task("Publish")
    .IsDependentOn("Squirrel")
    .Does(() =>
{
    Information("Version increment:");
    BumpyIncrement(3);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
