using AutoMapper;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Get;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Endpoints.Get
{
    [Route("contacts")]
    [ApiController]
    public class GetController : ControllerBase
    {
        private readonly IContactRepository repository;
        private readonly IMapper mapper;

        public GetController(IContactRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<GetResponse>> GetContractsAsync(CancellationToken cancellationToken)
        {
            var contracts = await repository.GetContactsAsync(cancellationToken);

            var response = new GetResponse()
            {
                Data = contracts.Select(mapper.Map<ContactResponse>).ToList(),
                TotalCount = contracts.Count()
            };

            return Ok(response);
        }
    }
}
