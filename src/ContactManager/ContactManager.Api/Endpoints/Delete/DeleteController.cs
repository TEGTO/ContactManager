using ContactManager.Api.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Endpoints.Delete
{
    [Route("contacts")]
    [ApiController]
    public class DeleteController : ControllerBase
    {
        private readonly IContactRepository repository;

        public DeleteController(IContactRepository repository)
        {
            this.repository = repository;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactAsync(string id, CancellationToken cancellationToken)
        {
            var contanct = await repository.GetContactByIdAsync(id, cancellationToken);

            if (contanct != null)
            {
                await repository.DeleteContactAsync(contanct, cancellationToken);
            }

            return Ok();
        }
    }
}
