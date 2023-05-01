using CQRS.Core.Query;

namespace Post.Query.Api.Queries;

public class FindPostByIdQuery : BaseQuery
{
    public Guid Id { get; set; }
}
