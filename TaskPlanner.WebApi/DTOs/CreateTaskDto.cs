using TaskPlanner.Core.Enums;

namespace TaskPlanner.WebApi.DTOs
{
    public class CreateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
