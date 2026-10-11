using System.ComponentModel.DataAnnotations;

namespace tickets.Api.Dtos;

public record CommentDtoRequest(
    [
        MaxLength(300, ErrorMessage = "Comment Max Length is 300 Characters"),
        Required(ErrorMessage = "Comment is Required")
    ]
        string Content
);
