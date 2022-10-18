using System.Collections.Generic;
using Amazon;

public class BuildEngine
{
    public static string StackName { get; private set; }
    public static IDictionary<string, string> StackTags { get; private set; }

    public static ConvertableDirectoryPath SrcDirectory { get; private set;}
    public static ConvertableDirectoryPath PublishDirectory { get; private set;}
    public static ConvertableDirectoryPath ArtifactsDirectory { get; private set;}
    public static ConvertableDirectoryPath TestDirectory { get; private set;}
    public static ConvertableDirectoryPath E2EDirectory { get; private set;}

    public static FilePathCollection Solutions { get; private set; }
    public static string DockerComposeFile { get; private set; }
    public static Func<System.Threading.Tasks.Task> ConfigureLocalAws { get; private set; }
    public static Dictionary<string, string> DockerImageNameOverrides { get; private set; }
    public static Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity DotNetCoreVerbosity { get; private set; }

    public static string Target { get; private set; }
    public static RegionEndpoint AwsRegionEndpoint { get; private set; }
    public static string Profile { get; private set; }
    public static string StackType { get; private set; }
    public static string Configuration { get; private set; }

    public static AWSCredentials LocalAwsCredentials { get; private set;}
    public static AWSCredentials RemoteAwsCredentials { get; private set;}

    public static void Initialise(
        ICakeContext context,
        string stackName,
        IDictionary<string, string> stackTags = null,
        string src = "./src",
        string publish = "./publish",
        string artifacts = "./artifacts",
        string test = "./test",
        string e2e = "./e2e",
        FilePathCollection solutionFiles = null,
        string dockerComposeFile = "./support/docker-compose.yml",
        Func<Task> configureLocalAws = null,
        Dictionary<string, string> dockerImageNameOverrides = null,
        Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity verbosity = Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity.Quiet)
    {
        StackName = stackName;
        StackTags = stackTags ?? new Dictionary<string, string>();
        SrcDirectory = context.Directory(src);
        PublishDirectory =  context.Directory(publish);
        ArtifactsDirectory = context.Directory(artifacts);
        TestDirectory = context.Directory(test);
        E2EDirectory = context.Directory(e2e);

        Solutions = solutionFiles ?? context.GetFiles("./**/*.sln");
        DockerComposeFile = dockerComposeFile;
        ConfigureLocalAws = configureLocalAws ?? (() => System.Threading.Tasks.Task.CompletedTask);
        DockerImageNameOverrides = dockerImageNameOverrides ?? new Dictionary<string, string>();
        DotNetCoreVerbosity = verbosity;

        Target = context.Argument("target", "Default");
        AwsRegionEndpoint = RegionEndpoint.GetBySystemName(context.Argument("region", "eu-west-1"));
        Profile = context.Argument("profile", "dev");
        StackType = context.Argument("stack", "").ToLowerInvariant();
        Configuration = context.Argument("configuration", "Release");
    }

    public static void SetLocalCredentials(AWSCredentials credentials)
    {
        LocalAwsCredentials = credentials;
    }

    public static void SetRemoteCredentials(AWSCredentials credentials)
    {
        RemoteAwsCredentials = credentials;
    }
}

public void Go()
{
    if (string.IsNullOrEmpty(BuildEngine.Target))
        throw new Exception("Must call BuildEngine.Initialise before Go");

    Information("Using CLI options:");
    Information("Target:        {0}", BuildEngine.Target);
    Information("Configuration: {0}", BuildEngine.Configuration);
    Information("AWS Region:    {0}", BuildEngine.AwsRegionEndpoint.DisplayName);
    Information("AWS Profile:   {0}", BuildEngine.Profile);
    var stack = string.IsNullOrWhiteSpace(BuildEngine.StackType)
        ? BuildEngine.StackName
        : $"{BuildEngine.StackName}-{BuildEngine.StackType}";
    Information("AWS Stack:     {0}", stack);

    RunTarget(BuildEngine.Target);
}
