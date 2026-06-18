using FinancialKDS.Data;
using FinancialKDS.Models;
using FinancialKDS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialKDS.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RegressionService _regressionService;
        private readonly NpvService _npvService;

        public ProjectsController(
            ApplicationDbContext context,
            RegressionService regressionService,
            NpvService npvService)
        {
            _context = context;
            _regressionService = regressionService;
            _npvService = npvService;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.AnalysisResult)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CashFlows)
                .Include(p => p.ForecastResults)
                .Include(p => p.AnalysisResult)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> Analyze(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CashFlows)
                .Include(p => p.ForecastResults)
                .Include(p => p.AnalysisResult)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (project.CashFlows.Count < 2)
            {
                TempData["ErrorMessage"] = "Regresyon analizi için en az 2 nakit akışı verisi girilmelidir.";
                return RedirectToAction(nameof(Details), new { id = project.Id });
            }

            try
            {
                if (project.ForecastResults.Any())
                {
                    _context.ForecastResults.RemoveRange(project.ForecastResults);
                }

                if (project.AnalysisResult != null)
                {
                    _context.AnalysisResults.Remove(project.AnalysisResult);
                }

                var orderedCashFlows = project.CashFlows
                    .OrderBy(c => c.Period)
                    .ToList();

                var regressionResult = _regressionService.CalculateForecasts(
                    orderedCashFlows,
                    project.ForecastPeriod);

                var forecasts = regressionResult.Forecasts;

                decimal npv = _npvService.CalculateNpv(
                    project.InitialInvestment,
                    project.DiscountRate,
                    forecasts);

                string decision = _npvService.GetDecision(npv);

                foreach (var forecast in forecasts)
                {
                    forecast.ProjectId = project.Id;
                    _context.ForecastResults.Add(forecast);
                }

                var analysisResult = new AnalysisResult
                {
                    ProjectId = project.Id,
                    Beta0 = regressionResult.Beta0,
                    Beta1 = regressionResult.Beta1,
                    NPV = npv,
                    Decision = decision,
                    CreatedAt = DateTime.Now
                };

                _context.AnalysisResults.Add(analysisResult);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Analiz başarıyla tamamlandı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = project.Id });
        }
    
    // Proje düzenleme sayfası
public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // Proje düzenleme kaydetme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProject = await _context.Projects.FindAsync(id);

                if (existingProject == null)
                {
                    return NotFound();
                }

                existingProject.ProjectName = project.ProjectName;
                existingProject.Description = project.Description;
                existingProject.InitialInvestment = project.InitialInvestment;
                existingProject.DiscountRate = project.DiscountRate;
                existingProject.ForecastPeriod = project.ForecastPeriod;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Proje bilgileri başarıyla güncellendi.";
                return RedirectToAction(nameof(Details), new { id = project.Id });
            }

            return View(project);
        }

        // Proje silme onay sayfası
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CashFlows)
                .Include(p => p.ForecastResults)
                .Include(p => p.AnalysisResult)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // Proje silme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CashFlows)
                .Include(p => p.ForecastResults)
                .Include(p => p.AnalysisResult)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (project.CashFlows.Any())
            {
                _context.CashFlows.RemoveRange(project.CashFlows);
            }

            if (project.ForecastResults.Any())
            {
                _context.ForecastResults.RemoveRange(project.ForecastResults);
            }

            if (project.AnalysisResult != null)
            {
                _context.AnalysisResults.Remove(project.AnalysisResult);
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Proje ve ilişkili analiz verileri başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
