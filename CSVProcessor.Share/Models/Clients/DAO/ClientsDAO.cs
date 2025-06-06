using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSVProcessor.Share.Models.Clients.DAO
{
    /// <summary>
    /// Clients DAO
    /// </summary>
    [Table("Clients")]
    public class ClientsDAO
    {
        /// <summary>
        /// Record ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Clients first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Clients last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Clients full name
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}