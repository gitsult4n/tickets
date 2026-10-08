using System.ComponentModel.DataAnnotations;
using tickets.Api.Enums;

namespace tickets.Api.Entity;

public class Ticket : BaseEntity
{
    [MaxLength(30)]
    public required string Title { get; set; }

    [MaxLength(400)]
    public required string Description { get; set; }

    public List<Comment>? Comments { get; set; }
    public required Priority Priority { get; set; }
    public Status Status { get; set; } = Status.Open;
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    [MaxLength(30)]
    public required string CreatedByName { get; set; }
}
