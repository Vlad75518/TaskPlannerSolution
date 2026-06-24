using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Core.DomainModels;
using TaskPlanner.Core.Interfaces;
using TaskPlanner.DAL.Context;
using TaskPlanner.DAL.Entities;
using TaskPlanner.DAL.Repositories;

namespace TaskPlanner.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskPlannerDbContext _context;
        private readonly IMapper _mapper;

        // Словник для кешування вже створених репозиторіїв
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(TaskPlannerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<TDomain> Repository<TDomain>() where TDomain : class
        {
            var type = typeof(TDomain);

            // Якщо репозиторій вже був створений в межах транзакції, повертаємо його
            if (_repositories.ContainsKey(type))
            {
                return (IRepository<TDomain>)_repositories[type];
            }

            object repositoryInstance;

            // Визначаємо, яку сутність БД підставити для бізнес-моделі
            if (type == typeof(Project))
            {
                repositoryInstance = new GenericRepository<ProjectEntity, Project>(_context, _mapper);
            }
            else if (type == typeof(TaskItem))
            {
                repositoryInstance = new GenericRepository<TaskEntity, TaskItem>(_context, _mapper);
            }
            else
            {
                throw new ArgumentException($"Модель {type.Name} не підтримується у UnitOfWork.");
            }

            _repositories.Add(type, repositoryInstance);
            return (IRepository<TDomain>)_repositories[type];
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this); // Гарна практика при IDisposable
        }
    }
}
