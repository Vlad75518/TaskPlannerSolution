using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.WebApi.DTOs;

namespace TaskPlanner.WebApi.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TasksController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks(int projectId)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId);
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTask(int projectId, [FromBody] SaveTaskDto dto)
        {
            try
            {
                var domainTask = _mapper.Map<TaskItem>(dto);
                await _taskService.AddTaskAsync(projectId, domainTask);
                return StatusCode(201);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult> UpdateTask(int projectId, int taskId, [FromBody] SaveTaskDto dto)
        {
            try
            {
                var domainTask = _mapper.Map<TaskItem>(dto);
                domainTask.Id = taskId;
                domainTask.ProjectId = projectId; // Гарантуємо прив'язку до проєкту

                await _taskService.UpdateTaskAsync(domainTask);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{taskId}/status")]
        public async Task<ActionResult> ChangeTaskStatus(int projectId, int taskId, [FromBody] StatusUpdateDto dto)
        {
            try
            {
                // Тут ми отримуємо статус (0, 1, 2) як int і передаємо в BLL
                await _taskService.ChangeTaskStatusAsync(taskId, dto.NewStatus);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult> DeleteTask(int projectId, int taskId)
        {
            try
            {
                await _taskService.DeleteTaskAsync(taskId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}