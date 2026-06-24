using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.Core.Interfaces
{
    /// <summary>
    /// Універсальний інтерфейс для репозиторію.
    /// T - це клас бізнес-моделі (наприклад, Project або TaskItem).
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // Отримати всі записи
        Task<IEnumerable<T>> GetAllAsync();

        // Отримати один запис за його ідентифікатором
        Task<T?> GetByIdAsync(int id);

        // Додати новий запис
        Task AddAsync(T entity);

        // Оновити існуючий запис
        void Update(T entity);

        // Видалити запис
        void Delete(T entity);
    }
}
