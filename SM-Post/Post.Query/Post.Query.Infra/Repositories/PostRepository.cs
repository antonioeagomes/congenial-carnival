using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infra.DataAccess;

namespace Post.Query.Infra.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _contextFactory;
    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task CreateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        context.Posts.Add(post);
        _ = await context.SaveChangesAsync();

    }

    public async Task DeleteAsync(Guid id)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await GetByIdAsync(id);

        if (post == null) return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<List<PostEntity>> GetAllAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        // NoTracking executes faster because there is no need to set up changing tracking information.
        // It is useful in a read only scenario
        return await context.Posts.AsNoTracking().Include(p => p.Comments).ToListAsync();
    }

    public async Task<List<PostEntity>> GetByAuthorAsync(string author)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Posts
            .Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Author.Contains(author)).ToListAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid id)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(x => x.PostId == id);
    }

    public async Task<List<PostEntity>> GetCommentedAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Posts
            .Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any()).ToListAsync();
    }

    public async Task<List<PostEntity>> GetLikedAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Posts
            .Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Likes > 0).ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Update(post);
        _ = await context.SaveChangesAsync();
    }
}