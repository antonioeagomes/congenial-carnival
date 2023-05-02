using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infra;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infra.Consumers;
using Post.Query.Infra.DataAccess;
using Post.Query.Infra.Dispatchers;
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
            services.AddScoped<IQueryHandler, QueryHandler>();
            services.AddScoped<IEventHandler, Infra.Handlers.EventHandler>();
            services.AddScoped<IEventConsumer, EventConsumer>();

            return services;
        }

        public static IServiceCollection ConsumerConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ConsumerConfig>(config.GetSection(nameof(ConsumerConfig)));
            return services;
        }

        public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services)
    {
        // Register Command handlers
        var queryHandler = services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
        var dispatcher = new QueryDispatcher();
        dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindCommentedPostsQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindLikedPostQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostByAuthorQuery>(queryHandler.HandleAsync);
        dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);

        services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

        return services;
    }
    }
}