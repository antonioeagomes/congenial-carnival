using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;

public interface IQueryHandler
{
    Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
    Task<List<PostEntity>> HandleAsync(FindCommentedPostsQuery query);
    Task<List<PostEntity>> HandleAsync(FindLikedPostQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);

}