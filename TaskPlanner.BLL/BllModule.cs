using Autofac;
using System.Reflection;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.BLL.Services;

namespace TaskPlanner.BLL
{
    public class BllModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectService>()
                   .As<IProjectService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<TaskService>()
                   .As<ITaskService>()
                   .InstancePerLifetimeScope();
        }
    }
}