using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.Core.Enums;

namespace TaskPlanner.BLL.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);

        Task<IEnumerable<TaskItem>> SearchTasksAsync(string query);

        Task<TaskItem?> GetTaskByIdAsync(int taskId);

        Task AddTaskAsync(int projectId, TaskItem task);

        Task ChangeTaskStatusAsync(int taskId, TaskItemStatus newStatus);

        Task UpdateTaskAsync(TaskItem task);

        Task DeleteTaskAsync(int taskId);
    }
}
