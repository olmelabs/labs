using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

//https://stackoverflow.com/questions/46962770/get-a-service-in-a-iservicecollection-extension

namespace ApiGateway
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration config;

        public ConfigureJwtBearerOptions(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.config = config;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            {
                var cognitoIssuer = $"https://cognito-idp.{config["AWS:Region"]}.amazonaws.com/{config["AWS:UserPoolId"]}";
                var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
                var validAudience = config["AWS:AppClientId"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                    {
                        var json = httpClientFactory.CreateClient().GetStringAsync(jwtKeySetUrl).GetAwaiter().GetResult();
                        var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json)?.Keys;
                        return keys;
                    },
                    ValidIssuer = cognitoIssuer,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,

                    ValidAudience = validAudience,
                    ValidateAudience = true,

                    ValidateLifetime = true
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }
    }
}
