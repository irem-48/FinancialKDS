using FinancialKDS.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialKDS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<CashFlow> CashFlows { get; set; }
        public DbSet<ForecastResult> ForecastResults { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
    }
}