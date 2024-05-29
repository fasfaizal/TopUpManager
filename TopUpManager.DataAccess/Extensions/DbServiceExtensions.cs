using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TopUpManager.DataAccess.DBContext;

namespace TopUpManager.DataAccess.Extensions
{
    public static class DbServiceExtensions
    {
        public static IServiceCollection AddDBService(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TopUpManagerDbContext>(x => x.UseSqlServer(connectionString));
            return services;
        }
    }
}
