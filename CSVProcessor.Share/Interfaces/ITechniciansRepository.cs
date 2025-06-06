namespace CSVProcessor.Share.Interfaces
{
    /// <summary>
    /// Referencer of Technicians Repository
    /// </summary>
    public interface ITechniciansRepository
    {
        /// <summary>
        /// Gets technician by name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        Task<int> GetTechnicianByName(string firstName, string lastName);
    }
}