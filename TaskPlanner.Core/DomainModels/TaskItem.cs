using TaskPlanner.Core.Enums;

namespace TaskPlanner.Core.DomainModels
{
    /// <summary>
    /// Бізнес-модель Завдання.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }

        // Зв'язок із проєктом (обов'язкова вимога)
        public int ProjectId { get; set; }

        public TaskItem()
        {
            // Згідно з бізнес-логікою, нове завдання за замовчуванням є "Не розпочатим"
            Status = TaskItemStatus.NotStarted;

            // За замовчуванням ставимо середній пріоритет
            // Priority = TaskPriority.Medium;
        }
    }
}
