using Microsoft.EntityFrameworkCore;
using System;
using ToDoTaskManagement.Data;
using ToDoTaskManagement.Domain.Interfaces;
using ToDoTaskManagement.Infrastructure.Repositories;
using ToDoTaskManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Read allowed CORS origins from appsettings.json
var allowedOrigins = builder.Configuration
    .GetSection("AllowedCorsOrigins")
    .Get<string[]>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register TodoDbContext in API so that migrations, DB file are in the Api process.
// SQLite file will be created next to working directory as todotaskmanagement.db
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// repository + service
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Apply CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Ensure DB created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
