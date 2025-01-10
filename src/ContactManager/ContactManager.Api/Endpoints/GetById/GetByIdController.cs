using AutoMapper;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Get;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Endpoints.GetById
{
    [Route("contacts")]
    [ApiController]
    public class GetByIdController : ControllerBase
    {
        private readonly IContactRepository repository;
        private readonly IMapper mapper;

        public GetByIdController(IContactRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetResponse>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var contact = await repository.GetContactByIdAsync(id, cancellationToken);

            if (contact == null)
            {
                return NotFound();
            }

            var response = mapper.Map<ContactResponse>(contact);

            return Ok(response);
        }
    }
}
