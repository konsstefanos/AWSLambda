Task("Test-All")
    .Description("Runs all tests in solution.")
    .IsDependentOn("Package")
    .IsDependentOn("Test-E2E")
    .Does(() => { Information("Test-All target ran."); })
    .Finally(CleanupEnvironment);