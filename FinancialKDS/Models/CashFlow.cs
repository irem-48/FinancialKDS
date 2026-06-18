using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class CashFlow
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        [Required(ErrorMessage = "Dönem bilgisi zorunludur.")]
        [Range(1, 1000, ErrorMessage = "Dönem değeri 1 veya daha büyük olmalıdır.")]
        [Display(Name = "Dönem")]
        public int Period { get; set; }

        [Required(ErrorMessage = "Nakit akışı zorunludur.")]
        [Range(1, 999999999, ErrorMessage = "Nakit akışı 0'dan büyük olmalıdır.")]
        [Display(Name = "Nakit Akışı")]
        public decimal Amount { get; set; }
    }
}