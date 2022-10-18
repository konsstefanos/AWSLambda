Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
    {
        foreach(var solution in BuildEngine.Solutions)
        {
            Information("Cleaning {0}", solution.FullPath);
            CleanDirectories(solution.FullPath + "/**/bin/" + BuildEngine.Configuration);
            CleanDirectories(solution.FullPath + "/**/obj/" + BuildEngine.Configuration);
            Information("{0} was clean.", solution.FullPath);
        }

        CleanDirectory(BuildEngine.PublishDirectory);
        CleanDirectory(BuildEngine.ArtifactsDirectory);
    });
