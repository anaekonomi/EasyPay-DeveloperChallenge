namespace CSVProcessor.Share.Models
{
    /// <summary>
    /// Configs class
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Determines the limit for processing a CSV file
        /// </summary>
        public int NrOfRowsLimit { get; set; }

        /// <summary>
        /// Determines the threshold for checking if a record is the one we're searching for
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// Determines the chunk size
        /// </summary>
        public int ChunkSize { get; set; }
    }
}