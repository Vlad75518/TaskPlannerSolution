using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.WebApi.DTOs;

namespace TaskPlanner.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public SearchController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> SearchTasks([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new List<TaskItemDto>());
            }

            var tasks = await _taskService.SearchTasksAsync(q);
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }
    }
}