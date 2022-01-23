namespace PCMS.UCEDockets.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class SqliteUCEDocketsContext : UCEDocketsContext
{
    public SqliteUCEDocketsContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={this.UCEDocketsOptions.Sqlite.Path}");
    }
}
