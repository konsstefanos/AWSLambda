using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;

IAmazonCloudFormation GetCloudFormationClient()
{
    EnsureCredentials();
    return new AmazonCloudFormationClient(BuildEngine.RemoteAwsCredentials, BuildEngine.AwsRegionEndpoint);
}
