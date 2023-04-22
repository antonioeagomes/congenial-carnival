using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands;

public class LikePostCommand : BaseCommand
{
    public string Content { get; set; }
}