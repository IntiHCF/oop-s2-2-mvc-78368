using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oop_s2_2_mvc_78368.Data;
using Microsoft.EntityFrameworkCore;

namespace oop_s2_2_mvc_78368.Controllers
{
    [Authorize(Roles = "Admin,Viewer")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string town, string risk)
        {
            var now = DateTime.Now;

            ViewBag.MonthlyInspections = await _context.Inspections
                .CountAsync(i => i.InspectionDate.Month == now.Month && i.InspectionDate.Year == now.Year);

            ViewBag.FailedInspections = await _context.Inspections
                .CountAsync(i => i.InspectionDate.Month == now.Month && i.InspectionDate.Year == now.Year && i.Outcome == "Fail");

            ViewBag.OverdueFollowUps = await _context.FollowUps
                .CountAsync(f => f.Status == "Open" && f.DueDate < DateTime.Now);

            var query = _context.Premises.AsQueryable();
            if (!string.IsNullOrEmpty(town))
                query = query.Where(p => p.Town == town);
            if (!string.IsNullOrEmpty(risk))
                query = query.Where(p => p.RiskRating == risk);

            ViewBag.FilteredPremises = await query.ToListAsync();

            return View();
        }
    }
}
