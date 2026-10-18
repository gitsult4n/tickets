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
    [HttpPost("create")]
    public async Task<ActionResult<Comment>> AddComment(Guid id, CommentDtoRequest request)
    {
        var comment = await commentService.AddComment(
            id,
            request,
            User.GetUserId(),
            User.IsAdmin(),
            User.GetUserName()
        );
        return comment is not null ? comment : NotFound("Ticket not found");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveComment(Guid id) =>
        await commentService.DeleteComment(id, User.GetUserId(), User.IsAdmin())
            ? Ok("Comment deleted Successfully")
            : NotFound("comment not found");
}
