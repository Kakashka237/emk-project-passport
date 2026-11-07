using Emk.Domain.Interfaces;
using Emk.Gateway.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IScheduleRepository, InMemoryScheduleRepository>();

var app = builder.Build();
app.MapControllers();
app.Run();
