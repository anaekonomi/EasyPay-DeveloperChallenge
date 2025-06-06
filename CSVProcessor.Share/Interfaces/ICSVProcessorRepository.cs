using CSVProcessor.Share.Models;
using CSVProcessor.Share.Models.WorkOrders.DTO;
using Microsoft.AspNetCore.Http;

namespace CSVProcessor.Share.Interfaces
{
    /// <summary>
    /// Reference of CSV Processor Repository
    /// </summary>
    public interface ICSVProcessorRepository
    {
        /// <summary>
        /// Checks row count of CSV file
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        int CheckCSVRowCount(IFormFile csvFile);

        /// <summary>
        /// Processes CSV file
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        Task<byte[]> ProcessCSV(IFormFile csvFile);

        /// <summary>
        /// Processes CSV file in chunks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="csvFile"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<string> ProcessCSVAsync(int taskId, byte[] csvFile, AppDBContext context = null);

        /// <summary>
        /// Schedules CSV file for processing
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        Task<int> ScheduleCSVProcessTask(IFormFile csvFile);

        /// <summary>
        /// Gets list of scheduled tasks
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WorkOrderTasks>> GetScheduledTasks();

        /// <summary>
        /// Gets status of scheduled task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<WorkOrderTasks> GetCSVTaskStatus(int taskId);

        /// <summary>
        /// Gets output file of processed task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<string> GetCSVOutputFile(int taskId);
    }
}