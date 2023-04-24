using CQRS.Core.Events;

namespace CQRS.Core.Producers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T eventProduced) where T : BaseEvent;


}