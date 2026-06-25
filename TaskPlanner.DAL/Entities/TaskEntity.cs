using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskPlanner.Core.Enums;

namespace TaskPlanner.DAL.Entities
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }

        // Зовнішній ключ для зв'язку з проєктом
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        public ProjectEntity? Project { get; set; }
    }
}
