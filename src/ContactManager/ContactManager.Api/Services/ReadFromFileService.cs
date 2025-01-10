using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace ContactManager.Api.Services
{
    public class ReadFromFileService : IReadFromFileService
    {
        public IEnumerable<T> ReadFromFile<T>(IFormFile file)
        {
            if (!file.ContentType.Contains("text") || !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The uploaded file is not a valid CSV file.");
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            if (reader.Peek() == -1)
            {
                throw new InvalidOperationException("The file is empty or not readable.");
            }

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
            {
                HasHeaderRecord = true,
                BadDataFound = null,
            });

            return csv.GetRecords<T>().ToArray();
        }
    }
}
