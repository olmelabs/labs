using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;

namespace AuthApi.Extensions
{
    public static class Extensions
    {
        public static AWSOptions GetAwsOptionsFromConfiguration(IConfiguration configuration)
        {
            var awsOptions = configuration.GetAWSOptions();
            awsOptions.Credentials = new BasicAWSCredentials(
                configuration
                    .GetSection("AWS").GetValue<string>("AccessKey"),
                configuration
                    .GetSection("AWS").GetValue<string>("SecretKey"));
            return awsOptions;
        }
    }
}
