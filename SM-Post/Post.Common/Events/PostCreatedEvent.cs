using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent
{
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime PostDate { get; set; }
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }
}
