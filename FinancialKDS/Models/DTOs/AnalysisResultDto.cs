using System.Collections.Generic;

namespace FinancialKDS.Models.DTOs
{
    public enum RiskLevel
    {
        Low,
        Medium,
        High
    }

    public class CashFlowDetailDto
    {
        public int Period { get; set; }
        public decimal HistoricalAmount { get; set; }
        public decimal EstimatedAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public decimal CumulativeDiscountedCashFlow { get; set; }
    }

    public class RecommendationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionableAdvice { get; set; } = string.Empty;
        public RiskLevel Risk { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class AnalysisResultDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal InitialInvestment { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal OriginalNPV { get; set; }

        // Yeni Finansal ve İstatistiksel Metrikler
        public decimal IRR { get; set; } // İç Verimlilik Oranı (%)
        public double DiscountedPaybackPeriod { get; set; } // İskonto Edilmiş Geri Ödeme Süresi (Dönem)
        public bool IsPaybackAchieved { get; set; } // Ödeme süresi proje ömrü içinde tamamlandı mı?
        public decimal MaxBearableDiscountRate { get; set; } // Maksimum Dayanılabilir İskonto Oranı (NPV=0 noktası)
        public decimal DiscountRateSafetyMargin { get; set; } // İskonto Oranı Emniyet Marjı (%)
        public double RSquared { get; set; } // Regresyon Belirtme Katsayısı (%)
        public double RMSE { get; set; } // Kök Ortalama Kare Hata (TL)

        public double Beta0 { get; set; } // Regresyon Sabiti
        public double Beta1 { get; set; } // Regresyon Eğim Katsayısı
        public string StatusDecision { get; set; } = string.Empty;

        public List<CashFlowDetailDto> ProjectedCashFlows { get; set; } = new List<CashFlowDetailDto>();
        public List<RecommendationDto> Recommendations { get; set; } = new List<RecommendationDto>();
    }
}