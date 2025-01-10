using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Get;
using ContractManager.Communication.Dtos.Endpoints.Update;
using System.Net.Http.Headers;

namespace ContactManager.Client.Services
{
    public class ContactService : IContactService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string apiUrl;

        public ContactService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            apiUrl = configuration[ConfigurationKeys.API_URL] ?? "";
        }

        public async Task<ContactResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{apiUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ContactResponse>(cancellationToken);
        }

        public async Task<IEnumerable<ContactResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(apiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var contacts = await response.Content.ReadFromJsonAsync<GetResponse>(cancellationToken);
            return contacts?.Data ?? new List<ContactResponse>();
        }

        public async Task<HttpResponseMessage> DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            return await httpClient.DeleteAsync($"{apiUrl}/{id}", cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            return await httpClient.PutAsJsonAsync($"{apiUrl}", request, cancellationToken);
        }

        public async Task<HttpResponseMessage> UploadFileToApiAsync(IFormFile file, CancellationToken cancellationToken)
        {
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var httpClient = httpClientFactory.CreateClient();

            return await httpClient.PostAsync($"{apiUrl}/upload", content, cancellationToken);
        }
    }
}