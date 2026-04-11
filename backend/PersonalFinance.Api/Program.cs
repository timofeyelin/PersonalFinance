using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFinanceDbContext>(serviceProvider =>
    serviceProvider.GetRequiredService<FinanceDbContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

