using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksForProject(int projectId)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId);
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto createDto)
        {
            try
            {
                var taskItem = _mapper.Map<TaskItem>(createDto);
                await _taskService.AddTaskAsync(projectId, taskItem);
                return StatusCode(201);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult> UpdateTask(int projectId, int taskId, [FromBody] UpdateTaskDto updateDto)
        {
            try
            {
                var taskItem = _mapper.Map<TaskItem>(updateDto);
                taskItem.Id = taskId;
                taskItem.ProjectId = projectId;
                await _taskService.UpdateTaskAsync(taskItem);
                return NoContent();
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }

        [HttpPut("{taskId}/status")]
        public async Task<ActionResult> ChangeStatus(int projectId, int taskId, [FromBody] ChangeTaskStatusDto statusDto)
        {
            try
            {
                await _taskService.ChangeTaskStatusAsync(taskId, statusDto.NewStatus);
                return NoContent();
            }
            catch (ArgumentException ex) 
            {
                return NotFound(ex.Message); 
            }
        }

        // ДОДАНО: Метод DELETE
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
                return NotFound(ex.Message); // Повертаємо 404, якщо не знайдено
            }
        }
    }
}
