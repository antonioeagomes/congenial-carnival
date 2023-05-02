using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infra;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostLookUpController : ControllerBase
    {
        private readonly ILogger<PostLookUpController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;
        private const string SUCCESS = "Success";

        public PostLookUpController(ILogger<PostLookUpController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet(Name = "GetAll")]
        public async Task<ActionResult<PostLookUpResponse>> GetAll()
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

            if (posts == null || !posts.Any()) return NoContent();

            int count = posts.Count;

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = $"Found {count} post{(count > 1 ? "s" : string.Empty)}"
            });
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<PostLookUpResponse>> GetById(string id)
        {
            var result = new PostLookUpResponse();
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery(new Guid(id)));

            if (posts == null || !posts.Any()) return NoContent();

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = SUCCESS
            });
        }
        [HttpGet("author", Name = "GetByAuthor")]
        public async Task<ActionResult<PostLookUpResponse>> GetByAuthor(string author)
        {
            var result = new PostLookUpResponse();
            var posts = await _queryDispatcher.SendAsync(new FindPostByAuthorQuery(author));

            if (posts == null || !posts.Any()) return NoContent();

            int count = posts.Count;

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = $"Found {count} post{(count > 1 ? "s" : string.Empty)}"
            });
        }
        [HttpGet("commented", Name = "GetCommented")]
        public async Task<ActionResult<PostLookUpResponse>> GetCommented()
        {
            var result = new PostLookUpResponse();
            var posts = await _queryDispatcher.SendAsync(new FindCommentedPostsQuery());

            if (posts == null || !posts.Any()) return NoContent();

            int count = posts.Count;

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = $"Found {count} post{(count > 1 ? "s" : string.Empty)}"
            });
        }
        [HttpGet("liked", Name = "GetLiked")]
        public async Task<ActionResult<PostLookUpResponse>> GetLiked()
        {
            var result = new PostLookUpResponse();
            var posts = await _queryDispatcher.SendAsync(new FindLikedPostQuery());

            if (posts == null || !posts.Any()) return NoContent();

            int count = posts.Count;

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = $"Found {count} post{(count > 1 ? "s" : string.Empty)}"
            });
        }
    }
}