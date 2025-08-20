using Microsoft.EntityFrameworkCore;

public class BotDbContext : DbContext
{
    public DbSet<GuildSettings> GuildSettings { get; set; }
    public DbSet<GuildMessageSettings> GuildMessageSettings { get; set; }
    public DbSet<BanRecord> BanRecords { get; set; }
    public DbSet<TicketRecord> TicketRecords { get; set; }
    public DbSet<TicketMessages> TicketMessages { get; set; }
    public DbSet<GuildPresets> GuildPresets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
