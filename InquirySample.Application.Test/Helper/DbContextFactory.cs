using InquirySample.Infrastruture.Persistance;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Application.Test.Helper;

public static class DbContextFactory
{
    public static ApplicationDbContext Create(string? dbName = null) =>
        new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options);
}
