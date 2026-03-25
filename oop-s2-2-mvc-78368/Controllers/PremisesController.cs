using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;

public class PremisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PremisesController> _logger;

    public PremisesController(ApplicationDbContext context, ILogger<PremisesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Premises page viewed by {User}", User.Identity?.Name ?? "Anonymous");
        var premises = await _context.Premises.ToListAsync();
        return View(premises);
    }

    public IActionResult Create()
    {
        _logger.LogInformation("Create Premises page opened by {User}", User.Identity?.Name ?? "Anonymous");
        return View();
    }
}