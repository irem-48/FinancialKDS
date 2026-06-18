using FinancialKDS.Models;

namespace FinancialKDS.Services
{
    public class RegressionService
    {
        public (decimal Beta0, decimal Beta1, List<ForecastResult> Forecasts) CalculateForecasts(
            List<CashFlow> cashFlows,
            int forecastPeriod)
        {
            int n = cashFlows.Count;

            if (n < 2)
            {
                throw new InvalidOperationException("Regresyon analizi için en az 2 nakit akışı verisi girilmelidir.");
            }

            decimal sumX = 0;
            decimal sumY = 0;
            decimal sumXY = 0;
            decimal sumXX = 0;

            foreach (var cashFlow in cashFlows)
            {
                decimal x = cashFlow.Period;
                decimal y = cashFlow.Amount;

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumXX += x * x;
            }

            decimal denominator = (n * sumXX) - (sumX * sumX);

            if (denominator == 0)
            {
                throw new InvalidOperationException("Regresyon hesaplaması yapılamadı. Dönem değerleri farklı olmalıdır.");
            }

            decimal beta1 = ((n * sumXY) - (sumX * sumY)) / denominator;
            decimal beta0 = (sumY - (beta1 * sumX)) / n;

            int lastPeriod = cashFlows.Max(c => c.Period);

            var forecasts = new List<ForecastResult>();

            for (int i = 1; i <= forecastPeriod; i++)
            {
                int futurePeriod = lastPeriod + i;
                decimal predictedCashFlow = beta0 + (beta1 * futurePeriod);

                forecasts.Add(new ForecastResult
                {
                    Period = futurePeriod,
                    PredictedCashFlow = Math.Round(predictedCashFlow, 2)
                });
            }

            return (Math.Round(beta0, 4), Math.Round(beta1, 4), forecasts);
        }
    }
}