using TaskPlanner.Core.Enums;

namespace TaskPlanner.WebApi.DTOs
{
    public class ChangeTaskStatusDto
    {
        public TaskItemStatus NewStatus { get; set; }
    }
}
