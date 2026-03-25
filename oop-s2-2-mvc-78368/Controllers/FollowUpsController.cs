using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;

namespace oop_s2_2_mvc_78368.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FollowUpsController> _logger;

    public FollowUpsController(ApplicationDbContext context, ILogger<FollowUpsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Index()
    {
        var followUps = await _context.FollowUps
            .Include(f => f.Inspection)
            .ThenInclude(i => i.Premises) // include related data for display
            .ToListAsync();
        return View(followUps);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        if (followUp.Status == "Closed" && followUp.ClosedDate == null)
        {
            _logger.LogWarning("Attempt to close follow-up without ClosedDate");
            ModelState.AddModelError("", "Closed follow-up must have a ClosedDate");
        }

        if (ModelState.IsValid)
        {
            _context.FollowUps.Add(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Follow-up created for InspectionId {InspectionId}", followUp.InspectionId);

            return RedirectToAction("Index", "Dashboard");
        }

        return View(followUp);
    }
}
