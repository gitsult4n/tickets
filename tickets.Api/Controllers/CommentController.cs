using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tickets.Api.Common;
using tickets.Api.Dtos;
using tickets.Api.Entity;
using tickets.Api.Services;

namespace tickets.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CommentController(CommentService commentService) : ControllerBase
{
    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<Comment>> AddComment(Guid id, CommentCreateRequest request)
    {
        var comment = await commentService.AddComment(
            id,
            request,
            User.GetUserId(),
            User.IsInRole("Admin")
        );
        return comment is not null ? comment : NotFound("Ticket not found");
    }
}
