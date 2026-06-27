using TaskPlanner.Core.Enums;

namespace TaskPlanner.WebApi.DTOs
{
    public class SaveTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
    }
}