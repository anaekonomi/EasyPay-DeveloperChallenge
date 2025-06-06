using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Models.Technicians;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Share.Repositories
{
    /// <summary>
    /// Technicians Repository
    /// </summary>
    public class TechniciansRepository : ITechniciansRepository
    {
        private readonly AppDBContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public TechniciansRepository(AppDBContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets technicians by name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<int> GetTechnicianByName(string firstName, string lastName)
        {
            try
            {
                var technician = await _db.Technicians.FirstOrDefaultAsync(t => t.FirstName == firstName && t.LastName == lastName);

                if (technician != null)
                    return technician.Id;

                var newTechnician = new TechniciansDAO { FirstName = firstName, LastName = lastName };
                _db.Technicians.Add(newTechnician);
                await _db.SaveChangesAsync();

                return newTechnician.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}