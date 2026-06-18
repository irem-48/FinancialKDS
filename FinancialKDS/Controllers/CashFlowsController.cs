using FinancialKDS.Data;
using FinancialKDS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialKDS.Controllers
{
    public class CashFlowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashFlowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Project = project;
            return View(new CashFlow { ProjectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CashFlow cashFlow)
        {
            var project = await _context.Projects.FindAsync(cashFlow.ProjectId);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Project = project;

            bool periodExists = await _context.CashFlows
                .AnyAsync(c => c.ProjectId == cashFlow.ProjectId && c.Period == cashFlow.Period);

            if (periodExists)
            {
                ModelState.AddModelError(nameof(CashFlow.Period), "Bu proje için aynı dönem zaten girilmiş. Lütfen farklı bir dönem gir.");
            }

            if (ModelState.IsValid)
            {
                _context.CashFlows.Add(cashFlow);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { id = cashFlow.ProjectId });
            }

            return View(cashFlow);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cashFlow = await _context.CashFlows.FindAsync(id);

            if (cashFlow == null)
            {
                return NotFound();
            }

            int projectId = cashFlow.ProjectId;

            _context.CashFlows.Remove(cashFlow);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}