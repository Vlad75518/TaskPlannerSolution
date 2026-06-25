using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.WebApi.DTOs;

namespace TaskPlanner.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProjectDto createDto)
        {
            var project = _mapper.Map<Project>(createDto);
            await _projectService.AddProjectAsync(project);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromBody] CreateProjectDto updateDto)
        {
            try
            {
                var project = _mapper.Map<Project>(updateDto);
                project.Id = id;
                await _projectService.UpdateProjectAsync(project);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // ДОДАНО: Метод DELETE для відповідності на "відмінно"
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // Повертаємо 404, якщо не знайдено
            }
        }
    }
}
