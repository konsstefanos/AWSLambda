using Amazon.SQS;

async Task EnsureQueue(string queue, int port = 4576)
{
    var sqs = new AmazonSQSClient(BuildEngine.LocalAwsCredentials, new AmazonSQSConfig
    {
        ServiceURL = $"http://localhost:{port}",
        AuthenticationRegion = BuildEngine.AwsRegionEndpoint.SystemName
    });
    await sqs.CreateQueueAsync(queue);
    Information("SQS Queue: {0}", queue);
}
