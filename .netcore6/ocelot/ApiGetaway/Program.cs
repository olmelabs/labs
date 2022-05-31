using System.Text;
using ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        s.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(authenticationProviderKey, x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(hostingContext.Configuration["JWT:Secret2"])),
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,
                    ValidIssuer = hostingContext.Configuration["JWT:ValidIssuer"],

                    ValidateAudience = true,
                    ValidAudience = hostingContext.Configuration["JWT:ValidAudience"]
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
