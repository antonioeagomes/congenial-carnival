using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infra.DataAccess;

namespace Post.Query.Infra.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task CreateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Add(comment);

        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
         using DatabaseContext context = _contextFactory.CreateDbContext();
            var comment = await GetByIdAsync(id);

            if (comment == null) return;

            context.Comments.Remove(comment);
            _ = await context.SaveChangesAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid id)
    {
         using DatabaseContext context = _contextFactory.CreateDbContext();
            return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
            context.Comments.Update(comment);

            _ = await context.SaveChangesAsync();
    }
}
