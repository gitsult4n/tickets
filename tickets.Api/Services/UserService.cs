using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using tickets.Api.Common;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;

namespace tickets.Api.Services;

public sealed class UserService(
    AppDbContext db,
    IPasswordHasher<User> pwdHash,
    IOptions<JwtSettings> jwt
)
{
    private readonly JwtSettings _jwt = jwt.Value;

    public async Task<bool> Register(UserRequest request)
    {
        var username = request.Username.ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Username == username))
            return false;
        var user = new User { Username = username, Password = string.Empty };
        user.Password = pwdHash.HashPassword(user, request.Password);
        db.Add(user);
        user.CreatedBy = user.Id;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<UserResponse?> Login(UserRequest request)
    {
        var username = request.Username.ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
            return null;

        var result = pwdHash.VerifyHashedPassword(user, user.Password, request.Password);
        if (result is PasswordVerificationResult.Failed)
            return null;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var token = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            [
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("name", user.Username),
                new("role", user.Role.ToString()),
            ],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new(signingKey, SecurityAlgorithms.HmacSha256)
        );
        Console.WriteLine("role");

        return new(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
