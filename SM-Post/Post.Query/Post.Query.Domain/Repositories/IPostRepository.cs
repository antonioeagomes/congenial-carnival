using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository
{
    Task CreateAsync(PostEntity post);
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid id);
    Task<List<PostEntity>> GetAllAsync();
    Task<PostEntity> GetByIdAsync(Guid id);
    Task<List<PostEntity>> GetByAuthorAsync(string author);
    Task<List<PostEntity>> GetLikedAsync();
    Task<List<PostEntity>> GetCommentedAsync();

}