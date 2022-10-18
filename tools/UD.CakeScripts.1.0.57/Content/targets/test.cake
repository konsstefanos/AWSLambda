Task("Test")
    .Description("Runs just the unit tests.")
    .IsDependentOn("Default")
    .IsDependentOn("Test-Unit")
    .Does(() => { Information("Test target ran."); });