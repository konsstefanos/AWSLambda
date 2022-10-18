using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

async Task EnsureTable(CreateTableRequest request, int port = 4569)
{
    using (var dynamo = new AmazonDynamoDBClient(BuildEngine.LocalAwsCredentials, new AmazonDynamoDBConfig
    {
        ServiceURL = $"http://localhost:{port}",
        AuthenticationRegion = BuildEngine.AwsRegionEndpoint.SystemName
    }))
    {
        try
        {
            await dynamo.DeleteTableAsync(request.TableName);
        }
        catch (Amazon.DynamoDBv2.Model.ResourceNotFoundException)
        {
        }
        var response = await dynamo.CreateTableAsync(request);
        var tableDescription = response.TableDescription;
        Information("DynamoDB Status: {0} ReadsPerSec: {1} WritesPerSec: {2} Table: {3}",
                    tableDescription.TableStatus,
                    tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                    tableDescription.ProvisionedThroughput.WriteCapacityUnits,
                    tableDescription.TableName);
    }
}
