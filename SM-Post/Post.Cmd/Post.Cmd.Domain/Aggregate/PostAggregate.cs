using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregate;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {

    }

    public PostAggregate(Guid id, string author, string content)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Content = content,
            PostDate = DateTime.Now
        });
    }

    public void Apply(PostCreatedEvent e)
    {
        _id = e.Id;
        _active = true;
        _author = e.Author;
    }

    public void EditContent(string content)
    {
        ValidatePost();

        if (string.IsNullOrEmpty(content))
            throw new InvalidOperationException(string.Format("The value of {0} can't be empty. Please provide a valid {0}", nameof(content)));

        RaiseEvent(new ContentUpdatedEvent
        {
            Id = _id,
            Content = content
        });
    }

    public void Apply(ContentUpdatedEvent e)
    {
        _id = e.Id;
    }

    public void LikePost()
    {
        ValidatePost();

        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
    }

    public void Apply(PostLikedEvent e)
    {
        _id = e.Id;
    }

    public void DeletePost(string username)
    {
        ValidatePost();

        if (!_author?.Equals(username, StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            throw new InvalidOperationException("Cannot remove this post");
        }

        RaiseEvent(new PostDeletedEvent
        {
            Id = _id
        });
    }

    public void Apply(PostDeletedEvent e)
    {
        _id = e.Id;
        _active = false;
    }

    public void AddComment(string comment, string username)
    {
        ValidatePost();

        if (string.IsNullOrEmpty(comment))
            throw new InvalidOperationException(string.Format("The value of {0} can't be empty. Please provide a valid {0}", nameof(comment)));

        if (string.IsNullOrEmpty(username))
            throw new InvalidOperationException("You must be logged in to comment");

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Username = username,
            Comment = comment,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent e)
    {
        _id = e.Id;
        _comments.Add(e.CommentId, new Tuple<string, string>(e.Comment, e.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        ValidatePost();

        if (!_comments[commentId].Item2?.Equals(username, StringComparison.CurrentCultureIgnoreCase) ?? false)
            throw new InvalidOperationException("You can't edit this comment");

        if (string.IsNullOrEmpty(comment))
            throw new InvalidOperationException(string.Format("The value of {0} can't be empty. Please provide a valid {0}", nameof(comment)));

        if (string.IsNullOrEmpty(username))
            throw new InvalidOperationException("You must be logged in to comment");

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now
        });
    }

    public void Apply(CommentUpdatedEvent e)
    {
        _id = e.Id;
        _comments[e.CommentId] = new Tuple<string, string>(e.Comment, e.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_comments[commentId].Item2?.Equals(username, StringComparison.CurrentCultureIgnoreCase) ?? false)
            throw new InvalidOperationException("You can't edit this comment");

        if (string.IsNullOrEmpty(username))
            throw new InvalidOperationException("You must be logged in to comment");

        RaiseEvent(new CommentRemovedEvent
        {
            Id = _id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent e)
    {
        _id = e.Id;
        _comments.Remove(e.CommentId);
    }

    private void ValidatePost()
    {
        if (!_active) throw new InvalidOperationException("This post is no longer active.");
    }


}