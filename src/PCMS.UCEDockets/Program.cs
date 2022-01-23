namespace PCMS.UCEDockets;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Hosting;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../config/");

        builder.Configuration
            .AddJsonFile(Path.Combine(configPath, "config.json"), optional: false)
            .AddJsonFile(Path.Combine(configPath, "config.secrets.json"), optional: true)
            .AddEnvironmentVariables();

        var options = new UCEDocketsOptions();
        builder.Configuration.Bind(UCEDocketsOptions.Section, options);

        builder.Services.Configure<HostOptions>((hostOptions) =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });

        builder.Services.AddOptions<UCEDocketsOptions>()
            .Bind(builder.Configuration.GetSection(UCEDocketsOptions.Section));

        builder.Services.AddMvcCore().AddXmlSerializerFormatters();

        builder.Services.AddControllers();

        builder.Services.AddHostedService<Services.Monitoring>();
        builder.Services.AddHostedService<Services.Scheduler>();

        builder.Services.AddTransient<Modules.SFTP>();
        builder.Services.AddTransient<Modules.Importer>();
        
        switch (options.EFDatabaseProvider?.ToLower())
        {
            case "sqlite": 
                builder.Services.AddDbContext<Entities.UCEDocketsContext, Entities.SqliteUCEDocketsContext>();
                break;
            case "sqlserver": 
                builder.Services.AddDbContext<Entities.UCEDocketsContext, Entities.SqlServerUCEDocketsContext>();
                break;
            case "none":
                break;
            default:
                throw new Exception($"Unknown EFDatabaseProvider: {options.EFDatabaseProvider}");
        }
            
        builder.Services.AddLogging();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "PCMS.UCEDockets API",
                Description = "PCMS.UCEDockets cache server to integrate with the API described here: https://portal.nycourts.gov/UCE/"
            });

            // ensure the original docs from the .xsds are forwarded into swagger spec
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        var app = builder.Build();

        //var logger = app.Services.GetRequiredService<ILogger<Program>>();

        if (options.Swagger.Enabled)
        {
            app.UseSwagger();       // http://localhost:5023/swagger/v1/swagger.json
            if (options.Swagger.UIEnabled)
                app.UseSwaggerUI(); // http://localhost:5023/swagger
        }

        app.MapControllers();

        await app.RunAsync();
        return 0;
    }
}