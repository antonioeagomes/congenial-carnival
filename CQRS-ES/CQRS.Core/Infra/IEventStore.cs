using CQRS.Core.Events;

namespace CQRS.Core.Infra;

public interface IEventStore
{
    Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int exprectedVersion);
    Task<List<BaseEvent>> GetEventAsync(Guid aggregateId);

    Task<List<Guid>> GetAllAggregateIdAsync();
}
