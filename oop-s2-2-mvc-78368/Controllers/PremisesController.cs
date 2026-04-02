using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;

public class PremisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PremisesController> _logger;

    public PremisesController(ApplicationDbContext context, ILogger<PremisesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ===================== INDEX =====================
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Premises page viewed by {User}", User.Identity?.Name ?? "Anonymous");
        var premises = await _context.Premises.ToListAsync();
        return View(premises);
    }

    // ===================== CREATE (GET) =====================
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        _logger.LogInformation("Create Premises page opened by {User}", User.Identity?.Name ?? "Anonymous");
        return View();
    }

    // ===================== CREATE (POST) =====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Premises premises)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Premises.Add(premises);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Premises created with ID {Id}", premises.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating premises");
                return View("Error");
            }
        }

        return View(premises);
    }

    // ===================== DETAILS =====================
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var premises = await _context.Premises.FirstOrDefaultAsync(p => p.Id == id);

        if (premises == null) return NotFound();

        return View(premises);
    }

    // ===================== EDIT (GET) =====================
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var premises = await _context.Premises.FindAsync(id);

        if (premises == null) return NotFound();

        return View(premises);
    }

    // ===================== EDIT (POST) =====================
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPost(int id, Premises premises)
    {
        if (id != premises.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(premises);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Premises {Id} updated", premises.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing premises {Id}", premises.Id);
                return View("Error");
            }
        }

        return View(premises);
    }

    // ===================== DELETE =====================
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var premises = await _context.Premises.FirstOrDefaultAsync(p => p.Id == id);
        if (premises == null) return NotFound();

        return View(premises);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var premises = await _context.Premises.FindAsync(id);
        if (premises != null)
        {
            _context.Premises.Remove(premises);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Premises {Id} deleted", id);
        }
        return RedirectToAction(nameof(Index));
    }
}