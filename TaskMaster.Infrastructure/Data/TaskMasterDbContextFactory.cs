using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskMaster.Infrastructure.Data;

public sealed class TaskMasterDbContextFactory : IDesignTimeDbContextFactory<TaskMasterDbContext>
{
    public TaskMasterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskMasterDbContext>();
        optionsBuilder.UseSqlite("Data Source=taskmaster.db");
        return new TaskMasterDbContext(optionsBuilder.Options);
    }
}
