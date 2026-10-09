using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;

namespace tickets.Api.Services;

public sealed class CommentService(AppDbContext db)
{
    public async Task<Comment?> AddComment(
        Guid ticketId,
        CommentCreateRequest request,
        Guid userId,
        bool isAdmin
    )
    {
        var canComment = await db.Tickets.AnyAsync(t =>
            t.Id == ticketId && (isAdmin || t.CreatedBy == userId || t.AssignedToUserId == userId)
        );
        if (!canComment)
        {
            return null;
        }

        var comment = new Comment
        {
            Content = request.Content,
            TicketId = ticketId,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
        };
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return comment;
    }
}
