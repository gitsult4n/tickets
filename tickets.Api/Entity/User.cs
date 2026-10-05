using System.ComponentModel.DataAnnotations;
using tickets.Api.Enums;

namespace tickets.Api.Entity;

public class User : BaseEntity
{
    [MaxLength(30)]
    public required string Username { get; set; }

    [MaxLength(130)]
    public required string Password { get; set; }
    public Role Role { get; set; } = Role.User;
}
