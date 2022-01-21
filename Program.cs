﻿namespace PCMS.UCEDockets;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration
            .AddJsonFile("config/config.json", optional: true)
            .AddEnvironmentVariables();

        builder.Services.AddOptions<UCEDocketsOptions>()
            .Bind(builder.Configuration.GetSection(UCEDocketsOptions.Section));

        builder.Services.AddMvcCore().AddXmlSerializerFormatters();

        builder.Services.AddControllers();

        builder.Services.AddHostedService<Services.Monitoring>();
        builder.Services.AddHostedService<Services.Scheduler>();

        builder.Services.AddTransient<Modules.SFTP>();
        builder.Services.AddTransient<Modules.Importer>();

        builder.Services.AddDbContext<Entities.UCEDocketsContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

        var options = app.Services.GetRequiredService<IOptions<UCEDocketsOptions>>().Value;
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        logger.LogWarning(System.Text.Json.JsonSerializer.Serialize(options));

        if (options.SwaggerEabled)
        {
            app.UseSwagger();       // http://localhost:5023/swagger/v1/swagger.json
            if (options.SwaggerUIEnabled)
                app.UseSwaggerUI(); // http://localhost:5023/swagger
        }

        app.MapControllers();

        await app.RunAsync();

        return 0;
    }
}