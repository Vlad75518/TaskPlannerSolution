using TaskPlanner.Core.Enums;

namespace TaskPlanner.WebApi.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }
        public int ProjectId { get; set; }
    }
}
