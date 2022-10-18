using Amazon.Kinesis;
using Amazon.Kinesis.Model;

async Task EnsureStream(CreateStreamRequest request, int port = 4568)
{
    using (var kinesis = new AmazonKinesisClient(BuildEngine.LocalAwsCredentials, new AmazonKinesisConfig
    {
        ServiceURL = $"http://localhost:{port}",
        AuthenticationRegion = BuildEngine.AwsRegionEndpoint.SystemName
    }))
    {
        try
        {
            await kinesis.DeleteStreamAsync(new DeleteStreamRequest
            {
                StreamName = request.StreamName
            });
            var describeRequest = new Amazon.Kinesis.Model.DescribeStreamRequest {StreamName = request.StreamName};
            do
            {
                await kinesis.DescribeStreamAsync(describeRequest);
            } while (true);
        }
        catch (Amazon.Kinesis.Model.ResourceNotFoundException)
        {
        }
        await kinesis.CreateStreamAsync(request);
        Information("Kinesis Stream: {0}", request.StreamName);
    }
}
