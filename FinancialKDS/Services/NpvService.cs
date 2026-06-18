using FinancialKDS.Models;

namespace FinancialKDS.Services
{
    public class NpvService
    {
        public decimal CalculateNpv(
            decimal initialInvestment,
            decimal discountRatePercent,
            List<ForecastResult> forecasts)
        {
            decimal discountRate = discountRatePercent / 100;
            decimal totalPresentValue = 0;

            for (int i = 0; i < forecasts.Count; i++)
            {
                int t = i + 1;
                decimal predictedCashFlow = forecasts[i].PredictedCashFlow;

                decimal presentValue = predictedCashFlow / (decimal)Math.Pow((double)(1 + discountRate), t);

                forecasts[i].PresentValue = Math.Round(presentValue, 2);
                totalPresentValue += presentValue;
            }

            decimal npv = totalPresentValue - initialInvestment;

            return Math.Round(npv, 2);
        }

        public string GetDecision(decimal npv)
        {
            if (npv > 0)
                return "Yatırım finansal açıdan kabul edilebilir. NPV değeri pozitiftir.";

            if (npv < 0)
                return "Yatırım finansal açıdan uygun görünmemektedir. NPV değeri negatiftir.";

            return "Yatırım başa baş noktasındadır. NPV değeri sıfıra eşittir.";
        }
    }
}