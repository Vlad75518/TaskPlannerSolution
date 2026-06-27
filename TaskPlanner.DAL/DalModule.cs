using Autofac;
using System.Reflection;
using TaskPlanner.Core.Interfaces;
using TaskPlanner.DAL.Context;
using TaskPlanner.DAL.Repositories;

namespace TaskPlanner.DAL
{
    public class DalModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєструємо Generic Repository
            builder.RegisterGeneric(typeof(GenericRepository<,>))
                   .As(typeof(IRepository<>))
                   .InstancePerLifetimeScope();

            
            builder.RegisterType<TaskPlanner.DAL.UnitOfWork.UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerLifetimeScope();

            // Примітка: DbContext реєструється стандартно через AddDbContext в Program.cs, 
            // оскільки Autofac добре працює з вбудованою фабрикою контекстів EF Core.
        }
    }
}