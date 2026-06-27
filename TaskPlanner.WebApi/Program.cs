using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskPlanner.BLL;
using TaskPlanner.DAL;
using TaskPlanner.DAL.Context;
using TaskPlanner.DAL.Mapping;
using TaskPlanner.WebApi.Mapping;

var builder = WebApplication.CreateBuilder(args);

// 1. ЗАМІНА ВБУДОВАНОГО DI НА AUTOFAC (Вимога Лаб 2.4)
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Підключаємо модулі. PL тепер не знає про конкретні класи реалізації!
    containerBuilder.RegisterModule(new DalModule());
    containerBuilder.RegisterModule(new BllModule());
});

// Підключення БД
builder.Services.AddDbContext<TaskPlannerDbContext>(options =>
    options.UseSqlite("Data Source=TaskPlannerDb.db"));

// Налаштування AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<DalMappingProfile>();
    config.AddProfile<ApiMappingProfile>();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskPlannerDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Важливо для роботи вашого UI (index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();