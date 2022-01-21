namespace PCMS.UCEDockets.Entities;

using Microsoft.EntityFrameworkCore;

public class UCEDocketsContext : DbContext
{
    public UCEDocketsContext(DbContextOptions<UCEDocketsContext> options) : base(options)
    {
    }

    public DbSet<Docket> Dockets => Set<Docket>();

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    //     => options.UseSqlServer();

}