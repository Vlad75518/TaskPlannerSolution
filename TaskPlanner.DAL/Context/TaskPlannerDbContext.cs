using Microsoft.EntityFrameworkCore;
using TaskPlanner.DAL.Entities;

namespace TaskPlanner.DAL.Context
{
    public class TaskPlannerDbContext : DbContext
    {
        public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
        public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

        public TaskPlannerDbContext(DbContextOptions<TaskPlannerDbContext> options) : base(options) { }

        // ОСЬ СУЧАСНИЙ ПІДХІД: Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Налаштування таблиці Projects
            modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Налаштування таблиці Tasks
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

                // Налаштування зв'язку (один до багатьох) без колекції в ProjectEntity
                entity.HasOne(e => e.Project)
                      .WithMany() // Пусто, бо ми прибрали колекцію з Project
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
