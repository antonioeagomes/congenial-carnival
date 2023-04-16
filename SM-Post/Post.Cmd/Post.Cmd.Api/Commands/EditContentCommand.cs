using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands;

public class EditContentCommand : BaseCommand
{
    public string? Content { get; set; }
}