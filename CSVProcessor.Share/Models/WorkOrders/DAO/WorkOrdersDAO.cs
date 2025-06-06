using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSVProcessor.Share.Models.WorkOrders.DAO
{
    /// <summary>
    /// Work orders DAO
    /// </summary>
    [Table("WorkOrders")]
    public class WorkOrdersDAO
    {
        /// <summary>
        /// Work Order ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkOrderId { get; set; }

        /// <summary>
        /// Technician ID
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Information
        /// </summary>
        public string Information { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }
    }
}