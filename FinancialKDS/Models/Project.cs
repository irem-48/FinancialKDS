using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proje adı zorunludur.")]
        [Display(Name = "Proje Adı")]
        public string ProjectName { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "İlk yatırım maliyeti zorunludur.")]
        [Range(1, 999999999, ErrorMessage = "İlk yatırım maliyeti 0'dan büyük olmalıdır.")]
        [Display(Name = "İlk Yatırım Maliyeti")]
        public decimal InitialInvestment { get; set; }

        [Required(ErrorMessage = "İskonto oranı zorunludur.")]
        [Range(0.01, 100, ErrorMessage = "İskonto oranı 0 ile 100 arasında olmalıdır.")]
        [Display(Name = "İskonto Oranı (%)")]
        public decimal DiscountRate { get; set; }

        [Required(ErrorMessage = "Tahmin edilecek dönem sayısı zorunludur.")]
        [Range(1, 50, ErrorMessage = "Tahmin edilecek dönem sayısı 1 ile 50 arasında olmalıdır.")]
        [Display(Name = "Tahmin Edilecek Dönem Sayısı")]
        public int ForecastPeriod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<CashFlow> CashFlows { get; set; } = new();
        public AnalysisResult? AnalysisResult { get; set; }
        public List<ForecastResult> ForecastResults { get; set; } = new();
    }
}