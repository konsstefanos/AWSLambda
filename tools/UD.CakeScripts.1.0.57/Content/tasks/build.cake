Task("Build")
    .Description("Builds all the different parts of the project.")
    .Does(() =>
    {
        var msBuildSettings = new DotNetCoreMSBuildSettings
        {
            TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
            Verbosity = BuildEngine.DotNetCoreVerbosity
        };

        var settings = new DotNetCoreBuildSettings
        {
            Configuration = BuildEngine.Configuration,
            MSBuildSettings = msBuildSettings,
            NoRestore = true
        };

        foreach(var solution in BuildEngine.Solutions)
        {
            Information("Building '{0}'...", solution);
            DotNetCoreBuild(solution.FullPath, settings);
            Information("'{0}' has been built.", solution);
        }
    });
