using TaskPlanner.Core.Enums;

namespace TaskPlanner.DAL.Entities
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }

        public int ProjectId { get; set; }
        public ProjectEntity? Project { get; set; }
    }
}