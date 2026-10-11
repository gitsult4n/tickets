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
        using var db = CreateDb();
        var service = new TicketService(db);
        var owner = Guid.NewGuid();

        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            owner,
            "sultan"
        );

        // the returned DTO
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Title", created.Title);
        Assert.Equal("Description", created.Description);
        Assert.Equal("High", created.Priority);
        Assert.Equal("Open", created.Status);
        Assert.Equal("sultan", created.CreatedByName);
        Assert.Null(created.AssignedToUserId);

        // the saved row
        var saved = await db.Tickets.SingleAsync(t => t.Id == created.Id);
        Assert.Equal(owner, saved.CreatedBy);
    }
}
