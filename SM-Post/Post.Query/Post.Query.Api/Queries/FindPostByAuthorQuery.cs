using CQRS.Core.Query;

namespace Post.Query.Api.Queries;

public class FindPostByAuthorQuery : BaseQuery
{
    public FindPostByAuthorQuery()
    {
    }

    public FindPostByAuthorQuery(string author)
    {
        Author = author;
    }

    public string Author { get; set; }
}
