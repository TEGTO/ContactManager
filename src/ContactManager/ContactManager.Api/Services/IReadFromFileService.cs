
namespace ContactManager.Api.Services
{
    public interface IReadFromFileService
    {
        public IEnumerable<T> ReadFromFile<T>(IFormFile file);
    }
}