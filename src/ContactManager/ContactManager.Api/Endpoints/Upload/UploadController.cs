using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContactManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Endpoints.Upload
{
    [Route("contacts")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IContactRepository repository;
        private readonly IReadFromFileService readFromFileService;
        private readonly IMapper mapper;

        public UploadController(IContactRepository repository, IReadFromFileService readFromFileService, IMapper mapper)
        {
            this.repository = repository;
            this.readFromFileService = readFromFileService;
            this.mapper = mapper;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadContacts(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            try
            {
                var fileContacts = readFromFileService.ReadFromFile<FileContact>(file);

                var contacts = fileContacts.Select(mapper.Map<Contact>).ToList();

                await repository.CreateContactAsync(contacts, cancellationToken);

                return Ok("Contacts uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing file: {ex.Message}");
            }
        }
    }
}
