namespace PCMS.UCEDockets.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public abstract class UCEDocketsContext : DbContext
{
    protected readonly IConfiguration Configuration;
    protected readonly UCEDocketsOptions UCEDocketsOptions;
    public UCEDocketsContext(IConfiguration configuration)
    {
        Configuration = configuration;

        this.UCEDocketsOptions = new UCEDocketsOptions();
        Configuration.Bind(UCEDocketsOptions.Section, this.UCEDocketsOptions);
    }

    

    public DbSet<Docket> Dockets => Set<Docket>();
}
