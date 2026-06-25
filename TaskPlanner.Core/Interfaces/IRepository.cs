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
        // Читання залишається асинхронним
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        // ВАЖЛИВО: Add тепер СИНХРОННИЙ! (Вимога Microsoft)
        void Add(T entity);

        void Update(T entity);

        // ВАЖЛИВО: Delete тепер приймає ID, а не цілу модель!
        Task DeleteAsync(int id);
    }
}
