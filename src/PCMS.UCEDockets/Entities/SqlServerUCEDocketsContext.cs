namespace PCMS.UCEDockets.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class SqlServerUCEDocketsContext : UCEDocketsContext
{
    public SqlServerUCEDocketsContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(this.UCEDocketsOptions.SqlServer.ConnectionString);
    }
}
