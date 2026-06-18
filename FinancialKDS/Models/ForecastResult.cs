using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class ForecastResult
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        [Display(Name = "Dönem")]
        public int Period { get; set; }

        [Display(Name = "Tahmini Nakit Akışı")]
        public decimal PredictedCashFlow { get; set; }

        [Display(Name = "Bugünkü Değer")]
        public decimal PresentValue { get; set; }
    }
}