using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.Core.Enums;
using TaskPlanner.Core.Interfaces;

namespace TaskPlanner.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId)
        {
            var allTasks = await _unitOfWork.Repository<TaskItem>().GetAllAsync();

            // Відображати тільки ті завдання, які стосуються конкретного проекту
            return allTasks.Where(t => t.ProjectId == projectId);
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string query)
        {
            var allTasks = await _unitOfWork.Repository<TaskItem>().GetAllAsync();

            if (string.IsNullOrWhiteSpace(query)) return new List<TaskItem>();

            // Шукаємо по назві або опису (нечутливо до регістру)
            return allTasks.Where(t =>
                (t.Title != null && t.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (t.Description != null && t.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _unitOfWork.Repository<TaskItem>().GetByIdAsync(taskId);
        }

        public async Task AddTaskAsync(int projectId, TaskItem task)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectId);
            if (project == null)
            {
                throw new ArgumentException($"Проєкт з Id {projectId} не знайдено.");
            }

            task.ProjectId = projectId;

            _unitOfWork.Repository<TaskItem>().Add(task);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ChangeTaskStatusAsync(int taskId, TaskItemStatus newStatus)
        {
            var repository = _unitOfWork.Repository<TaskItem>();
            var task = await repository.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new ArgumentException("Завдання не знайдено.");
            }

            task.Status = newStatus;

            repository.Update(task);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            var existingTask = await _unitOfWork.Repository<TaskItem>().GetByIdAsync(task.Id);
            if (existingTask == null)
            {
                throw new ArgumentException("Завдання не знайдено.");
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Priority = task.Priority;

            _unitOfWork.Repository<TaskItem>().Update(existingTask);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteTaskAsync(int taskId)
        {
            var repository = _unitOfWork.Repository<TaskItem>();
            var task = await repository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Завдання з ID {taskId} не знайдено.");
            }

            await repository.DeleteAsync(taskId);
            await _unitOfWork.CompleteAsync();
        }
    }
}
