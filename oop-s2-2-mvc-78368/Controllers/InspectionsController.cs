using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_2_mvc_78368.Data;
using oop_s2_2_mvc_78368.Models;


namespace oop_s2_2_mvc_78368.Controllers
{

    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionsController> _logger;

        public InspectionsController(ApplicationDbContext context, ILogger<InspectionsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Index()
        {
            var inspections = await _context.Inspections
                .Include(i => i.Premises) // include related Premises for display
                .ToListAsync();
            return View(inspections);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewBag.PremisesList = _context.Premises.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Inspections.Add(inspection);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Inspection created for PremisesId {PremisesId} with InspectionId {InspectionId}",
                        inspection.PremisesId, inspection.Id);

                    return RedirectToAction("Details", "Premises", new { id = inspection.PremisesId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating inspection for PremisesId {PremisesId}", inspection.PremisesId);
                    return View("Error"); // Friendly failure
                }
            }

            return View(inspection);
        }
    }
}
