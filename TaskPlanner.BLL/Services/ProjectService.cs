using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.Core.Interfaces;

namespace TaskPlanner.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Впровадження залежності через конструктор
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var repository = _unitOfWork.Repository<Project>();
            return await repository.GetAllAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        }

        public async Task AddProjectAsync(Project project)
        {
            await _unitOfWork.Repository<Project>().AddAsync(project);

            // Обов'язково викликаємо Commit, інакше дані не збережуться!
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            var repository = _unitOfWork.Repository<Project>();
            var existingProject = await repository.GetByIdAsync(project.Id);

            if (existingProject == null)
            {
                throw new ArgumentException($"Проєкт з ID {project.Id} не знайдено.");
            }

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;

            repository.Update(existingProject);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var repository = _unitOfWork.Repository<Project>();
            var project = await repository.GetByIdAsync(id);
            if (project == null)
            {
                throw new ArgumentException($"Проєкт з ID {id} не знайдено.");
            }

            repository.Delete(project);
            await _unitOfWork.CommitAsync();
        }
    }
}
