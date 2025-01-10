
namespace ContactManager.Client.Services
{
    public interface IFileValidationService
    {
        public bool ValidateCsvFile(IFormFile file, out string errorMessage);
    }
}