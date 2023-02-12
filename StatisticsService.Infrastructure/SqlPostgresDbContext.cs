using Microsoft.EntityFrameworkCore;
using StatisticsService.Domain.Entities;

namespace StatisticsService.Infrastructure;

public sealed class SqlPostgresDbContext : DbContext
{
    public SqlPostgresDbContext(DbContextOptions<SqlPostgresDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionSql>(entity =>
        {
            entity.ToTable("Transactions");

            entity.HasKey(e => e.TransactionNumber);
        });
    }
    public DbSet<TransactionSql> Transactions { get; set; }
}