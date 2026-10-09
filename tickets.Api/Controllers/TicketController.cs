using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using tickets.Api.Common;
using tickets.Api.Dtos;
using tickets.Api.Entity;
using tickets.Api.Enums;
using tickets.Api.Services;

namespace tickets.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TicketController(TicketService tictService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<TicketResponse>> CreateTicket(
        [FromQuery, BindRequired] Priority priority,
        TicketCreateRequest request
    )
    {
        var ticket = await tictService.CreateTicket(
            request,
            priority,
            User.GetUserId(),
            User.GetUserName()
        );
        return Ok(ticket);
    }

    [HttpGet("GetTicketsList")]
    public async Task<ActionResult<List<TicketResponse>>> GetListOfTickets(
        [FromQuery] Pagination pagination
    ) => Ok(await tictService.GetTicketsList(User.GetUserId(), User.IsInRole("Admin"), pagination));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketResponse>> GetTicketById(Guid id)
    {
        var ticket = await tictService.GetTicketById(id, User.GetUserId(), User.IsInRole("Admin"));
        return ticket is not null ? ticket : NotFound("Ticket Not Found");
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketResponse>> UpdateTicket(
        Guid id,
        [FromQuery] Priority? priority,
        TicketUpdateRequest request
    )
    {
        var ticket = await tictService.UpdateTicket(
            id,
            request,
            priority,
            User.GetUserId(),
            User.IsInRole("Admin")
        );
        return ticket is not null ? ticket : NotFound("Ticket not found");
    }

    [HttpPatch("{id:guid}/close")]
    public async Task<IActionResult> CloseTicket(Guid id) =>
        await tictService.CloseTicket(id, User.GetUserId(), User.IsInRole("Admin"))
            ? NoContent()
            : NotFound("Ticket not found");

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTicket(Guid id) =>
        await tictService.DeleteTicket(id, User.GetUserId(), User.IsInRole("Admin"))
            ? NoContent()
            : NotFound("Ticket not found");

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TicketResponse>> UpdateTicketStatus(
        Guid id,
        [FromQuery] Status status
    )
    {
        var ticket = await tictService.UpdateStatus(
            id,
            status,
            User.GetUserId(),
            User.IsInRole("Admin")
        );
        return ticket is not null ? ticket : NotFound("Ticket not found");
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/assign")]
    public async Task<ActionResult<TicketResponse>> AssignTicket(
        Guid id,
        TicketAssignRequest request
    )
    {
        if (request.UserId is null && string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("UserId or Username is required");
        var ticket = await tictService.AssignTicket(id, request, User.GetUserId());
        return ticket is not null ? ticket : NotFound("Ticket or User not found");
    }
}
