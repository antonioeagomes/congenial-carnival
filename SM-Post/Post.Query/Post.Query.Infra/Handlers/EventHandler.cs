using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infra.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task On(CommentAddedEvent e)
    {
        var comment = new CommentEntity
        {
            Comment = e.Comment,
            CommentDate = e.CommentDate,
            CommentId = e.CommentId,
            Username = e.Username,
            PostId = e.Id,
            WasEdited = false
        };

        await _commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentRemovedEvent e)
    {      
        await _commentRepository.DeleteAsync(e.CommentId);
    }

    public async Task On(CommentUpdatedEvent e)
    {
        var comment = await _commentRepository.GetByIdAsync(e.CommentId);

        if (comment == null) return;

        comment.Comment = e.Comment;
        comment.WasEdited = true;
        comment.CommentDate = e.EditDate;

        await _commentRepository.UpdateAsync(comment);
    }

    public async Task On(ContentUpdatedEvent e)
    {
        var post = await _postRepository.GetByIdAsync(e.Id);

        if (post == null) return;

        post.Content = e.Content;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(PostCreatedEvent e)
    {
        var post = new PostEntity
        {
            PostId = e.Id,
            Author = e.Author,
            DatePosted = e.PostDate,
            Content = e.Content
        };

        await _postRepository.CreateAsync(post);
    }

    public async Task On(PostDeletedEvent e)
    {
        await _postRepository.DeleteAsync(e.Id);
    }

    public async Task On(PostLikedEvent e)
    {
        var post = await _postRepository.GetByIdAsync(e.Id);
        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }
}
