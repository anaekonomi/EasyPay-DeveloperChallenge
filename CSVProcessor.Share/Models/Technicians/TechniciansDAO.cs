using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSVProcessor.Share.Models.Technicians
{
    /// <summary>
    /// Technicians DAO
    /// </summary>
    [Table("Technicians")]
    public class TechniciansDAO
    {
        /// <summary>
        /// Record ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Technicians first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Technicians last name
        /// </summary>
        public string LastName { get; set; }
    }
}