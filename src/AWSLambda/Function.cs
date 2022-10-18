using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda;

public class Function
{
    private readonly ILambdaEntryPoint _lambdaEntryPoint;

    public Function()
    {
        var startup = new Startup();
        IServiceProvider provider = startup.ConfigureServices();

        _lambdaEntryPoint = provider.GetRequiredService<ILambdaEntryPoint>();
    }
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        return await _lambdaEntryPoint.Handler(input);
    }
}
