using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using CQRS.Core.Producers;
using Microsoft.Extensions.DependencyInjection;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregate;
using Post.Cmd.Infra.Config;
using Post.Cmd.Infra.Dispatchers;
using Post.Cmd.Infra.Handlers;
using Post.Cmd.Infra.Producers;
using Post.Cmd.Infra.Repositories;
using Post.Cmd.Infra.Stores;

namespace Post.Cmd.Infra.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
    {
        // Register Command handlers
        var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
        var dispatcher = new CommandDispatcher();
        dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditContentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);

        services.AddSingleton<ICommandDispatcher>(_ => dispatcher);

        return services;
    }

    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        services.AddScoped<IEventProducer, EventProducer>();
        services.AddScoped<IEventStore, EventStore>();        
        services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        
        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbConfig>(config.GetSection(nameof(MongoDbConfig)));
        services.Configure<ProducerConfig>(config.GetSection(nameof(ProducerConfig)));
        return services;
    }
}