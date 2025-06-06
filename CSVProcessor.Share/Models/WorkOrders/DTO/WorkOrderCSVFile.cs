using Microsoft.AspNetCore.Http;

namespace CSVProcessor.Share.Models.WorkOrders.DTO
{
    /// <summary>
    /// CSV file model
    /// </summary>
    public class WorkOrderCSVFile
    {
        /// <summary>
        /// CSV file
        /// </summary>
        public IFormFile CSVFile { get; set; }
    }
}