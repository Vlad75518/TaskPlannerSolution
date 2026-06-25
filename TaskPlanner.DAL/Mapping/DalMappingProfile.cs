using AutoMapper;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.DAL.Entities;

namespace TaskPlanner.DAL.Mapping
{
    public class DalMappingProfile : Profile
    {
        public DalMappingProfile()
        {
            CreateMap<ProjectEntity, Project>().ReverseMap();
            CreateMap<TaskEntity, TaskItem>().ReverseMap();
        }
    }
}
