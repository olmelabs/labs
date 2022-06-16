using ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var authenticationProviderKey = "CognitoTestKey";

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
        s.AddHttpClient();
        s.AddAuthentication()
            .AddJwtBearer(authenticationProviderKey, configureOptions: null!);
        s.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        s.AddOcelot();
        s.Decorate<IClaimsAuthorizer, ClaimAuthorizerDecorator>();
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
