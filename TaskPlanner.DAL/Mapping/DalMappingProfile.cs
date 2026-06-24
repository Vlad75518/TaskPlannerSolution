using AutoMapper;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.DAL.Entities;

namespace TaskPlanner.DAL.Mapping
{
    public class DalMappingProfile : Profile
    {
        public DalMappingProfile()
        {
            // ReverseMap() означає, що мапінг працює в обидва боки
            CreateMap<ProjectEntity, Project>().ReverseMap();
            CreateMap<TaskEntity, TaskItem>().ReverseMap();
        }
    }
}
