using System;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infra;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregate;

namespace Post.Cmd.Infra.Stores;

public class EventStore : IEventStore
{

    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepostiroty, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepostiroty;
        _eventProducer = eventProducer;
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
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");        
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (exprectedVersion != -1 && eventStream[^1].Version != exprectedVersion)
            throw new ConcurrencyException();

        var version = exprectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                AggregateIdentifier = aggregateId,
                Timestamp = DateTime.Now,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            await _eventStoreRepository.SaveAsync(eventModel);            

            await _eventProducer.ProduceAsync(topic, @event);

        }

        
    }
}
