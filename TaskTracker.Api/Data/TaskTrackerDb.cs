using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain;

namespace TaskTracker.Api.Data;

public class TaskTrackerDbContext : DbContext
{
    public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var task = modelBuilder.Entity<TaskItem>();

        task.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        task.Property(x => x.Notes)
            .HasMaxLength(500);

        task.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}