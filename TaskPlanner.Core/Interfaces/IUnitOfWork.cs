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
        IRepository<T> Repository<T>() where T : class;

        // Перейменовано з CommitAsync на правильний термін
        Task<int> CompleteAsync();
    }
}
