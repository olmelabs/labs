/*
 // CODE FOR ROOMEX TOKEN VALIDATION

using ApiGateway;

using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var authenticationProviderKey = "TestKey";

new WebHostBuilder()
    .UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddJsonFile("ocelot.json")
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(AsmResolver).Assembly);
    })
    .ConfigureServices((hostingContext, s) =>
    {
        var key = hostingContext.Configuration["Jwt:Secret"];
        var bytes = Base64UrlEncoder.DecodeBytes(key);
        var signingKey = new SymmetricSecurityKey(bytes);

        s.AddAuthentication()
            .AddJwtBearer(authenticationProviderKey, x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,

                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,
                    ValidIssuer = "Rx.AuthServer",

                    ValidateAudience = true,
                    ValidAudience = "Rx.AudienceID"
                };
            });
        s.AddOcelot();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
            .ClearProviders()
            .AddSimpleConsole(options => { options.IncludeScopes = true; });
    })
    .Configure(app =>
    {
        app.UseOcelot().Wait();
    })
    .Build()
    .Run();
*/