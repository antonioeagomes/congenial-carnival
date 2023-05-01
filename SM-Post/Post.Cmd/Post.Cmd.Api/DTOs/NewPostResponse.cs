using Post.Common.DTOs;

namespace Post.Cmd.Api.DTOs;

public class NewPostResponse : BaseResponse
{
    public NewPostResponse(string message, Guid id, string author, string content)
    {
        Id = id;
        Author = author;
        Content = content;
        Message = message;
    }

     public NewPostResponse(string message, Guid id)
    {
        Id = id;
        Message = message;
    }

    public Guid Id { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
}
