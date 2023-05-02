using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregate;

namespace Post.Cmd.Infra.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer;

        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }
        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventAsync(aggregateId);

            if (events != null || events.Any())
            {
                aggregate.ReplayEvents(events);
                aggregate.Version = events.Select(v => v.Version).Max();
            }

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await _eventStore.GetAllAggregateIdAsync();

            if (aggregateIds == null || !aggregateIds.Any()) return;

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

            foreach (var id in aggregateIds)
            {
                var aggregate = await GetByIdAsync(id);

                if (aggregate == null || !aggregate.Active) continue;

                var events = await _eventStore.GetEventAsync(aggregate.Id);

                foreach (var e in events)
                {
                    await _eventProducer.ProduceAsync(topic, e);
                }

            }
        }
    }
}