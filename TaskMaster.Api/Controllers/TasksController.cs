using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Api.Models;
using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Models;

namespace TaskMaster.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskQuery? query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all tasks with query: {@Query}", query);
        var tasks = await _taskService.GetAllAsync(query, cancellationToken);
        _logger.LogInformation("Retrieved {Count} tasks", tasks.Items.Count);
        return Ok(new ApiResponse<PagedResult<TaskDto>> { Data = tasks, Success = true });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting task with ID: {TaskId}", id);
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });
        }
        return Ok(new ApiResponse<TaskDto> { Data = task, Success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new task: {@Request}", request);
        var task = await _taskService.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Task created with ID: {TaskId}", task.Id);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, new ApiResponse<TaskDto> { Data = task, Success = true });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        var task = await _taskService.UpdateAsync(id, request, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found for update", id);
            return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });
        }
        _logger.LogInformation("Task with ID {TaskId} updated successfully", id);
        return Ok(new ApiResponse<TaskDto> { Data = task, Success = true });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        var deleted = await _taskService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning("Task with ID {TaskId} not found for deletion", id);
            return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });
        }
        _logger.LogInformation("Task with ID {TaskId} deleted successfully", id);
        return NoContent();
    }
}
