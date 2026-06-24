using AutoMapper;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.WebApi.DTOs;

namespace TaskPlanner.WebApi.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            // Мапінг проєктів
            CreateMap<Project, ProjectDto>();
            CreateMap<CreateProjectDto, Project>();

            // Мапінг завдань
            CreateMap<TaskItem, TaskItemDto>();
            CreateMap<CreateTaskDto, TaskItem>();
            CreateMap<UpdateTaskDto, TaskItem>();
        }
    }
}
