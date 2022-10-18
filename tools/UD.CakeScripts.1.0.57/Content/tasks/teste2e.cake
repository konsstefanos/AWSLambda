Task("Test-E2E")
    .Description("Runs end-to-end tests.")
    .IsDependentOn("Local-AWS")
    .Does(() =>
    {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = BuildEngine.Configuration,
            NoRestore = true,
            NoBuild = true,
            Loggers = new [] { "trx" },
            ResultsDirectory = BuildEngine.ArtifactsDirectory
        };

        var projectFiles = GetFiles((string)(BuildEngine.E2EDirectory + File("./**/*.csproj")));
        foreach(var file in projectFiles)
        {
            Information("Testing '{0}'...", file);
            DotNetCoreTest(file.FullPath, settings);
            Information("'{0}' has been tested.", file);
        }
    });
