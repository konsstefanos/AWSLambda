Task("Local-AWS-Up")
    .Description("Adds dependent resources into a local AWS substitute (localstack)")
    .WithCriteria(() => !string.IsNullOrEmpty(BuildEngine.DockerComposeFile))
    .Does(() =>
    {
        Information("Ensuring AWS Profile is defined");
        EnsureAwsProfile();

        Information("Ensuring Support Stack is running");
        DockerComposeUp(new DockerComposeUpSettings
        {
            Files = new [] { BuildEngine.DockerComposeFile },
            DetachedMode = true,
            RemoveOrphans = true
        });
    });
