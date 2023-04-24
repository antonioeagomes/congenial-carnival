using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infra.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;

    public EventProducer(IOptions<ProducerConfig> config)
    {
        _config = config.Value;
    }
    public async Task ProduceAsync<T>(string topic, T eventToProduce) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(eventToProduce, eventToProduce.GetType())
        };

        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            throw new Exception($"Could't produce {eventToProduce.GetType().Name} message to topic {topic}. Due to {deliveryResult.Message}");

    }
}
