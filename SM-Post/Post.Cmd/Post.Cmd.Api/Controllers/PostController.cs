using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infra;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public PostController(ILogger<PostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;

            try
            {
                await _commandDispatcher.SendAsync(command);

                return Created(nameof(NewPost), new NewPostResponse("New post created successfully", id, command.Author, command.Content));

            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogWarning(ioe, "Client made bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ioe.Message
                });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error while creating a new post");

                return BadRequest(new NewPostResponse(ex.Message, id));
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditPost(Guid id, EditContentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);
                return Ok(new EditPostResponse("Edited", command.Id, command.Content));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error while editing a post");

                return BadRequest(new EditPostResponse(ex.Message, command.Id));
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemovePost(Guid id, DeletePostCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error to delete a post");

                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> LikePost(Guid id, LikePostCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error to like a post");

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("restore")]
        public async Task<ActionResult> RestoreReadDB()
        {
            try
            {
                await _commandDispatcher.SendAsync(new RestoreReadDbCommand());
                return Ok();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error to restore the read db");

                return BadRequest(ex.Message);
            }
        }

    }
}