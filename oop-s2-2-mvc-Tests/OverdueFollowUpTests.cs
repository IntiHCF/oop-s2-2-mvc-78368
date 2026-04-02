using Xunit;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;
using System;
using System.Linq;

public class OverdueFollowUpTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void OverdueFollowUps_ReturnsCorrectItems()
    {
        var context = GetDbContext();

        context.FollowUps.AddRange(
            new FollowUp { DueDate = DateTime.Now.AddDays(-5), Status = "Open" },
            new FollowUp { DueDate = DateTime.Now.AddDays(5), Status = "Open" },
            new FollowUp { DueDate = DateTime.Now.AddDays(-2), Status = "Closed" }
        );

        context.SaveChanges();

        var result = context.FollowUps
            .Where(f => f.DueDate < DateTime.Now && f.Status == "Open")
            .ToList();

        Assert.Single(result);
    }
}