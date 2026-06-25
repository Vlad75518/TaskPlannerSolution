using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Core.Interfaces;
using TaskPlanner.DAL.Context;

namespace TaskPlanner.DAL.Repositories
{
    public class GenericRepository<TEntity, TDomain> : IRepository<TDomain>
        where TEntity : class
        where TDomain : class
    {
        private readonly TaskPlannerDbContext _context;
        private readonly IMapper _mapper; // Автомапер як об'єкт
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(TaskPlannerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            // Мапимо список сутностей БД у список бізнес-моделей
            return _mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<TDomain?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity == null ? null : _mapper.Map<TDomain>(entity);
        }

        public async Task AddAsync(TDomain domainModel)
        {
            var entity = _mapper.Map<TEntity>(domainModel);
            await _dbSet.AddAsync(entity);
        }

        public void Update(TDomain domainModel)
        {
            _context.ChangeTracker.Clear();

            var entity = _mapper.Map<TEntity>(domainModel);
            _dbSet.Update(entity);
        }

        public void Delete(TDomain domainModel)
        {
            _context.ChangeTracker.Clear();

            var entity = _mapper.Map<TEntity>(domainModel);
            _dbSet.Remove(entity);
        }
    }
}
