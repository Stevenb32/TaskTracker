using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain;

namespace TaskTracker.Api.Data;

public class TaskTrackerDbContext : DbContext
{
    public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}