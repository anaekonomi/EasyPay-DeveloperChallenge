using CSVProcessor.Share.Helpers;
using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Models.WorkOrders.DAO;
using CSVProcessor.Share.Models.WorkOrders.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace CSVProcessor.Share.Repositories
{
    /// <summary>
    /// CSV Processor Repository
    /// </summary>
    public class CSVProcessorRepository : ICSVProcessorRepository
    {
        private AppDBContext _db;
        private readonly IClientsRepository _clientsRepository;
        private readonly ITechniciansRepository _techniciansRepository;
        private readonly Configs _configs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="clientsRepository"></param>
        /// <param name="techniciansRepository"></param>
        /// <param name="configs"></param>
        public CSVProcessorRepository(AppDBContext db, IClientsRepository clientsRepository, ITechniciansRepository techniciansRepository, IOptions<Configs> configs)
        {
            _db = db;
            _clientsRepository = clientsRepository;
            _techniciansRepository = techniciansRepository;
            _configs = configs.Value;
        }

        /// <summary>
        /// Checks CSV row count
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        public int CheckCSVRowCount(IFormFile csvFile)
        {
            return CSVHelpers.CheckCSVRowCount(csvFile);
        }

        /// <summary>
        /// Logic for processing the CSV
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        public async Task<byte[]> ProcessCSV(IFormFile csvFile)
        {
            var csvRows = CSVHelpers.ReadFile(csvFile);
            var processedRows = await ProcessCSVRows(csvRows);

            var file = WriteCombinedCsv(processedRows);
            return file;
        }

        /// <summary>
        /// Processes CSV by chunks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="csvFile"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> ProcessCSVAsync(int taskId, byte[] csvFile, AppDBContext context = null)
        {
            if(context != null)
                _db = context;

            var allResults = new List<WorkOrder>();

            foreach (var chunk in CSVHelpers.ReadChunksFromFile(csvFile, _configs?.ChunkSize ?? 1000))
            {
                var processedChunks = await ProcessCSVRows(chunk);
                allResults.AddRange(processedChunks);
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadPath);

            var fileName = $"processed_{Guid.NewGuid()}.csv";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file to disk
            var file = WriteCombinedCsv(allResults);
            File.WriteAllBytes(filePath, file);

            return filePath;
        }

        /// <summary>
        /// Processes each row of the CSV
        /// </summary>
        /// <param name="csvRows"></param>
        /// <returns></returns>
        private async Task<List<WorkOrder>> ProcessCSVRows(List<WorkOrder> csvRows)
        {
            var validRows = new List<WorkOrder>();
            var allResults = new List<WorkOrder>();

            foreach (var row in csvRows)
            {
                try
                {
                    int clientId = await _clientsRepository.GetClientByName(row.ClientFirstName, row.ClientLastName);
                    if (clientId == 0)
                    {
                        row.Status = "Failed";
                        allResults.Add(row);
                        continue;
                    }

                    int techId = await _techniciansRepository.GetTechnicianByName(row.TechnicianFirstName, row.TechnicianLastName);
                    if (techId == 0)
                    {
                        row.Status = "Failed";
                        allResults.Add(row);
                        continue;
                    }

                    row.Status = "Success";
                    row.ClientId = clientId;
                    row.TechnicianId = techId;

                    validRows.Add(row);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    row.Status = "Failed";
                    row.ErrorMessage = ex.Message;

                    allResults.Add(row);
                }
            }

            // try to insert the batch
            try
            {
                await InsertBatch(validRows);

                allResults.AddRange(validRows);
            }
            // if batch fails, process the rows independently
            catch (Exception bulkEx)
            {
                Console.WriteLine(bulkEx.Message);

                foreach (var row in validRows)
                {
                    try
                    {
                        await InsertSingle(row);
                        row.Status = "Success";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        row.Status = "Failed";
                        row.ErrorMessage = $"Insert error: {ex.Message}";
                    }

                    allResults.Add(row);
                }
            }
            finally
            {
                validRows.Clear();
            }

            return allResults;
        }

        /// <summary>
        /// Logic for inserting batch
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        private async Task InsertBatch(List<WorkOrder> batch)
        {
            var workOrdersDAO = batch.Select(w => new WorkOrdersDAO
            {
                ClientId = w.ClientId,
                Date = w.Date,
                Information = w.Info,
                TechnicianId = w.TechnicianId,
                Total = w.Total
            });

            await _db.WorkOrders.AddRangeAsync(workOrdersDAO);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Logic for inserting a single row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task InsertSingle(WorkOrder row)
        {
            var workOrder = new WorkOrdersDAO()
            {
                ClientId = row.ClientId,
                Date = row.Date,
                Information = row.Info,
                TechnicianId = row.TechnicianId,
                Total = row.Total
            };

            _db.Add(workOrder);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Logic for writing CSV based on the specified columns
        /// </summary>
        /// <param name="allRows"></param>
        /// <returns></returns>
        private byte[] WriteCombinedCsv(List<WorkOrder> allRows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Technician,Notes,Total,Status,ErrorMessage");

            foreach (var row in allRows)
            {
                var technician = $"{row.TechnicianFirstName} {row.TechnicianLastName}";
                var notes = row.Info;
                var total = row.Total.ToString();
                var status = row.Status;
                var errorMessage = row.ErrorMessage;

                var csvRow = string.Join(",",
                    CSVHelpers.EscapeForCsv(technician),
                    CSVHelpers.EscapeForCsv(notes),
                    CSVHelpers.EscapeForCsv(total),
                    CSVHelpers.EscapeForCsv(status),
                    CSVHelpers.EscapeForCsv(errorMessage));

                sb.AppendLine(csvRow);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Logic for scheduling CSV processing task
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        public async Task<int> ScheduleCSVProcessTask(IFormFile csvFile)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadPath);

            var fileName = $"upload_{Guid.NewGuid()}.csv";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await csvFile.CopyToAsync(stream);
            }

            var newTask = new WorkOrderTasksDAO
            {
                FilePath = filePath,
                LastModifiedDate = DateTime.Now,
                Status = 0,
                OutputFilePath = string.Empty
            };

            _db.WorkOrderTasks.Add(newTask);
            await _db.SaveChangesAsync();

            return newTask.Id;
        }

        /// <summary>
        /// Get scheduled tasks
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WorkOrderTasks>> GetScheduledTasks()
        {
            var tasks = await _db.WorkOrderTasks.ToListAsync();

            return tasks.Select(t => new WorkOrderTasks
            {
                Id = t.Id,
                Status = t.Status
            });
        }

        /// <summary>
        /// Gets CSV's task status
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<WorkOrderTasks> GetCSVTaskStatus(int taskId)
        {
            var task = await _db.WorkOrderTasks.FirstOrDefaultAsync(t => t.Id == taskId);

            return new WorkOrderTasks
            {
                Id = task.Id,
                Status = task.Status
            };
        }

        /// <summary>
        /// Gets CSV output file 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<string> GetCSVOutputFile(int taskId)
        {
            var outputPath = await _db.WorkOrderTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.Status == 2);

            return outputPath?.OutputFilePath ?? string.Empty;
        }
    }
}