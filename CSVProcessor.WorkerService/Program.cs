using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Repositories;
using CSVProcessorWorkerService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration["SQL"], b => b.MigrationsAssembly("CSVProcessor")));
builder.Services.AddScoped<ICSVProcessorRepository, CSVProcessorRepository>();
builder.Services.AddScoped<ITechniciansRepository, TechniciansRepository>();
builder.Services.AddScoped<IClientsRepository, ClientsRepository>();

var host = builder.Build();
host.Run();
