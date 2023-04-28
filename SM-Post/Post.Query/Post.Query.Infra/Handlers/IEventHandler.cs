using Post.Common.Events;

namespace Post.Query.Infra.Handlers;

public interface IEventHandler
{
    Task On(CommentAddedEvent e);
    Task On(CommentRemovedEvent e);
    Task On(CommentUpdatedEvent e);
    Task On(ContentUpdatedEvent e);
    Task On(PostCreatedEvent e);
    Task On(PostDeletedEvent e);
    Task On(PostLikedEvent e);
}