namespace ContactManager.Client.Services
{
    public class FileValidationService : IFileValidationService
    {
        public bool ValidateCsvFile(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (file == null || file.Length == 0)
            {
                errorMessage = "Please upload a valid CSV file.";
                return false;
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Only CSV files are allowed.";
                return false;
            }

            return true;
        }
    }
}
