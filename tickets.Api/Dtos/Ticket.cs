using System.ComponentModel.DataAnnotations;
using tickets.Api.Entity;
using tickets.Api.Enums;

namespace tickets.Api.Dtos;

using System.ComponentModel.DataAnnotations;

public record TicketCreateRequest(
    [
        MaxLength(30, ErrorMessage = "Title Max Length is 30 Characters"),
        Required(ErrorMessage = "Title is Required")
    ]
        string Title,
    [
        MaxLength(400, ErrorMessage = "Description Max Length is 400 Characters"),
        Required(ErrorMessage = "Description is Required")
    ]
        string Description
// [Required(ErrorMessage = "Priority is Required")]  Priority
);

public record TicketUpdateRequest(
    [MaxLength(30, ErrorMessage = "Title Max Length is 30 Characters")] string? Title,
    [MaxLength(400, ErrorMessage = "Description Max Length is 400 Characters")] string? Description,
    Priority? Priority
);

public record TicketStatusRequest(Status Status);

public record TicketAssignRequest(Guid? UserId, string? Username);

public record CommentCreateRequest(
    [
        MaxLength(300, ErrorMessage = "Comment Max Length is 300 Characters"),
        Required(ErrorMessage = "Comment is Required")
    ]
        string Content
);

public record TicketResponse(
    Guid Id,
    Guid? AssignedToUserId,
    string Title,
    string Description,
    List<Comment> Comments,
    string Priority,
    string Status,
    string? AssignedToUserName,
    string CreatedByName
);
