using Microsoft.EntityFrameworkCore;
using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;
using TaskMaster.Infrastructure.Data;
using TaskMaster.Infrastructure.Entities;

namespace TaskMaster.Infrastructure.Repositories;

public sealed class EfTaskRepository : ITaskRepository
{
    private readonly TaskMasterDbContext _dbContext;

    public EfTaskRepository(TaskMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TaskItem>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default)
    {
        query ??= new TaskQuery();
        var tasks = _dbContext.Tasks.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim();
            tasks = tasks.Where(x => x.Title.Contains(searchTerm) || (x.Description ?? string.Empty).Contains(searchTerm));
        }

        if (query.Status is not null)
        {
            tasks = tasks.Where(x => x.Status == query.Status.Value);
        }

        if (query.DueBefore is not null)
        {
            tasks = tasks.Where(x => x.DueDate.HasValue && x.DueDate <= query.DueBefore.Value);
        }

        var totalItems = await tasks.CountAsync(cancellationToken);
        var pageNumber = Math.Max(query.PageNumber, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var pagedTasks = await tasks
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pagedTasks.Select(x => x.ToDomain()).ToArray();

        return new PagedResult<TaskItem>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDomain();
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var entity = TaskEntity.FromDomain(task);
        await _dbContext.Tasks.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Tasks.FindAsync(new object[] { task.Id }, cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException($"Task with id '{task.Id}' was not found.");
        }

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.DueDate = task.DueDate;
        existing.Status = task.Status;

        _dbContext.Tasks.Update(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return;
        }

        _dbContext.Tasks.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
