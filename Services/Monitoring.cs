namespace PCMS.UCEDockets.Services;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using System;

public class Monitoring : BackgroundService
{
    private readonly IOptions<UCEDocketsOptions> options;
    private readonly ILogger<Monitoring> logging;

    public Monitoring(IOptions<UCEDocketsOptions> options, ILogger<Monitoring> logging)
    {
        this.options = options;
        this.logging = logging;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.Value.MetricsPort > 0)
        {
            logging.LogInformation($"Starting prometheus metrics endpoint on port {options.Value.MetricsPort}");
            try
            {
                var metricsServer = new KestrelMetricServer(port: options.Value.MetricsPort);
                metricsServer.Start();
            }
            catch (Exception e)
            {
                logging.LogError($"Failed: {e}");
            }
        }

        return Task.CompletedTask;
    }
}
