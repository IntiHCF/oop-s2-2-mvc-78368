using Xunit;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;
using System;
using System.Linq;

public class DashboardTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void DashboardCounts_AreCorrect()
    {
        var context = GetDbContext();

        context.Premises.Add(new Premises
        {
            Id = 1,
            Name = "Test Place",
            Address = "123 Street",
            Town = "Dublin",
            RiskRating = "High"
        });

        context.Inspections.AddRange(
            new Inspection
            {
                InspectionDate = DateTime.Now,
                Outcome = "Pass",
                Score = 80,
                Notes = "Good",
                PremisesId = 1
            },
            new Inspection
            {
                InspectionDate = DateTime.Now,
                Outcome = "Fail",
                Score = 40,
                Notes = "Bad hygiene",
                PremisesId = 1
            },
            new Inspection
            {
                InspectionDate = DateTime.Now.AddMonths(-1),
                Outcome = "Fail",
                Score = 30,
                Notes = "Old issue",
                PremisesId = 1
            }
        );

        context.FollowUps.AddRange(
            new FollowUp { DueDate = DateTime.Now.AddDays(-3), Status = "Open" },
            new FollowUp { DueDate = DateTime.Now.AddDays(2), Status = "Open" }
        );

        context.SaveChanges();

        var monthlyInspections = context.Inspections
            .Count(i => i.InspectionDate.Month == DateTime.Now.Month);

        var failedInspections = context.Inspections
            .Count(i => i.Outcome == "Fail" && i.InspectionDate.Month == DateTime.Now.Month);

        var overdueFollowUps = context.FollowUps
            .Count(f => f.DueDate < DateTime.Now && f.Status == "Open");

        Assert.Equal(2, monthlyInspections);
        Assert.Equal(1, failedInspections);
        Assert.Equal(1, overdueFollowUps);
    }
}