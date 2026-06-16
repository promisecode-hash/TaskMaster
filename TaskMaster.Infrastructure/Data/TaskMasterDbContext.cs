using Microsoft.EntityFrameworkCore;
using TaskMaster.Infrastructure.Entities;

namespace TaskMaster.Infrastructure.Data;

public sealed class TaskMasterDbContext : DbContext
{
    public TaskMasterDbContext(DbContextOptions<TaskMasterDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskEntity> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Status).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
