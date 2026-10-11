using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using tickets.Api.Common;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;
using tickets.Api.Services;

namespace tickets.Tests.UsersTest;

public class UserLoginTest
{
    private static AppDbContext CreateDb()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        var db = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlite(conn).Options
        );
        db.Database.EnsureCreated();
        return db;
    }

    private static UserService CreateService(AppDbContext db) =>
        new(
            db,
            new PasswordHasher<User>(),
            Options.Create(
                new JwtSettings
                {
                    Issuer = "IssuerForAll",
                    Audience = "AudiencdForAll",
                    Key = "zxB5VkMBzc8LdVOW3w+5k4o9FlZF0LEcW8szvkZe4MY=",
                }
            )
        );

    [Fact]
    public async Task Login_CorrectPassword_ReturnToken()
    {
        using var db = CreateDb();
        var service = CreateService(db);
        await service.Register(new UserRequest("sultan", "Sult4n"));
        var result = await service.Login(new UserRequest("sultan", "Sult4n"));
        Assert.False(string.IsNullOrEmpty(result?.Token));
    }

    [Fact]
    public async Task Login_IncorrectPassword_ReturnNull()
    {
        using var db = CreateDb();
        var service = CreateService(db);
        await service.Register(new UserRequest("sultan", "Sult4n321"));
        var result = await service.Login(new UserRequest("sultan", "wrong4321"));
        Assert.Null(result);
    }

    [Fact]
    public async Task Login_UnknownUser_ReturnNull()
    {
        using var db = CreateDb();
        var service = CreateService(db);
        var result = await service.Login(new UserRequest("Nobody", "stupidPWD"));
        Assert.Null(result);
    }
}
