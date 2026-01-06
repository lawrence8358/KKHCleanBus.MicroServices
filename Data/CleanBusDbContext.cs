using KKHCleanBus.MicroServices.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KKHCleanBus.MicroServices.Data;

public class CleanBusDbContext : DbContext
{
    public DbSet<News> News => Set<News>();
    public DbSet<ArrivalTime> ArrivalTime => Set<ArrivalTime>();
    public DbSet<ArrivalTimeDetail> ArrivalTimeDetail => Set<ArrivalTimeDetail>();

    public CleanBusDbContext(DbContextOptions<CleanBusDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // 設定所有 Guid 屬性使用 TEXT 格式（匹配現有 SQLite 資料庫）
        configurationBuilder.Properties<Guid>()
            .HaveConversion<string>();
        configurationBuilder.Properties<Guid?>()
            .HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes for better query performance
        modelBuilder.Entity<ArrivalTimeDetail>()
            .HasIndex(e => e.ParentId);

        modelBuilder.Entity<ArrivalTimeDetail>()
            .HasIndex(e => e.CarLicence);

        modelBuilder.Entity<ArrivalTimeDetail>()
            .HasIndex(e => e.ReplaceCarLicence);

        modelBuilder.Entity<News>()
            .HasIndex(e => e.SystemId);

        modelBuilder.Entity<News>()
            .HasIndex(e => e.Enabled);
    }
}

