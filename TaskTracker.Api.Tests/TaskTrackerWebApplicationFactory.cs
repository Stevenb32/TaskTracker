using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskTracker.Api.Data;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests;

public class TaskTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<TaskTrackerDbContext>));
            services.RemoveAll(typeof(DbContextOptions<TaskTrackerDbContext>));

            services.AddDbContext<TaskTrackerDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();

        return await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

}