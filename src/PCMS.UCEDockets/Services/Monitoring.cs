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

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        if (options.Value.Metrics.PrometheusEnabled)
        {
            logging.LogInformation($"Starting prometheus metrics endpoint on port {options.Value.Metrics.Port}");
            try
            {
                var metricsServer = new KestrelMetricServer(port: options.Value.Metrics.Port);
                metricsServer.Start();
            }
            catch (Exception e)
            {
                logging.LogError($"Failed: {e}");
            }
        }
    }
}
