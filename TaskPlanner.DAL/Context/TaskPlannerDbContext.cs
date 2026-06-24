using Microsoft.EntityFrameworkCore;
using TaskPlanner.DAL.Entities;

namespace TaskPlanner.DAL.Context
{
    public class TaskPlannerDbContext : DbContext
    {
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }

        // Конструктор приймає налаштування з Program.cs (рядок підключення)
        public TaskPlannerDbContext(DbContextOptions<TaskPlannerDbContext> options) : base(options)
        {
        }
    }
}
