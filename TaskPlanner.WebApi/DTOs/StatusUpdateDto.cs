using TaskPlanner.Core.Enums;

namespace TaskPlanner.WebApi.DTOs
{
    public class StatusUpdateDto
    {
        public TaskItemStatus NewStatus { get; set; }
    }
}
