using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infra.Config;

namespace Post.Cmd.Infra.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStore;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStore = mongoDatabase.GetCollection<EventModel>(config.Value?.Collection);
    }

    public async Task<List<EventModel>> FindAllAsync()
    {
        return await _eventStore.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStore.Find(e => e.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel eventModel)
    {
        await _eventStore.InsertOneAsync(eventModel).ConfigureAwait(false);
    }
}
