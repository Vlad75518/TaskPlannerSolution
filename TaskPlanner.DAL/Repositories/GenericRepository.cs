using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Core.Interfaces;

namespace TaskPlanner.DAL.Repositories
{
    public class GenericRepository<TEntity, TDomain> : IRepository<TDomain>
        where TEntity : class
        where TDomain : class
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly IMapper _mapper;

        // Зверніть увагу: _context більше не зберігається як поле класу!
        public GenericRepository(DbContext context, IMapper mapper)
        {
            _dbSet = context.Set<TEntity>();
            _mapper = mapper;
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            // AsNoTracking - рятує пам'ять і роботу Garbage Collector'а
            var entities = await _dbSet.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<TDomain?> GetByIdAsync(int id)
        {
            // Шукаємо сутність (EF Core оптимізує FindAsync)
            var entity = await _dbSet.FindAsync(id);

            // Якщо знайшли - від'єднуємо від трекера (щоб не було проблем при Update)
            if (entity != null)
            {
                _dbSet.Entry(entity).State = EntityState.Detached;
            }

            return entity == null ? null : _mapper.Map<TDomain>(entity);
        }

        // Add СИНХРОННИЙ згідно з рекомендаціями EF Core
        public void Add(TDomain domainModel)
        {
            var entity = _mapper.Map<TEntity>(domainModel);
            _dbSet.Add(entity);
        }

        public void Update(TDomain domainModel)
        {
            var entity = _mapper.Map<TEntity>(domainModel);
            _dbSet.Update(entity);
        }

        // Delete по ID без перегонки цілої моделі туди-сюди
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}