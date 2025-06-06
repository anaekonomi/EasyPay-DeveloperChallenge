using System.Text.RegularExpressions;

namespace CSVProcessor.Share.Helpers
{
    /// <summary>
    /// Text Extractor Helper
    /// </summary>
    public class TextExtractorHelper
    {
        /// <summary>
        /// Extracts data from string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static (string? FirstName, string? LastName, string? Date) ExtractData(string text)
        {
            var dateMatch = Regex.Match(text, @"\b(?:0?[1-9]|[12][0-9]|3[01])[\/.\-](?:0?[1-9]|1[0-2])[\/.\-](?:19|20)\d{2}\b");
            string date = null;

            if (dateMatch.Success)
                date = dateMatch.Value;

            // Extract client name with capitalized first + last name
            var nameMatch = Regex.Match(text, @"(?:(?<=\b(?:nga|per|klienti|te klienti|tek|te)\s+)|(?:\b))(?:Znj\.?|Zoti)?\s*([A-ZÇË][a-zçë]+(?:\s+[A-ZÇË][a-zçë]+)+)");
            if (nameMatch.Success)
            {
                var nameParts = nameMatch.Groups[1].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >= 2)
                {
                    var firstName = nameParts[0];
                    var lastName = string.Join(" ", nameParts.Skip(1));
                    return (firstName, lastName, date);
                }
            }

            return (null, null, date);
        }
    }
}