using CSVProcessor.Share.Models.Clients.DAO;

namespace CSVProcessor.Share.Interfaces
{
    /// <summary>
    /// Reference of Clients Repository
    /// </summary>
    public interface IClientsRepository
    {
        /// <summary>
        /// Gets list of clients from the database
        /// </summary>
        /// <returns></returns>
        List<ClientsDAO> GetClients();

        /// <summary>
        /// Gets client by name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        Task<int> GetClientByName(string firstName, string lastName);
    }
}