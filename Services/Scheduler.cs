namespace PCMS.UCEDockets.Services;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using PCMS.UCEDockets.Modules;

public class Scheduler : BackgroundService
{
    private readonly IOptions<UCEDocketsOptions> options;
    private readonly ILogger<Scheduler> logging;
    private readonly SFTP sftp;
    private readonly Importer importer;

    public Scheduler(IOptions<UCEDocketsOptions> options, ILogger<Scheduler> logging, SFTP sftp, Importer importer)
    {
        this.options = options;
        this.logging = logging;
        this.sftp = sftp;
        this.importer = importer;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
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
