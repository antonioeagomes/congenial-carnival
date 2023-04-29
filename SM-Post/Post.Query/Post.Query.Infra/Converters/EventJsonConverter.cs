using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infra.Converters
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }
        public override BaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out var document))
                throw new JsonException($"Fail to parse {nameof(JsonDocument)}");

            if (!document.RootElement.TryGetProperty("Type", out var type))
                throw new JsonException("Couldn't detect the Type discriminator property");

            var typeDiscriminator = type.GetString();
            var json = document.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
                nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
                nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
                nameof(PostDeletedEvent) => JsonSerializer.Deserialize<PostDeletedEvent>(json, options),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
                nameof(ContentUpdatedEvent) => JsonSerializer.Deserialize<ContentUpdatedEvent>(json, options),
                _ => throw new JsonException($"{typeDiscriminator} is not supported yet")
            };

        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}