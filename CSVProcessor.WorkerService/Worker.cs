using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessorWorkerService
{
    /// <summary>
    /// CSV Worker
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executes task for processing the CSV file
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CsvTaskWorkerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                var csvRepository = scope.ServiceProvider.GetRequiredService<ICSVProcessorRepository>();
                var techniciansRepository = scope.ServiceProvider.GetRequiredService<ITechniciansRepository>();
                var clientsRepository = scope.ServiceProvider.GetRequiredService<IClientsRepository>();

                // we are only handling the not started ones
                var task = await dbContext.WorkOrderTasks
                .Where(t => t.Status == 0)
                .OrderBy(t => t.LastModifiedDate)
                .FirstOrDefaultAsync(stoppingToken);

                if (task == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                try
                {
                    if (task != null)
                    {
                        _logger.LogInformation($"Processing file: {task.FilePath}");

                        task.Status = 1;
                        await dbContext.SaveChangesAsync(stoppingToken);

                        var fileBytes = File.ReadAllBytes(task.FilePath);

                        var outputPath = await csvRepository.ProcessCSVAsync(task.Id, fileBytes, dbContext);

                        task.Status = 2;
                        task.OutputFilePath = outputPath;
                        task.LastModifiedDate = DateTime.Now;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    task.Status = -1;
                    task.LastModifiedDate = DateTime.Now;
                    _logger.LogError(ex, "Failed to process task.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("CsvTaskWorkerService stopped.");
        }

        /// <summary>
        /// Logic for stopping the worker
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker is stopping!");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
