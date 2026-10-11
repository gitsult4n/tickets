using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Enums;
using tickets.Api.Services;

namespace tickets.Tests.TicketsTest;

public class TicketCreateTest
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
    public async Task CreateTicket_SavesAndReturnsTicket()
    {
        var title = "Title";
        var description = "Description";
        var priority = Priority.High;
        var userName = "sultan";

        using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();

        var created = await service.CreateTicket(
            new TicketCreateRequest(title, description),
            priority,
            owner,
            userName
        );

        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal(title, created.Title);
        Assert.Equal(description, created.Description);
        Assert.Equal(priority.ToString(), created.Priority);
        Assert.NotEqual("Closed", created.Status);
        Assert.Equal("Open", created.Status);
        Assert.NotEqual("Low", created.Priority);
        Assert.Equal("High", created.Priority);
        Assert.Equal(userName, created.CreatedByName);
        Assert.Null(created.AssignedToUserId);
        Assert.Empty(created.Comments);

        var saved = await db.Tickets.SingleAsync(t => t.Id == created.Id);
        Assert.Equal(owner, saved.CreatedBy);
        Assert.Equal(DateTime.Now.Minute, saved.CreatedAt.Minute);
        Assert.NotEqual(DateTime.UtcNow.Millisecond, saved.CreatedAt.Millisecond);
    }

    [Fact]
    public async Task CreateTicket_TwoTickets_SavesBothWithDifferentIds()
    {
        using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();

        var first = await service.CreateTicket(
            new TicketCreateRequest("First", "One"),
            Priority.Low,
            owner,
            "sultan"
        );
        var second = await service.CreateTicket(
            new TicketCreateRequest("Second", "Two"),
            Priority.High,
            owner,
            "sultan"
        );

        Assert.NotEqual(first!.Id, second!.Id);
        Assert.Equal(2, await db.Tickets.CountAsync());
    }

    [Fact]
    public async Task CreateTicket_SetsCreatedAtToNow()
    {
        using var db = CreateDb();
        var service = new TicketService(db);
        var before = DateTime.UtcNow;

        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.Medium,
            Guid.NewGuid(),
            "sultan"
        );

        var saved = await db.Tickets.SingleAsync(t => t.Id == created!.Id);
        Assert.InRange(saved.CreatedAt, before, DateTime.UtcNow);
        Assert.NotInRange(DateTime.UtcNow, before, saved.CreatedAt);
    }

    [Fact]
    public async Task CreateTicket_WithoutUserName_ReturnsNull()
    {
        using var db = CreateDb();
        var service = new TicketService(db);

        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            Guid.NewGuid(),
            null!
        );

        Assert.Null(created);
        Assert.Equal(0, await db.Tickets.CountAsync());
    }

    [Fact]
    public async Task CreateTicket_WithEmptyUserId_ReturnsNull()
    {
        await using var db = CreateDb();
        var service = new TicketService(db);

        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            Guid.Empty,
            "sultan"
        );

        Assert.Null(created);
        Assert.Equal(0, await db.Tickets.CountAsync());
    }
}
