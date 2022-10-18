Task("Publish")
    .Description("Publish the Projects.")
    .Does(() =>
    {
        foreach (var project in GetFiles((string)(BuildEngine.SrcDirectory + File("./**/*.csproj"))))
        {
            var projectName = GetProjectName(project.FullPath);
            var targetFramework = XmlPeek(project.FullPath, "//PropertyGroup/TargetFramework", new XmlPeekSettings { SuppressWarning = true });
            var publishReadyToRun = XmlPeek(project.FullPath, "//PropertyGroup/PublishReadyToRun", new XmlPeekSettings { SuppressWarning = true }) == "true" ? true : false;

            if (targetFramework != null && targetFramework.StartsWith("netstandard"))
            {
                Information("Skipping Library Project '{0}'.", projectName);
                continue;
            }

            Information("Publishing '{0}'...", projectName);
            if (FileExists(project.GetDirectory().CombineWithFilePath("Dockerfile")))
            {
                // Build Docker Image
                if (!BuildEngine.DockerImageNameOverrides.TryGetValue(projectName, out var projectId))
                {
                    projectId = PascalToKebabCase(projectName);
                }
                var assemblyVersion = XmlPeek(project.FullPath, "//PropertyGroup/Version") ?? "1.0.0";
                var gitSha = GetGitSha();
                var version = $"{assemblyVersion}-g{gitSha}";
                var settings = new DockerImageBuildSettings
                {
                    File = project.GetDirectory().CombineWithFilePath("Dockerfile").FullPath,
                    BuildArg = new [] {
                        $"VERSION={version}"
                    },
                    Tag = new [] {
                        $"unidays/{projectId}:{version}",
                        $"unidays/{projectId}:latest"
                    }
                };
                DockerBuild(settings, ".");
                if (!string.IsNullOrWhiteSpace(EnvironmentVariable("ECR_REGISTRY_URL")))
                {
                    var repositoryTag = $"{new Uri(EnvironmentVariable("ECR_REGISTRY_URL")).Host}/{settings.Tag[0]}";
                    DockerTag(settings.Tag[0], repositoryTag);
                    DockerPush(repositoryTag);
                    FileWriteText(BuildEngine.PublishDirectory + File(projectName), repositoryTag);
                }
            }
            else
            {
                // Assume Lambda
                var outputDirectory = BuildEngine.PublishDirectory + Directory(projectName);

                var msBuildSettings = new DotNetCoreMSBuildSettings
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
                    Verbosity = BuildEngine.DotNetCoreVerbosity
                };

                var settings = new DotNetCorePublishSettings
                {
                    Configuration = BuildEngine.Configuration,
                    MSBuildSettings = msBuildSettings,
                    NoRestore = publishReadyToRun ? false : true,
                    OutputDirectory = outputDirectory,
                    Runtime = "linux-x64",
                    SelfContained = false,
                    Verbosity = BuildEngine.DotNetCoreVerbosity
                };
                DotNetCorePublish(project.FullPath, settings);
            }

            if (publishReadyToRun)
                Information("Published '{0}' as Ready-To-Run", projectName);
            else
                Information("Published '{0}'.", projectName);
        }
    });
