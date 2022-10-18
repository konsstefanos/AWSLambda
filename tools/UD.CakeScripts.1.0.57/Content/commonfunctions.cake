using System.Text.RegularExpressions;

void CleanupEnvironment() {
    Information("Shutting down Support Stack");
    DockerComposeDown(new DockerComposeDownSettings {
        Files = new [] { BuildEngine.DockerComposeFile }
    });
}

string GetProjectName(string project)
{
    return project
        .Split(new [] {'/'}, StringSplitOptions.RemoveEmptyEntries)
        .Last()
        .Replace(".csproj", string.Empty);
}

string PascalToKebabCase(string value)
{
    if (string.IsNullOrEmpty(value))
        return value;

    return Regex.Replace(
        value,
        "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
        "-$1",
        RegexOptions.Compiled)
        .Trim()
        .ToLower();
}

string GetGitSha()
{
    using (var process = StartAndReturnProcess("git", new ProcessSettings
    {
        Arguments = "rev-parse --short HEAD",
        RedirectStandardOutput = true
    }))
    {
        process.WaitForExit();
        return process.GetStandardOutput().First();
    }
}
