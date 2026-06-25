using Microsoft.EntityFrameworkCore;
using TaskPlanner.BLL.Interfaces;
using TaskPlanner.BLL.Services;
using TaskPlanner.Core.Interfaces;
using TaskPlanner.DAL.Context;
using TaskPlanner.DAL.Mapping;
using TaskPlanner.DAL.UnitOfWork;
using TaskPlanner.WebApi.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskPlannerDbContext>(options =>
    options.UseSqlite("Data Source=TaskPlannerDb.db"));

// Налаштування AutoMapper
// Реєструємо відразу два профілі мапінгу: для DAL і для WebAPI
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<DalMappingProfile>();
    config.AddProfile<ApiMappingProfile>();
});

// Реєстрація залежностей
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Інтерфейс для тестування

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskPlannerDbContext>();
    context.Database.EnsureCreated();
}

// Налаштування Swagger (UI для тестування API)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
