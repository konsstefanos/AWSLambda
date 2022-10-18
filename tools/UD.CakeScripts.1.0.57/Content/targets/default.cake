Task("Default")
    .Description("This is the default task which will run if no specific target is passed in.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");