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

    public bool SwaggerEnabled { get; set; } = true;
    public bool SwaggerUIEnabled { get; set; } = true;

    public bool PrometheusEnabled { get; set; } = false;
    public int MetricsPort { get; set; } = 5201;

    public bool SQLiteEnabled { get; set; } = true;
    public string SQLitePath { get; set; } = null;
    public bool MSSQLEnabled { get; set; } = false;
}