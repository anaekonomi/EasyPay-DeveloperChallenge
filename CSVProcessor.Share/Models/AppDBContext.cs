using CSVProcessor.Share.Models.Clients.DAO;
using CSVProcessor.Share.Models.Technicians;
using CSVProcessor.Share.Models.WorkOrders.DAO;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Share.Models
{
    /// <summary>
    /// Application DB context
    /// </summary>
    public class AppDBContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        /// <summary>
        /// Work orders
        /// </summary>
        public DbSet<WorkOrdersDAO> WorkOrders { get; set; }

        /// <summary>
        /// Work orders tasks
        /// </summary>
        public DbSet<WorkOrderTasksDAO> WorkOrderTasks { get; set; }

        /// <summary>
        /// Clients
        /// </summary>
        public DbSet<ClientsDAO> Clients { get; set; }

        /// <summary>
        /// Technicians
        /// </summary>
        public DbSet<TechniciansDAO> Technicians { get; set; }
    }
}