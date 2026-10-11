using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Enums;
using tickets.Api.Services;

namespace tickets.Tests.TicketsTest;

public class TicketDeleteTests
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

    [Fact]
    public async Task DeleteTicket_ByOtherUser_ReturnsFalse()
    {
        await using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();
        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            owner,
            "sultan"
        );

        var deleted = await service.DeleteTicket(created!.Id, Guid.NewGuid(), isAdmin: false);

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteTicket_ByCreated_ReturnsTrue()
    {
        await using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();
        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            owner,
            "sultan"
        );

        var deleted = await service.DeleteTicket(created!.Id, owner, isAdmin: false);

        Assert.True(deleted);
    }

    [Fact]
    public async Task DeleteTicket_ByAdmin_ReturnsTrue()
    {
        await using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();
        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            owner,
            "sultan"
        );

        var deleted = await service.DeleteTicket(created!.Id, Guid.NewGuid(), isAdmin: true);

        Assert.True(deleted);
    }
}
