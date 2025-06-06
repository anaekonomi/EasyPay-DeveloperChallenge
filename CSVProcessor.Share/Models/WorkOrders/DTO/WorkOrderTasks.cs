namespace CSVProcessor.Share.Models.WorkOrders.DTO
{
    /// <summary>
    /// Work order tasks model which will be returned to the user
    /// </summary>
    public class WorkOrderTasks
    {
        /// <summary>
        /// Record ID
        /// </summary>
        public int Id { get; set; }

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
    }
}
