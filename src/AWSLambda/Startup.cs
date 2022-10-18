using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda
{
    public class Startup
    {
        private readonly IConfigurationRoot Configuration;

        public Startup()
        {
            Configuration = new ConfigurationBuilder() // ConfigurationBuilder() method requires Microsoft.Extensions.Configuration NuGet package
                .SetBasePath(Directory.GetCurrentDirectory())  // SetBasePath() method requires Microsoft.Extensions.Configuration.FileExtensions NuGet package
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // AddJsonFile() method requires Microsoft.Extensions.Configuration.Json NuGet package
                .AddEnvironmentVariables() // AddEnvironmentVariables() method requires Microsoft.Extensions.Configuration.EnvironmentVariables NuGet package
                .Build();
        }

        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection(); // ServiceCollection require Microsoft.Extensions.DependencyInjection NuGet package

            ConfigureLoggingAndConfigurations(services);

            ConfigureApplicationServices(services);

            IServiceProvider provider = services.BuildServiceProvider();

            return provider;
        }


        private void ConfigureLoggingAndConfigurations(ServiceCollection services)
        {

            // Add configuration service
            services.AddSingleton<IConfiguration>(Configuration);

            // Add logging service
            services.AddLogging(loggingBuilder =>  // AddLogging() requires Microsoft.Extensions.Logging NuGet package
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole(); // AddConsole() requires Microsoft.Extensions.Logging.Console NuGet package
            });
        }

        private void ConfigureApplicationServices(ServiceCollection services)
        {
            #region AWS SDK setup
            // Get the AWS profile information from configuration providers
            AWSOptions awsOptions = Configuration.GetAWSOptions();

            // Configure AWS service clients to use these credentials
            services.AddDefaultAWSOptions(awsOptions);

            // These AWS service clients will be singleton by default
            services.AddAWSService<IAmazonS3>();
            #endregion

            services.AddSingleton<ILambdaEntryPoint, LambdaEntryPoint>();
        }
    }
}
