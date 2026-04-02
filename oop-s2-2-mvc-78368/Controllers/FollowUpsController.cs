using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;

namespace oop_s2_2_mvc_78368.Controllers;

[Authorize(Roles = "Admin,Inspector,Viewer")]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FollowUpsController> _logger;

    public FollowUpsController(ApplicationDbContext context, ILogger<FollowUpsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Index (all roles can see)
    public async Task<IActionResult> Index()
    {
        var followUps = await _context.FollowUps
            .Include(f => f.Inspection)
            .ThenInclude(i => i.Premises)
            .ToListAsync();
        return View(followUps);
    }

    // Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps
            .Include(f => f.Inspection)
            .ThenInclude(i => i.Premises)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (followUp == null) return NotFound();
        return View(followUp);
    }

    // ---------------- CREATE ----------------
    [Authorize(Roles = "Admin,Inspector")]
    public IActionResult Create()
    {
        // 1️⃣ Pass a new FollowUp model so the view has something to bind to
        var model = new FollowUp();

        // 2️⃣ Populate the dropdown list for Inspections
        ViewBag.InspectionList = _context.Inspections
            .Include(i => i.Premises) // so you can show Premises name
            .ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    [ActionName("Create")]
    public async Task<IActionResult> CreatePost(FollowUp followUp)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.FollowUps.Add(followUp);
                await _context.SaveChangesAsync();
                _logger.LogInformation("FollowUp created for InspectionId {InspectionId}", followUp.InspectionId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating follow-up for InspectionId {InspectionId}", followUp.InspectionId);
                return View("Error");
            }
        }
        ViewBag.InspectionList = _context.Inspections.ToList();
        return View(followUp);
    }

    // ---------------- EDIT ----------------
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp == null) return NotFound();

        ViewBag.InspectionList = _context.Inspections.ToList();
        return View(followUp);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPost(int id, FollowUp followUp)
    {
        if (id != followUp.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(followUp);
                await _context.SaveChangesAsync();
                _logger.LogInformation("FollowUp {FollowUpId} updated", followUp.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing FollowUp {FollowUpId}", followUp.Id);
                return View("Error");
            }
        }

        ViewBag.InspectionList = _context.Inspections.ToList();
        return View(followUp);
    }

    // Optional: Delete
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp == null) return NotFound();

        return View(followUp);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp != null)
        {
            _context.FollowUps.Remove(followUp);
            await _context.SaveChangesAsync();
            _logger.LogInformation("FollowUp {FollowUpId} deleted", followUp.Id);
        }
        return RedirectToAction(nameof(Index));
    }
}
