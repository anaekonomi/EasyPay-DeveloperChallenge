using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Configs>(builder.Configuration.GetSection("Configs"));
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration["SQL"], b => b.MigrationsAssembly("CSVProcessor")));

builder.Services.AddScoped<ITechniciansRepository, TechniciansRepository>();
builder.Services.AddScoped<IClientsRepository, ClientsRepository>();
builder.Services.AddScoped<ICSVProcessorRepository, CSVProcessorRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();