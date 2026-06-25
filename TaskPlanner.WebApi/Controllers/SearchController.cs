using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // GET: api/search/tasks?q=
        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> SearchTasks([FromQuery] string q)
        {
            var tasks = await _taskService.SearchTasksAsync(q);
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }
    }
}