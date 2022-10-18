using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda
{
    public class LambdaEntryPoint : ILambdaEntryPoint
    {
        private readonly ILogger<LambdaEntryPoint> _logger;

        public LambdaEntryPoint(ILogger<LambdaEntryPoint> logger)
        {
            _logger = logger;
        }

        public async Task<string> Handler(string input)
        {
            _logger.LogInformation($"Handler invoked with following text \"{input}\"");

            return $"Hello {input}";
        }
    }
}
