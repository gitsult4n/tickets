using Microsoft.EntityFrameworkCore;
using tickets.Api.Entity;
using tickets.Api.Enums;

namespace tickets.Api.Db;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
}
