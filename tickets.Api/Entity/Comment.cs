using System.ComponentModel.DataAnnotations;

namespace tickets.Api.Entity;

public class Comment : BaseEntity
{
    [MaxLength(300)]
    public required string Content { get; set; }
    public Guid TicketId { get; set; }
}
