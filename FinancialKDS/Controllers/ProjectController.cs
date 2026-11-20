using FinancialKDS.Data;
using FinancialKDS.Models;
using FinancialKDS.Models.DTOs;
using FinancialKDS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinancialKDS.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly FinancialAnalysisService _analysisService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(FinancialAnalysisService analysisService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _analysisService = analysisService;
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: /Project/Index
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(projects);
        }

        // GET: /Project/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();

            var cashFlows = await _context.CashFlows
                .Where(cf => cf.ProjectId == id)
                .OrderBy(cf => cf.Period)
                .ToListAsync();

            ViewBag.CashFlows = cashFlows;
            return View("Details", project);
        }

        // POST: /Project/AddCashFlow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCashFlow(int projectId, int period, decimal amount)
        {
            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
            if (project == null) return NotFound();

            var cashFlow = new CashFlow
            {
                ProjectId = projectId,
                Period = period,
                Amount = amount
            };

            _context.CashFlows.Add(cashFlow);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: /Project/DeleteCashFlow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCashFlow(int id, int projectId)
        {
            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
            if (project == null) return NotFound();

            var cashFlow = await _context.CashFlows.FindAsync(id);
            if (cashFlow != null && cashFlow.ProjectId == projectId)
            {
                _context.CashFlows.Remove(cashFlow);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // GET: /Project/Analyze/1
        public async Task<IActionResult> Analyze(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();

            var historicalCashFlows = await _context.CashFlows
                .Where(cf => cf.ProjectId == id)
                .OrderBy(cf => cf.Period)
                .Select(cf => cf.Amount)
                .ToListAsync();

            if (!historicalCashFlows.Any())
            {
                TempData["ErrorMessage"] = "Analiz yapılabilmesi için lütfen önce en az bir nakit akışı ekleyin.";
                return RedirectToAction(nameof(Details), new { id = project.Id });
            }

            AnalysisResultDto analysisResult = _analysisService.AnalyzeProject(
                projectId: project.Id,
                projectName: project.Name,
                initialInvestment: project.InitialInvestment,
                discountRatePercent: project.DiscountRate,
                historicalCashFlows: historicalCashFlows,
                forecastPeriods: project.ForecastPeriods
            );

            return View("Analyze", analysisResult);
        }

        // GET: /Project/ExportPdf/1
        public async Task<IActionResult> ExportPdf(int id)
        {
            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();

            var historicalCashFlows = await _context.CashFlows
                .Where(cf => cf.ProjectId == id)
                .OrderBy(cf => cf.Period)
                .Select(cf => cf.Amount)
                .ToListAsync();

            var analysis = _analysisService.AnalyzeProject(
                project.Id, project.Name, project.InitialInvestment,
                project.DiscountRate, historicalCashFlows, project.ForecastPeriods
            );

            byte[] pdfBytes = _analysisService.GeneratePdfReport(analysis);
            return File(pdfBytes, "application/pdf", $"{project.Name}_Finansal_Rapor.pdf");
        }

        // GET: /Project/ExportExcel/1
        public async Task<IActionResult> ExportExcel(int id)
        {
            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();

            var historicalCashFlows = await _context.CashFlows
                .Where(cf => cf.ProjectId == id)
                .OrderBy(cf => cf.Period)
                .Select(cf => cf.Amount)
                .ToListAsync();

            var analysis = _analysisService.AnalyzeProject(
                project.Id, project.Name, project.InitialInvestment,
                project.DiscountRate, historicalCashFlows, project.ForecastPeriods
            );

            byte[] excelBytes = _analysisService.GenerateExcelReport(analysis);

            // Uzantı ve Content-Type CSV olarak ayarlandı (Excel çift tıklandığında otomatik açar)
            return File(excelBytes, "text/csv; charset=utf-8", $"{project.Name}_Finansal_Rapor.csv");
        }

        // GET: /Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            var userId = GetUserId();
            project.UserId = userId;
            project.CreatedDate = DateTime.Now;

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("CashFlows");

            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: /Project/Edit/1
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();

            return View("Edit", project);
        }

        // POST: /Project/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            var userId = GetUserId();
            var existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (existingProject == null) return NotFound();

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.InitialInvestment = project.InitialInvestment;
            existingProject.DiscountRate = project.DiscountRate;
            existingProject.ForecastPeriods = project.ForecastPeriods;

            try
            {
                _context.Projects.Update(existingProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.Id == id && e.UserId == userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: /Project/Compare
        public async Task<IActionResult> Compare()
        {
            var userId = GetUserId();
            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var comparisonList = new List<AnalysisResultDto>();

            foreach (var project in projects)
            {
                var historicalCashFlows = await _context.CashFlows
                    .Where(cf => cf.ProjectId == project.Id)
                    .OrderBy(cf => cf.Period)
                    .Select(cf => cf.Amount)
                    .ToListAsync();

                // Eğer geçmiş nakit akışı varsa analizi çalıştırıp listeye ekliyoruz
                if (historicalCashFlows.Any())
                {
                    var analysis = _analysisService.AnalyzeProject(
                        projectId: project.Id,
                        projectName: project.Name,
                        initialInvestment: project.InitialInvestment,
                        discountRatePercent: project.DiscountRate,
                        historicalCashFlows: historicalCashFlows,
                        forecastPeriods: project.ForecastPeriods
                    );
                    comparisonList.Add(analysis);
                }
            }

            return View("Compare", comparisonList);
        }

        // POST: /Project/Simulate
        [HttpPost]
        public async Task<IActionResult> Simulate([FromBody] ScenarioParametersDto scenario)
        {
            if (!ModelState.IsValid || scenario == null)
            {
                return BadRequest(new { success = false, message = "Geçersiz simülasyon parametreleri." });
            }

            var userId = GetUserId();
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == scenario.ProjectId && p.UserId == userId);
            if (project == null) return NotFound(new { success = false, message = "Proje bulunamadı veya erişim yetkiniz yok." });

            var historicalCashFlows = await _context.CashFlows
                .Where(cf => cf.ProjectId == scenario.ProjectId)
                .OrderBy(cf => cf.Period)
                .Select(cf => cf.Amount)
                .ToListAsync();

            var currentAnalysis = _analysisService.AnalyzeProject(
                project.Id,
                project.Name,
                project.InitialInvestment,
                project.DiscountRate,
                historicalCashFlows,
                project.ForecastPeriods
            );

            var simulatedResult = _analysisService.SimulateScenario(currentAnalysis, scenario);
            decimal improvementAmount = simulatedResult.OriginalNPV - currentAnalysis.OriginalNPV;

            return Json(new
            {
                success = true,
                originalNpv = currentAnalysis.OriginalNPV,
                simulatedNpv = simulatedResult.OriginalNPV,
                improvementAmount = improvementAmount,
                simulatedCashFlows = simulatedResult.ProjectedCashFlows,
                statusDecision = simulatedResult.OriginalNPV >= 0 ? "Kabul Edilebilir (Simüle Edildi)" : "Reddedilmeli"
            });
        }
    }
}