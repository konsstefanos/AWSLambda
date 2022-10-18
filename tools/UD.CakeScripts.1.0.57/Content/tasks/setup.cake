Setup(ctx =>
{
    // Executed BEFORE the first task.
    EnsureDirectoryExists(BuildEngine.PublishDirectory);
    EnsureDirectoryExists(BuildEngine.ArtifactsDirectory);
    Information("Running tasks...");
});
