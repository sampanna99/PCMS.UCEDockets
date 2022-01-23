namespace PCMS.UCEDockets.Modules;
using Microsoft.Extensions.Logging;
using Prometheus;
using PCMS.UCEDockets;
using PCMS.UCEDockets.Entities;
using System.IO;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

public class Importer
{
    private readonly IOptions<UCEDocketsOptions> options;
    private readonly ILogger<Importer> logger;
    private readonly IServiceProvider serviceProvider;

    private static Counter MetricImportedFilesTotal =
        Metrics.CreateCounter("imported_files_total", "Counts files imported",
            new CounterConfiguration
            {
                LabelNames = new[] { "county" },
            });
    private static Counter MetricImportedDocketsTotal =
        Metrics.CreateCounter("imported_dockets_total", "Count of dockets imported",
            new CounterConfiguration
            {
                LabelNames = new[] { "county" },
            });
    private static Counter MetricDeletedDocketsTotal =
        Metrics.CreateCounter("deleted_dockets_total", "Count of dockets deleted",
            new CounterConfiguration
            {
                LabelNames = new[] { "county" },
            });

    public Importer(IOptions<UCEDocketsOptions> options, ILogger<Importer> logger, IServiceProvider serviceProvider)
    {
        this.options = options;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public Task DoImport(CancellationToken cancel)
    {
        using IServiceScope scope = this.serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UCEDocketsContext>();

        logger.LogInformation($"Starting Import at {options.Value.LocalPath}");
        
        var fileCount = ImportLocalPathRecursively(context, options.Value.LocalPath);
        logger.LogInformation($"Import Complete: {fileCount} new files");

        if (fileCount == 0)
            logger.LogWarning($"There were no new files processed");

        return Task.CompletedTask;
    }

    public int ImportLocalPathRecursively(UCEDocketsContext context, string path)
    {
        int fileCount = 0;
        var files = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);

        // the filenames are formatted to sort by date if alpha sorted (thank you)
        Array.Sort(files);

        foreach (var fileName in files)
        {
            // using a zero-byte file named originalfile.zip.imported to mark whether
            // that particular file has already been imported
            // this allows a fastforwarding past already imported data

            // rationale: zero byte file seemed most likely successful across unexepcted
            //  filesystems. ready to be wrong.
            var importedMarker = $"{fileName}.imported";
            if (!options.Value.UseImportedMarkers || !File.Exists(importedMarker))
            {
                fileCount++;
                using var fileStream = new FileStream(fileName, FileMode.Open);
                ParseFile(context, fileStream, fileName);

                // mark the file as imported/patrolled
                if (options.Value.UseImportedMarkers)
                    File.WriteAllBytes(importedMarker, Array.Empty<byte>());
            }
        }

        return fileCount;
    }

    private void ParseFile(UCEDocketsContext context, Stream fileStream, string fileName)
    {
        var dockets = Common.UCEDocketsSerializer.Parse(fileStream);

        if (dockets == null)
        {
            logger.LogError($"{fileName} Failed to parse");
            return;
        }

        //QUESTION: not sure why these are typed as strings. might be my xsd code generator,
        // not a problem in source .xsd
        if (dockets.DocketCount == "0" && dockets.DocketDeletedCount == "0")
        {
            logger.LogWarning($"{fileName} Empty update");
            return;
        }

        var whichCounty = string.Empty;

        foreach (var district in dockets.District)
            foreach (var county in district.County)
            {
                whichCounty = county.Name;
                foreach (var court in county.Court)
                {
                    if (court.Docket != null && court.Docket.Any())
                        foreach (var docket in court.Docket)
                        {
                            var xml = Common.UCEDocketsSerializer.SerializeDocket(docket);

                            var docketEntity = context.Dockets.Find(docket.DocketID);
                            if (docketEntity != null)
                            {
                                docketEntity.Updated = DateTime.UtcNow;
                                docketEntity.XMLDocket = xml;

                                // QUESTION: any chance any of these other fields change without a new DocketID?

                                logger.LogDebug($"{fileName} UPDATE {district.Name} {county.Name} {docketEntity.DocketID}");
                            }
                            else
                            {
                                docketEntity = new Docket
                                {
                                    DocketID = docket.DocketID,
                                    District = district.Name,
                                    County = county.Name,
                                    Filed = docket.FiledDate,
                                    Created = DateTime.UtcNow,
                                    Updated = DateTime.UtcNow,
                                    XMLDocket = xml
                                };

                                context.Dockets.Add(docketEntity);
                                logger.LogDebug($"{fileName} INSERT {district.Name} {county.Name} {docketEntity.DocketID}");
                            }

                            MetricImportedDocketsTotal.WithLabels(whichCounty).Inc();
                        }

                    if (court.DeletedDocket != null && court.DeletedDocket.Any())
                        foreach (var deleted in court.DeletedDocket)
                        {
                            // QUESTION: is this complying properly? do we need to worry about the zip files?
                            var original = context.Dockets.FirstOrDefault(d => d.DocketID == deleted.DeletedDocketID);
                            if (original != null)
                                context.Dockets.Remove(original);

                            logger.LogDebug($"{fileName} DELETE {district.Name} {county.Name} {court.Name} {deleted.DeletedDocketID}");
                            MetricDeletedDocketsTotal.WithLabels(whichCounty).Inc();
                        }
                }
            }

        MetricImportedFilesTotal.WithLabels(whichCounty).Inc();
        context.SaveChanges();
    }
}