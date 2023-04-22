using System;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infra;
using Post.Cmd.Domain.Aggregate;

namespace Post.Cmd.Infra.Stores;

public class EventStore : IEventStore
{

    private readonly IEventStoreRepository _eventStoreRepository;

    public EventStore(IEventStoreRepository eventStoreRepostiroty)
    {
        _eventStoreRepository = eventStoreRepostiroty;
    }

    public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException("Incorrect ID");

        return eventStream.OrderBy(a => a.Version).Select(e => e.EventData).ToList();
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int exprectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (exprectedVersion != -1 && eventStream[^1].Version != exprectedVersion)
            throw new ConcurrencyException();

        var version = exprectedVersion;

        foreach (var item in events)
        {
            version++;
            item.Version = version;
            var eventType = item.GetType().Name;
            var eventModel = new EventModel
            {
                AggregateIdentifier = aggregateId,
                Timestamp = DateTime.Now,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = item
            };

            await _eventStoreRepository.SaveAsync(eventModel);
        }

        
    }
}
