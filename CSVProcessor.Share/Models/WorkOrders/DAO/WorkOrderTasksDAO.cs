using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSVProcessor.Share.Models.WorkOrders.DAO
{
    /// <summary>
    /// Work order tasks DAO
    /// </summary>
    [Table("WorkOrderTasks")]
    public class WorkOrderTasksDAO
    {
        /// <summary>
        /// Record ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Output file path
        /// </summary>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Status of the task
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// 0 -> not started
        /// </item>
        /// <item>
        /// 1 -> started
        /// </item>
        /// <item>
        /// 2 -> finished
        /// </item>
        /// <item>
        /// -1 -> failed
        /// </item>
        /// </list>
        /// </remarks>
        public int Status { get; set; }

        /// <summary>
        /// Last modified datetime
        /// </summary>
        public DateTime LastModifiedDate { get; set; }
    }
}