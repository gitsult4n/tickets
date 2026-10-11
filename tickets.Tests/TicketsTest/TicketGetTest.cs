using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using tickets.Api.Db;
using tickets.Api.Dtos;
using tickets.Api.Entity;
using tickets.Api.Enums;
using tickets.Api.Services;

namespace tickets.Tests.TicketsTest;

public class TicketGetTest
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
    public async Task GetTicketById_ByAdminCreatedAndOtherUser()
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

        var byAdmin = await service.GetTicketById(created!.Id, Guid.NewGuid(), isAdmin: true);
        var byCreated = await service.GetTicketById(created.Id, owner, isAdmin: false);
        var byOther = await service.GetTicketById(created.Id, Guid.NewGuid(), isAdmin: false);

        var assigned = new User { Username = "ali", Password = "Ali12345" };
        db.Users.Add(assigned);
        await db.SaveChangesAsync();
        await service.AssignTicket(created.Id, new TicketAssignRequest(assigned.Id, null), Guid.NewGuid());
        var byAssigned = await service.GetTicketById(created.Id, assigned.Id, isAdmin: false);

        Assert.NotNull(byAdmin);
        Assert.NotNull(byCreated);
        Assert.NotNull(byAssigned);
        Assert.Null(byOther);
    }
}