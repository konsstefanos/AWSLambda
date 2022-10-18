using System.Threading;
using System.Threading.Tasks;

Task("Test-Unit")
    .Description("Runs all unit tests.")
    .Does(() =>
    {
        TestInParallel(GetFiles((string)(BuildEngine.TestDirectory + File("./**/*.csproj"))));
    });

public void TestInParallel(
    FilePathCollection files,
    int maxDegreeOfParallelism = -1,
    CancellationToken cancellationToken = default(CancellationToken))
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = BuildEngine.Configuration,
        NoRestore = true,
        NoBuild = true,
        Loggers = new [] { "trx" },
        ResultsDirectory = BuildEngine.ArtifactsDirectory
    };

    var actions = new List<Action>();
    foreach (var file in files) {
        actions.Add(() => {
            Information("Testing '{0}'...", file);
            DotNetCoreTest(file.FullPath, settings);
            Information("'{0}' has been tested.", file);
        });
    }

    var options = new ParallelOptions {
        MaxDegreeOfParallelism = maxDegreeOfParallelism,
        CancellationToken = cancellationToken
    };

    Parallel.Invoke(options, actions.ToArray());
}
