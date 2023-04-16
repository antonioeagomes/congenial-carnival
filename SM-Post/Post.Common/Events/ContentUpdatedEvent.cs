using CQRS.Core.Events;

namespace Post.Common.Events;

public class ContentUpdatedEvent : BaseEvent
{
    public string? Content { get; set; }
    public ContentUpdatedEvent() : base(nameof(ContentUpdatedEvent))
    {
    }
}
