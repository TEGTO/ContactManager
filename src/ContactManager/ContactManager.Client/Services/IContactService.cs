
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Update;

namespace ContactManager.Client.Services
{
    public interface IContactService
    {
        public Task<ContactResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
        public Task<IEnumerable<ContactResponse>> GetAllAsync(CancellationToken cancellationToken);
        public Task<HttpResponseMessage> DeleteByIdAsync(string id, CancellationToken cancellationToken);
        public Task<HttpResponseMessage> UploadFileToApiAsync(IFormFile file, CancellationToken cancellationToken);
        public Task<HttpResponseMessage> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken);
    }
}