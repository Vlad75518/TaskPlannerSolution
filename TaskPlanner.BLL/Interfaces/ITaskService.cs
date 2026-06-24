using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.Core.Enums;

namespace TaskPlanner.BLL.Interfaces
{
    public interface ITaskService
    {
        // Отримати всі завдання конкретного проєкту
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);

        Task<TaskItem?> GetTaskByIdAsync(int taskId);

        // Додати нове завдання до проєкту
        Task AddTaskAsync(int projectId, TaskItem task);

        // Змінити статус виконання
        Task ChangeTaskStatusAsync(int taskId, TaskItemStatus newStatus);

        // Відредагувати завдання
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int taskId);
    }
}
