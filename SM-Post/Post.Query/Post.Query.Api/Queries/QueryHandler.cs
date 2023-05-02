using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository _postReposiroty;
    public QueryHandler(IPostRepository postReposiroty)
    {
        _postReposiroty = postReposiroty;
    }
    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await _postReposiroty.GetAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindCommentedPostsQuery query)
    {
        return await _postReposiroty.GetCommentedAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindLikedPostQuery query)
    {
        return await _postReposiroty.GetLikedAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query)
    {
        return await _postReposiroty.GetByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await _postReposiroty.GetByIdAsync(query.Id);
        return new List<PostEntity>() { post };
        
    }
}
