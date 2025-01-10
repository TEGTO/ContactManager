using ContactManager.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Api.Data
{
    public class ContactDbContext : DbContext
    {
        public virtual DbSet<Contact> Contacts { get; set; }

        public ContactDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
