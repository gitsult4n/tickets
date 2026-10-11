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

public class UserRegisterTest
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
    public async Task Register_NewUser_WithHashed_Password()
    {
        using var db = CreateDb();
        var service = CreateService(db);
        var ok = await service.Register(new UserRequest("Sultan", "Sult4n321"));
        Assert.True(ok);
        var saved = await db.Users.SingleAsync();
        Assert.Equal("sultan", saved.Username);
        Assert.NotEqual("Sult4n321", saved.Password);
    }

    [Fact]
    public async Task Register_Existing_Username_ReturnFalse()
    {
        using var db = CreateDb();
        var service = CreateService(db);
        await service.Register(new UserRequest("Sultan", "Sult4n321"));
        var ok = await service.Register(new UserRequest("SULTAN", "SULT4N321"));
        Assert.False(ok);
        Assert.Equal(1, await db.Users.CountAsync());
    }
}
