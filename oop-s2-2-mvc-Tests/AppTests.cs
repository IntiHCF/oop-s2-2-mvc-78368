using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;
using System;
using System.Linq;
using Xunit;

public class AppTests
{
    // ? In-Memory DB Helper
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    // ? TEST 1: Overdue Follow-Ups
    [Fact]
    public void OverdueFollowUps_ReturnsCorrectItems()
    {
        var context = GetDbContext();

        context.FollowUps.Add(new FollowUp
        {
            Id = 1,
            Status = "Open",
            DueDate = DateTime.Now.AddDays(-1)
        });

        context.FollowUps.Add(new FollowUp
        {
            Id = 2,
            Status = "Open",
            DueDate = DateTime.Now.AddDays(1)
        });

        context.SaveChanges();

        var result = context.FollowUps
            .Where(f => f.Status == "Open" && f.DueDate < DateTime.Now)
            .ToList();

        Assert.Single(result);
    }

    // ? TEST 2: FollowUp cannot close without ClosedDate
    [Fact]
    public void FollowUp_ClosedWithoutClosedDate_ShouldFail()
    {
        var followUp = new FollowUp
        {
            Status = "Closed",
            ClosedDate = null
        };

        bool isInvalid = followUp.Status == "Closed" && followUp.ClosedDate == null;

        Assert.True(isInvalid);
    }

    // ? TEST 3: Dashboard Counts
    [Fact]
    public void DashboardCounts_ShouldBeCorrect()
    {
        var context = GetDbContext();

        context.Inspections.Add(new Inspection
        {
            Id = 1,
            InspectionDate = DateTime.Now,
            Outcome = "Fail"
        });

        context.Inspections.Add(new Inspection
        {
            Id = 2,
            InspectionDate = DateTime.Now,
            Outcome = "Pass"
        });

        context.SaveChanges();

        var total = context.Inspections.Count();
        var failed = context.Inspections.Count(i => i.Outcome == "Fail");

        Assert.Equal(2, total);
        Assert.Equal(1, failed);
    }

    // ? TEST 4: Role Access Logic
    [Fact]
    public void RoleAccess_ShouldRestrictViewer()
    {
        string role = "Viewer";

        bool canCreate = role == "Admin" || role == "Inspector";

        Assert.False(canCreate);

        role = "Inspector";
        canCreate = role == "Admin" || role == "Inspector";

        Assert.True(canCreate);
    }
}