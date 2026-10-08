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
        [FromQuery] string? search
    ) => Ok(await tictService.GetTicketsList(User.GetUserId(), User.IsInRole("Admin"), search));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketResponse>> GetTicketById(Guid id)
    {
        var ticket = await tictService.GetTicketById(id, User.GetUserId(), User.IsInRole("Admin"));
        return ticket is not null ? Ok(ticket) : NotFound("Ticket Not Found");
    }
}
