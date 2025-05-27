// AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Skin> Skins { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка индексов для улучшения производительности
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => new { i.UserId, i.SkinId })
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Status);
    }
}