using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proje adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; } // Hataları çözmek için eklenen açıklama alanı

        [Required(ErrorMessage = "İlk yatırım maliyeti zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Maliyet 0'dan büyük olmalıdır.")]
        public decimal InitialInvestment { get; set; }

        [Required(ErrorMessage = "İskonto oranı zorunludur.")]
        [Range(0, 100, ErrorMessage = "İskonto oranı 0 ile 100 arasında olmalıdır.")]
        public decimal DiscountRate { get; set; }

        [Required(ErrorMessage = "Tahmin dönemi sayısı zorunludur.")]
        [Range(1, 20, ErrorMessage = "Tahmin dönemi 1 ile 20 arasında olmalıdır.")]
        public int ForecastPeriods { get; set; }

        public int PeriodsToForecast
        {
            get => ForecastPeriods;
            set => ForecastPeriods = value;
        } // Hataları çözmek için ForecastPeriods'a bağlanan alternatif özellik

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Hataları çözmek için eklenen tarih alanı

        // Kullanıcı İlişkisi (Identity User ID)
        public string UserId { get; set; } = string.Empty;

        // İlişkili Nakit Akışları
        public ICollection<CashFlow> CashFlows { get; set; } = new List<CashFlow>();
    }
}