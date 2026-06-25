using System;

namespace TaskPlanner.DAL.Entities
{
    public class ProjectEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Ми прибрали ICollection<TaskEntity> Tasks. 
        // Зв'язок буде одностороннім з боку Завдання. Це покращує швидкодію.
    }
}