 Task("Local-AWS")
    .Description("Adds dependent resources into a local AWS substitute (localstack)")
    .WithCriteria(() => !string.IsNullOrEmpty(BuildEngine.DockerComposeFile))
    .IsDependentOn("Local-AWS-Up")
    .Does(async () =>
    {
        await BuildEngine.ConfigureLocalAws.Invoke();
    });
