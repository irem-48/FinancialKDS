using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class CashFlow
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int Period { get; set; }

        [Required]
        [Display(Name = "Nakit Akışı (TL)")]
        // Range kısıtı negatif nakit akışlarına (zarar dönemlerine) izin verecek şekilde kaldırıldı
        public decimal Amount { get; set; }

        public Project? Project { get; set; }
    }
}