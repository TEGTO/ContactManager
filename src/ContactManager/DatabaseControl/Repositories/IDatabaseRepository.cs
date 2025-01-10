using Microsoft.EntityFrameworkCore;

namespace DatabaseControl.Repositories
{
    public interface IDatabaseRepository<TContext> where TContext : DbContext
    {
        public Task MigrateDatabaseAsync(CancellationToken cancellationToken);
        public Task<TContext> GetDbContextAsync(CancellationToken cancellationToken);
        public IQueryable<T> Query<T>(TContext dbContext) where T : class;
        public Task AddRangeAsync<T>(TContext dbContext, IEnumerable<T> objects, CancellationToken cancellationToken) where T : class;
        public T Update<T>(TContext dbContext, T obj) where T : class;
        public void Remove<T>(TContext dbContext, T obj) where T : class;
        public Task SaveChangesAsync(TContext dbContext, CancellationToken cancellationToken);
    }
}