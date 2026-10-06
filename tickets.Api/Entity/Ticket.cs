using System.ComponentModel.DataAnnotations;
using tickets.Api.Enums;

namespace tickets.Api.Entity;

public class Ticket : BaseEntity
{
    [MaxLength(30)]
    public required string Title { get; set; }

    [MaxLength(400)]
    public required string Description { get; set; }

    [MaxLength(300)]
    public List<Comment>? Comments { get; set; }
    public required Priority Priority { get; set; } = Priority.Low;
    public required Status Status { get; set; } = Status.Open;
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedtoUser { get; set; }
}
