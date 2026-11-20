namespace FinancialKDS.Models.DTOs
{
    public class ScenarioParametersDto
    {
        public int ProjectId { get; set; }
        public decimal CashFlowGrowthPercent { get; set; }
        public decimal DiscountRateAdjustment { get; set; }
        public decimal InitialInvestmentAdjustmentPercent { get; set; }
    }
}