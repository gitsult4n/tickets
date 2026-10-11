using System.ComponentModel.DataAnnotations;

namespace tickets.Api.Dtos;

public record UserRequest(
    [Required, MinLength(3), MaxLength(30)] string Username,
    [Required, MinLength(6), MaxLength(130)] string Password
);

public record UserResponse(string? Token);
