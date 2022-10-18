Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
    {
        var settings = new DotNetCoreRestoreSettings
        {
            DisableParallel = false,
            NoCache = true,
            Verbosity = BuildEngine.DotNetCoreVerbosity
        };

        foreach(var solution in BuildEngine.Solutions)
        {
            Information("Restoring NuGet packages for '{0}'...", solution);
            DotNetCoreRestore(solution.FullPath, settings);
            Information("NuGet packages restored for '{0}'.", solution);
        }
    });
