using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands;

public class EditCommentCommand : BaseCommand
{
    public Guid CommentId;
    public string Comment { get; set; }

    public string Username { get; set; }
}
