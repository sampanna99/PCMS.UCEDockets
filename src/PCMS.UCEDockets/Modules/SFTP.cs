namespace PCMS.UCEDockets.Modules;

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Prometheus;
using PCMS.UCEDockets;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;

public class SFTP
{
    private readonly IOptions<UCEDocketsOptions> options;
    private readonly ILogger<SFTP> logger;

    private static Counter MetricNewFilesTotal =
        Metrics.CreateCounter("new_files_total", "Counts files downloaded from the sftp server",
            new CounterConfiguration
            {
                LabelNames = new[] { "county" },
            });
    private static Counter MetricNewFileSizeTotal =
        Metrics.CreateCounter("new_files_bytes_total", "Count of bytes downloaded from the sftp server",
            new CounterConfiguration
            {
                LabelNames = new[] { "county" },
            });

    public SFTP(IOptions<UCEDocketsOptions> options, ILogger<SFTP> logger)
    {
        this.options = options;
        this.logger = logger;
    }

    public Task ExecuteSynchronizations(CancellationToken cancel)
    {
        logger.LogInformation($"Starting synchronization of files over with {options.Value.SFTP.UserName}@{options.Value.SFTP.Host} to {options.Value.LocalSyncPath}");

        if (!Directory.Exists(options.Value.LocalSyncPath))
        {
            logger.LogWarning($"local path \"{options.Value.LocalSyncPath}\" does not exist, attempting to create it");
            Directory.CreateDirectory(options.Value.LocalSyncPath);
        }

        using var sftp = new Renci.SshNet.SftpClient(options.Value.SFTP.Host, options.Value.SFTP.Port, options.Value.SFTP.UserName, options.Value.SFTP.Password);

        sftp.Connect();
        logger.LogDebug($"connected");

        foreach (var county in options.Value.Counties)
        {
            var remotePath = $"/UCE/Incremental-Standard/{county}/";

            logger.LogDebug($"querying remote file list {remotePath}");
            int incrementalsSkipped = 0;

            foreach (var entry in sftp.ListDirectory(remotePath))
            {
                if (cancel.IsCancellationRequested)
                    throw new TaskCanceledException();

                if (entry.IsDirectory && entry.Name != "." && entry.Name != "..")
                {
                    var relativePath = entry.FullName.StartsWith("/") ? entry.FullName.Substring(1) : entry.FullName;
                    var localPath = Path.Combine(options.Value.LocalSyncPath, relativePath);

                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                        foreach (var zip in sftp.ListDirectory(entry.FullName))
                        {
                            if (zip.IsRegularFile)
                            {
                                var localFilePath = Path.Combine(localPath, zip.Name);
                                if (!File.Exists(localFilePath))
                                {
                                    logger.LogDebug($"downloading {zip.FullName} to {localFilePath}");

                                    using (var localFileStream = File.OpenWrite(localFilePath))
                                        sftp.DownloadFile(zip.FullName, localFileStream);

                                    var fi = new FileInfo(localFilePath);
                                    if (!fi.Exists)
                                        throw new Exception("file failed to download or has zero byte length");

                                    MetricNewFileSizeTotal.WithLabels(county).Inc(fi.Length);
                                    MetricNewFilesTotal.WithLabels(county).Inc();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (incrementalsSkipped == 0)
                            logger.LogInformation($"Matched incremental");

                        incrementalsSkipped++;
                        logger.LogDebug($"FastForward: {entry.FullName}");
                    }
                }
            }
        }
        sftp.Disconnect();

        return Task.CompletedTask;
    }
}