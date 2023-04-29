using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infra.Consumers;
using Post.Query.Infra.DataAccess;
using Post.Query.Infra.Handlers;
using Post.Query.Infra.Repositories;

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

        public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IEventHandler, Infra.Handlers.EventHandler>();
            services.AddScoped<IEventConsumer, EventConsumer>();

            return services;
        }

        public static IServiceCollection ConsumerConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ConsumerConfig>(config.GetSection(nameof(ConsumerConfig)));
            return services;
        }
    }
}