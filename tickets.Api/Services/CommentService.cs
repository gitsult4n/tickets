using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;

namespace tickets.Api.Services;

public sealed class CommentService(AppDbContext db)
{
    public async Task<Comment?> AddComment(
        Guid ticketId,
        CommentDtoRequest request,
        Guid userId,
        bool isAdmin,
        string Username
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
            Username = Username,
        };
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteComment(Guid id, Guid userId, bool isAdmin) =>
        await db
            .Comments.Where(c => c.Id == id && (isAdmin || c.CreatedBy == userId))
            .ExecuteDeleteAsync() > 0;
}
