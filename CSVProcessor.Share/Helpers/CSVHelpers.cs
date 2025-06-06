using ClosedXML.Excel;
using CSVProcessor.Share.Models.WorkOrders.DTO;
using Microsoft.AspNetCore.Http;

namespace CSVProcessor.Share.Helpers
{
    /// <summary>
    /// CSV chunk reader helper class
    /// </summary>
    public class CSVHelpers
    {
        /// <summary>
        /// Checks number of rows in CSV file
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        public static int CheckCSVRowCount(IFormFile csvFile)
        {
            using var stream = csvFile.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            return rows.Count();
        }

        /// <summary>
        /// Escapes string before writting it to CSV file
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeForCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            // Escape double quotes and preserve line breaks
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        /// <summary>
        /// Reads CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<WorkOrder> ReadFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            var workOrders = new List<WorkOrder>();

            foreach (var row in rows)
            {
                var index = row.RowNumber();
                try
                {
                    var technician = row.Cell(1).GetString().Trim().Split(' ');
                    var notes = row.Cell(2).GetString().Trim();
                    var totalText = row.Cell(3).GetString().Trim();
                    decimal.TryParse(totalText, out var total);

                    var (clientFirstName, clientLastName, parsedDate) = TextExtractorHelper.ExtractData(notes);

                    var result = new WorkOrder
                    {
                        Index = index,
                        TechnicianFirstName = technician[0],
                        TechnicianLastName = technician[1],
                        ClientFirstName = clientFirstName,
                        ClientLastName = clientLastName,
                        Info = notes,
                        Date = parsedDate,
                        Total = total,
                        Status = "Pending"
                    };

                    workOrders.Add(result);
                }
                catch (Exception ex)
                {
                    workOrders.Add(new WorkOrder
                    {
                        Index = index,
                        Status = "Failed",
                        ErrorMessage = ex.Message
                    });
                }
            }

            return workOrders;
        }

        /// <summary>
        /// Reads file in chunks
        /// </summary>
        /// <param name="file"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<WorkOrder>> ReadChunksFromFile(byte[] file, int chunkSize)
        {
            using var stream = new MemoryStream(file);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            var chunk = new List<WorkOrder>();

            foreach (var row in rows)
            {
                var index = row.RowNumber();

                try
                {
                    var technician = row.Cell(1).GetString().Trim().Split(' ');
                    var notes = row.Cell(2).GetString().Trim();
                    var totalText = row.Cell(3).GetString().Trim();
                    decimal.TryParse(totalText, out var total);

                    var (clientFirstName, clientLastName, parsedDate) = TextExtractorHelper.ExtractData(notes);

                    var result = new WorkOrder
                    {
                        Index = index,
                        TechnicianFirstName = technician[0],
                        TechnicianLastName = technician[1],
                        ClientFirstName = clientFirstName,
                        ClientLastName = clientLastName,
                        Info = notes,
                        Date = parsedDate,
                        Total = total,
                        Status = "Pending"
                    };

                    chunk.Add(result);
                }
                catch (Exception ex)
                {
                    chunk.Add(new WorkOrder
                    {
                        Index = index,
                        Status = "Failed",
                        ErrorMessage = ex.Message
                    });
                }

                if (chunk.Count >= chunkSize)
                {
                    yield return chunk;
                    chunk = new List<WorkOrder>();
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}