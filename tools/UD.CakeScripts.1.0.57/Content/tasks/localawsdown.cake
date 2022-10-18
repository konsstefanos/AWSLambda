Task("Local-AWS-Down")
    .Description("Removes dependent resources into a local AWS substitute (localstack)")
    .WithCriteria(() => !string.IsNullOrEmpty(BuildEngine.DockerComposeFile))
    .Does(CleanupEnvironment);
