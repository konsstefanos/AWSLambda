Task("Package")
    .Description("This is the task which will run if target Package is passed in.")
    .IsDependentOn("Test")
    .IsDependentOn("Publish")
    .Does(() => { Information("Package target ran."); });
