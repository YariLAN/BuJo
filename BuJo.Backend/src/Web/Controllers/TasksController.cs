using BuJo.Application.Tasks.Abstractions;
using BuJo.Contracts.V1;
using BuJo.Contracts.V1.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BuJo.Web.Controllers;

[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet(ApiRoutesV1.Tasks)]
    public async Task<List<TaskResponse>> GetTasksAsync(CancellationToken ct)
    {
        // TODO: extract userId from auth context once implemented
        var userId = Guid.Empty;
        
        var tasks = await _taskService.GetTasksAsync(userId, ct);
        
        return tasks;
    }

    [HttpPost(ApiRoutesV1.Tasks)]
    public async Task<ActionResult<TaskResponse>> CreateTaskAsync(
        CreateTaskRequest request,
        CancellationToken ct)
    {
        // TODO: extract userId from auth context once implemented
        var userId = Guid.Empty;
        
        try
        {
            var task = await _taskService.CreateAsync(userId, request, ct);
            return CreatedAtAction(nameof(GetTasksAsync), new { taskId = task.Id }, task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}