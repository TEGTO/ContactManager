using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseControl
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository<Context>(this IServiceCollection services) where Context : DbContext
        {
            services.AddSingleton<IDatabaseRepository<Context>, DatabaseRepository<Context>>();
            return services;
        }

        public static IServiceCollection AddDbContextFactory<Context>
            (this IServiceCollection services, string connectionString, string? migrationAssembly = null) where Context : DbContext
        {
            services.AddDbContextFactory<Context>(options =>
            {
                options.UseSqlServer(connectionString, b =>
                {
                    if (!string.IsNullOrEmpty(migrationAssembly))
                    {
                        b.MigrationsAssembly(migrationAssembly);
                    }
                });
            });

            return services;
        }
    }
}
