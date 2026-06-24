using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskPlanner.DAL.Entities
{
    public class ProjectEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Навігаційна властивість: один проєкт має багато завдань
        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}
