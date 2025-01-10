using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos.Endpoints.Update;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Endpoints.Update
{
    [Route("contacts")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IContactRepository repository;
        private readonly IMapper mapper;

        public UpdateController(IContactRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContactAsync(UpdateRequest request, CancellationToken cancellationToken)
        {
            var contanctInDb = await repository.GetContactByIdAsync(request.Id, cancellationToken);

            if (contanctInDb == null)
            {
                return BadRequest("The contact you are trying to update does not exist!");
            }

            var contact = mapper.Map<Contact>(request);

            contanctInDb.Copy(contact);

            await repository.UpdateContactAsync(contact, cancellationToken);

            return Ok();
        }
    }
}
