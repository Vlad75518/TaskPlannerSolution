using System;
using System.Threading.Tasks;

namespace TaskPlanner.Core.Interfaces
{
    /// <summary>
    /// Інтерфейс Unit of Work для керування транзакціями та доступом до репозиторіїв.
    /// Згідно з вимогами на "відмінно", забезпечує ізоляцію джерела даних.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Універсальний метод отримання репозиторію для будь-якої бізнес-моделі
        IRepository<T> Repository<T>() where T : class;

        // Збереження всіх накопичених змін у базу даних (еквівалент SaveChanges)
        Task<int> CommitAsync();
    }
}
