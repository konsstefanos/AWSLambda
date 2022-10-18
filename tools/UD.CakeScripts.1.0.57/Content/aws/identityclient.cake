using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

IAmazonIdentityManagementService GetIdentityManagementClient()
{
    EnsureCredentials();
    return new AmazonIdentityManagementServiceClient(BuildEngine.RemoteAwsCredentials, BuildEngine.AwsRegionEndpoint);
}
