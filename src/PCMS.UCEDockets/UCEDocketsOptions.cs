namespace PCMS.UCEDockets;

public class UCEDocketsOptions
{
    public const string Section = "UCEDockets";

    public string LocalSyncPath { get; set; }
    public string[] Counties { get; set; }

    public SFTPOptions SFTP { get; set; } = new SFTPOptions();
    public class SFTPOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; } = "sftp.nycourts.gov";
        public int Port { get; set; } = 22;
    }

    public bool UseImportedMarkers { get; set; } = true;

    public MetricsOptions Metrics { get; set; } = new MetricsOptions();
    public class MetricsOptions
    {
        public bool PrometheusEnabled { get; set; } = false;
        public int Port { get; set; } = 5201;
    }


    public string EFDatabaseProvider { get; set; } = "sqlite";

    public SqliteOptions Sqlite { get; set; } = new SqliteOptions();
    public class SqliteOptions
    {
        public string Path { get; set; } = "../../data/db/ucedockets.sqlite";
    }

    public SqlServerOptions SqlServer { get; set; } = new SqlServerOptions();
    public class SqlServerOptions
    {
        public string ConnectionString { get; set; } = null;
    }

    public SwaggerOptions Swagger { get; set; } = new SwaggerOptions();
    public class SwaggerOptions
    {
        public bool Enabled { get; set; } = true;
        public bool UIEnabled { get; set; } = true;
    }
}