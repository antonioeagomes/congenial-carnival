using CQRS.Core.Infra;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public CommentController(ILogger<CommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> AddComment(Guid id, AddCommentCommand command)
    {

        command.Id = id;

        try
        {
            await _commandDispatcher.SendAsync(command);

            return Ok();

        }

        catch (Exception ex)
        {

            _logger.LogError(ex, "Error while creating a new post");

            return BadRequest(ex.Message);
        }

    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> EditComment(Guid id, EditContentCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return Ok();
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error while editing a post");

            return BadRequest(ex.Message);
        }

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveComment(Guid id, DeletePostCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to delete a post");

            return BadRequest(ex.Message);
        }
    }
}
