using Post.Common.DTOs;

namespace Post.Cmd.Api.DTOs;

public class EditPostResponse : BaseResponse
{
     public EditPostResponse(string message, Guid id)
    {
        Id = id;
        Message = message;
    }

    public EditPostResponse(string message, Guid id, string content)
    {
        Id = id;
        Content = content;
        Message = message;
    }

    public Guid Id { get; set; }
    public string Content { get; set; }
    
}