using ContactManager.Api.Data.Entities;

namespace ContactManager.Api.Data.Repositories
{
    public interface IContactRepository
    {
        public Task CreateContactAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken);
        public Task DeleteContactAsync(Contact contact, CancellationToken cancellationToken);
        public Task<Contact?> GetContactByIdAsync(string id, CancellationToken cancellationToken);
        public Task<IEnumerable<Contact>> GetContactsAsync(CancellationToken cancellationToken);
        public Task UpdateContactAsync(Contact contact, CancellationToken cancellationToken);
    }
}