namespace PCMS.UCEDockets;

public class UCEDocketsOptions
{
    public const string Section = "UCEDockets";
    public string[] Counties { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; } = "sftp.nycourts.gov";
    public int Port { get; set; } = 22;

    public string LocalPath { get; set; }
    public bool UseImportedMarkers { get; set; } = true;

    public bool SwaggerEabled { get; set; } = true;
    public bool SwaggerUIEnabled { get; set; } = true;


    // if nonzero, this will enable the prometheus
    // monitoring endpoints to collect the counters
    public int MetricsPort { get; set; } = 0;
}