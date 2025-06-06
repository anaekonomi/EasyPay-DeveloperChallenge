namespace CSVProcessor.Share.Models.WorkOrders.DTO
{
    /// <summary>
    /// Work Order DTO
    /// </summary>
    public class WorkOrder
    {
        /// <summary>
        /// Row number
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Client first name
        /// </summary>
        public string ClientFirstName { get; set; }

        /// <summary>
        /// Client last name
        /// </summary>
        public string ClientLastName { get; set; }

        /// <summary>
        /// Technician first name
        /// </summary>
        public string TechnicianFirstName { get; set; }

        /// <summary>
        /// Technician last name
        /// </summary>
        public string TechnicianLastName { get; set; }

        /// <summary>
        /// Raw date converted in datetime
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Information
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Technician ID
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// 0 -> processed unsuccessfully
        /// </item>
        /// <item>
        /// 1 -> processed successfully
        /// </item>
        /// </list>
        /// </remarks>
        public string Status { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}