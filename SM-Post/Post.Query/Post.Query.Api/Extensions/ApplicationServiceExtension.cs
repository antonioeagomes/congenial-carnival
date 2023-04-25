using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Infra.DataAccess;

namespace Post.Query.Api.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection CreateDatabase(this IServiceCollection services, IConfiguration config)
        {
            Action<DbContextOptionsBuilder> configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(config.GetConnectionString("SqlServer"));
            services.AddDbContext<DatabaseContext>(configureDbContext);
            services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

            // Create database and tables from code
            var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
            dataContext.Database.EnsureCreated();
            return services;
        }
    }
}