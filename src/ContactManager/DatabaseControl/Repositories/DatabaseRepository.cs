using Microsoft.EntityFrameworkCore;

namespace DatabaseControl.Repositories
{
    public class DatabaseRepository<TContext> : IDatabaseRepository<TContext> where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext> dbContextFactory;

        public DatabaseRepository(IDbContextFactory<TContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        #region IDatabaseRepository Members

        public async Task MigrateDatabaseAsync(CancellationToken cancellationToken)
        {
            using var dbContext = await CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<TContext> GetDbContextAsync(CancellationToken cancellationToken)
        {
            return await CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<T> Query<T>(TContext dbContext) where T : class
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public async Task AddRangeAsync<T>(TContext dbContext, IEnumerable<T> objects, CancellationToken cancellationToken) where T : class
        {
            await dbContext.AddRangeAsync(objects, cancellationToken).ConfigureAwait(false);
        }

        public T Update<T>(TContext dbContext, T obj) where T : class
        {
            return dbContext.Update(obj).Entity;
        }

        public void Remove<T>(TContext dbContext, T obj) where T : class
        {
            dbContext.Remove(obj);
        }

        public async Task SaveChangesAsync(TContext dbContext, CancellationToken cancellationToken)
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        protected async Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken)
        {
            return await dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
