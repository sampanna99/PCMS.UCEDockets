namespace PCMS.UCEDockets.Services;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using PCMS.UCEDockets.Modules;
using Microsoft.Extensions.DependencyInjection;
using PCMS.UCEDockets.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

public class Scheduler : BackgroundService
{
    private readonly IOptions<UCEDocketsOptions> options;
    private readonly ILogger<Scheduler> logging;
    private readonly SFTP sftp;
    private readonly Importer importer;
    private readonly IServiceProvider serviceProvider;

    public Scheduler(IOptions<UCEDocketsOptions> options, ILogger<Scheduler> logging, SFTP sftp, Importer importer, IServiceProvider serviceProvider)
    {
        this.options = options;
        this.logging = logging;
        this.sftp = sftp;
        this.importer = importer;
        this.serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        await Task.Yield();
        
        using(IServiceScope scope = this.serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<UCEDocketsContext>();
            var migrator = context.Database.GetService<IMigrator>();
            await migrator.MigrateAsync();
        }

        while (!cancel.IsCancellationRequested)
        {
            logging.LogInformation("Beginning run");

            await sftp.ExecuteSynchronizations(cancel);
            await importer.DoImport(cancel);

            logging.LogInformation("Run Complete");
            
            await Task.Delay(TimeSpan.FromMinutes(5), cancel);
        }
    }
}
