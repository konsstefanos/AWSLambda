namespace AWSLambda
{
    public interface ILambdaEntryPoint
    {
        Task<string> Handler(string input);
    }
}