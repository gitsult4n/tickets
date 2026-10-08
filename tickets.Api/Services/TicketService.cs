using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;
using tickets.Api.Enums;

namespace tickets.Api.Services;

public sealed class TicketService(AppDbContext db)
{
    public async Task<TicketResponse> CreateTicket(
        TicketCreateRequest request,
        Priority priority,
        Guid userId,
        string userName
    )
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Status = Status.Open,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            Priority = priority,
            CreatedByName = userName,
        };
        await db.AddAsync(ticket);
        await db.SaveChangesAsync();
        return new TicketResponse(
            Id: ticket.Id,
            AssignedToUserId: null,
            Title: ticket.Title,
            Description: ticket.Description,
            Comments: [],
            Priority: ticket.Priority.ToString(),
            Status: ticket.Status.ToString(),
            AssignedToUserName: null,
            CreatedByName: ticket.CreatedByName
        );
    }

    public async Task<List<TicketResponse>> GetTicketsList(
        Guid userId,
        bool isAdmin,
        string? search
    )
    {
        var query = db.Tickets.Where(t =>
            isAdmin || t.CreatedBy == userId || t.AssignedToUserId == userId
        );
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                EF.Functions.ILike(t.Title, $"%{search}%")
                || EF.Functions.ILike(t.Description, $"%{search}%")
                || EF.Functions.ILike(t.CreatedByName, $"%{search}%")
            );
        var tickets = await query
            .Include(t => t.Comments)
            .Include(t => t.AssignedToUser)
            .ToListAsync();
        return tickets.Select(ToResponse).ToList();
    }

    public async Task<TicketResponse?> GetTicketById(Guid id, Guid userId, bool isAdmin)
    {
        var ticket = await db
            .Tickets.Include(t => t.Comments)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t =>
                t.Id == id && (isAdmin || t.CreatedBy == userId || t.AssignedToUserId == userId)
            );
        return ticket is null ? null : ToResponse(ticket);
    }

    public async Task<TicketResponse?> UpdateTicket(
        Guid id,
        TicketUpdateRequest request,
        Guid userId,
        bool isAdmin
    )
    {
        var ticket = await db
            .Tickets.Include(t => t.Comments)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id && (isAdmin || t.CreatedBy == userId));
        if (ticket is null)
            return null;
        ticket.Title = request.Title ?? ticket.Title;
        ticket.Description = request.Description ?? ticket.Description;
        ticket.Priority = request.Priority ?? ticket.Priority;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ToResponse(ticket);
    }

    public async Task<bool> CloseTicket(Guid id, Guid userId, bool isAdmin)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t =>
            t.Id == id && (isAdmin || t.CreatedBy == userId)
        );
        if (ticket is null)
            return false;
        ticket.Status = Status.Closed;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTicket(Guid id, Guid userId, bool isAdmin) =>
        await db
            .Tickets.Where(t => t.Id == id && (isAdmin || t.CreatedBy == userId))
            .ExecuteDeleteAsync() > 0;

    public async Task<TicketResponse?> UpdateStatus(
        Guid id,
        Status status,
        Guid userId,
        bool isAdmin
    )
    {
        var ticket = await db
            .Tickets.Include(t => t.Comments)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id && (isAdmin || t.AssignedToUserId == userId));
        if (ticket is null)
            return null;
        ticket.Status = status;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ToResponse(ticket);
    }

    public async Task<TicketResponse?> AssignTicket(
        Guid id,
        TicketAssignRequest request,
        Guid adminId
    )
    {
        var username = request.Username?.ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.Id == request.UserId || u.Username == username
        );
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket is null || user is null)
            return null;
        ticket.AssignedToUserId = user.Id;
        ticket.AssignedToUser = user;
        ticket.UpdatedBy = adminId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ToResponse(ticket);
    }

    private static TicketResponse ToResponse(Ticket t) =>
        new(
            t.Id,
            t.AssignedToUserId,
            t.Title,
            t.Description,
            t.Comments ?? [],
            t.Priority.ToString(),
            t.Status.ToString(),
            t.AssignedToUser?.Username,
            t.CreatedByName
        );
}
