using System;

namespace TaskPlanner.Core.DomainModels
{
    /// <summary>
    /// Бізнес-модель Проєкту.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Конструктор для задання значень за замовчуванням
        public Project()
        {
            // Автоматично фіксуємо час створення проєкту
            CreatedAt = DateTime.UtcNow;
        }
    }
}
