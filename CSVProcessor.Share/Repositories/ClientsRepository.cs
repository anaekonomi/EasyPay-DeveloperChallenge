using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Models.Clients.DAO;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CSVProcessor.Share.Repositories
{
    /// <summary>
    /// Clients Repository
    /// </summary>
    public class ClientsRepository : IClientsRepository
    {
        private readonly AppDBContext _db;
        private readonly List<ClientsDAO> clients;
        private readonly Configs _configs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configs"></param>
        public ClientsRepository(AppDBContext db, IOptions<Configs> configs)
        {
            _db = db;
            _configs = configs.Value;

            clients = GetClients();
        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <returns></returns>
        public List<ClientsDAO> GetClients()
        {
            try
            {
                return _db.Clients.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return [];
            }
        }

        /// <summary>
        /// Gets clients by name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<int> GetClientByName(string firstName, string lastName)
        {
            try
            {
                string inputFullName = $"{firstName} {lastName}";

                var bestMatch = clients
                            .Select(c => new
                            {
                                c.Id,
                                c.FirstName,
                                c.LastName,
                                Score = Fuzz.Ratio(c.FullName, inputFullName)
                            })
                            .OrderByDescending(x => x.Score)
                            .FirstOrDefault();

                if (bestMatch != null && bestMatch.Score >= _configs.Threshold)
                    return bestMatch.Id;

                // No close enough match found — create a new client
                var newClient = new ClientsDAO { FirstName = firstName, LastName = lastName };
                _db.Clients.Add(newClient);
                await _db.SaveChangesAsync();

                return newClient.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}