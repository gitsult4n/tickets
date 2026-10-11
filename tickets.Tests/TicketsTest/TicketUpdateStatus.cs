using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Enums;
using tickets.Api.Services;

namespace tickets.Tests.TicketsTest;

public class TicketUpdateStatus
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
    public async Task UpdateStatus_ByAdmin_ChangesStatus()
    {
        await using var db = CreateDb();
        var service = new TicketService(db);
        var created = await service.CreateTicket(
            new TicketCreateRequest("Title", "Description"),
            Priority.High,
            Guid.NewGuid(),
            "sultan"
        );

        var updated = await service.UpdateStatus(
            created!.Id,
            Status.InProgress,
            Guid.NewGuid(),
            isAdmin: true
        );

        Assert.NotNull(updated);
        Assert.Equal("InProgress", updated.Status);
    }
}