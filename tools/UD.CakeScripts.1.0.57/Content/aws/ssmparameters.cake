using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

async Task EnsureParameter(PutParameterRequest request, int port = 4583)
{
    using (var ssm = new AmazonSimpleSystemsManagementClient(BuildEngine.LocalAwsCredentials, new AmazonSimpleSystemsManagementConfig
    {
        ServiceURL = $"http://localhost:{port}",
        AuthenticationRegion = BuildEngine.AwsRegionEndpoint.SystemName
    }))
    {
        var response = await ssm.PutParameterAsync(request);
        Information("SSM Parameter: {0} \t Version: {1}",
                    request.Name,
                    response.Version);
    }
}
