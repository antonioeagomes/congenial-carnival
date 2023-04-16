using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId;
    public string? Username { get; set; }
}