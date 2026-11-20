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

        // POST: CashFlows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Period,Amount")] CashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = cashFlow.ProjectId });
            }
            return RedirectToAction("Details", "Projects", new { id = cashFlow.ProjectId });
        }

        // POST: CashFlows/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int projectId)
        {
            var cashFlow = await _context.FindAsync<CashFlow>(id);
            if (cashFlow != null)
            {
                _context.Remove(cashFlow);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}