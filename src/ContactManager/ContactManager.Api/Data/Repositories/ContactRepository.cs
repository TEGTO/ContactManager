using ContactManager.Api.Data.Entities;
using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Api.Data.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDatabaseRepository<ContactDbContext> repository;

        public ContactRepository(IDatabaseRepository<ContactDbContext> repository)
        {
            this.repository = repository;
        }

        public async Task CreateContactAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken)
        {
            using var dbContext = await repository.GetDbContextAsync(cancellationToken);
            await repository.AddRangeAsync(dbContext, contacts, cancellationToken);
            await repository.SaveChangesAsync(dbContext, cancellationToken);
        }

        public async Task DeleteContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            using var dbContext = await repository.GetDbContextAsync(cancellationToken);
            repository.Remove(dbContext, contact);
            await repository.SaveChangesAsync(dbContext, cancellationToken);
        }

        public async Task<Contact?> GetContactByIdAsync(string id, CancellationToken cancellationToken)
        {
            using var dbContext = await repository.GetDbContextAsync(cancellationToken);
            return await repository.Query<Contact>(dbContext).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(CancellationToken cancellationToken)
        {
            using var dbContext = await repository.GetDbContextAsync(cancellationToken);
            return await repository.Query<Contact>(dbContext).ToListAsync(cancellationToken);
        }

        public async Task UpdateContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            using var dbContext = await repository.GetDbContextAsync(cancellationToken);
            repository.Update(dbContext, contact);
            await repository.SaveChangesAsync(dbContext, cancellationToken);
        }
    }
}
