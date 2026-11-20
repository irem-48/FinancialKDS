using FinancialKDS.Models.DTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinancialKDS.Services
{
    public class FinancialAnalysisService
    {
        public AnalysisResultDto AnalyzeProject(
            int projectId,
            string projectName,
            decimal initialInvestment,
            decimal discountRatePercent,
            List<decimal> historicalCashFlows,
            int forecastPeriods)
        {
            var result = new AnalysisResultDto
            {
                ProjectId = projectId,
                ProjectName = projectName,
                InitialInvestment = initialInvestment,
                DiscountRate = discountRatePercent
            };

            if (historicalCashFlows == null || !historicalCashFlows.Any())
            {
                result.StatusDecision = "Geçmiş Veri Eksik";
                return result;
            }

            // 1. Doğrusal Regresyon Tahmini (Least Squares)
            int n = historicalCashFlows.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i + 1;
                double y = (double)historicalCashFlows[i];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double beta1 = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double beta0 = (sumY - beta1 * sumX) / n;

            if (double.IsNaN(beta1) || double.IsInfinity(beta1)) beta1 = 0;
            if (double.IsNaN(beta0) || double.IsInfinity(beta0)) beta0 = 0;

            result.Beta0 = Math.Round(beta0, 2);
            result.Beta1 = Math.Round(beta1, 2);

            // 2. Regresyon İstatistiksel Başarı Metrikleri (R² ve RMSE)
            double meanY = sumY / n;
            double ssTot = 0, ssRes = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i + 1;
                double actualY = (double)historicalCashFlows[i];
                double predictedY = beta0 + beta1 * x;

                ssTot += Math.Pow(actualY - meanY, 2);
                ssRes += Math.Pow(actualY - predictedY, 2);
            }

            double rSquared = ssTot > 0 ? (1.0 - (ssRes / ssTot)) * 100.0 : 0.0;
            double rmse = n > 0 ? Math.Sqrt(ssRes / n) : 0.0;

            result.RSquared = Math.Max(0, Math.Min(100, Math.Round(rSquared, 2)));
            result.RMSE = Math.Round(rmse, 2);

            // 3. Gelecek Dönem Nakit Akışları ve İskonto Edilmiş Değerler
            decimal discountRateDecimal = discountRatePercent / 100m;
            decimal cumulativeDiscounted = 0m;
            double paybackPeriod = -1;
            bool paybackFound = false;

            for (int t = 1; t <= forecastPeriods; t++)
            {
                decimal estimatedAmount = Math.Max(0m, (decimal)(beta0 + beta1 * (n + t)));
                decimal discountFactor = (decimal)Math.Pow((double)(1m + discountRateDecimal), t);
                decimal discountedAmount = discountFactor > 0 ? estimatedAmount / discountFactor : 0m;

                cumulativeDiscounted += discountedAmount;

                // İskonto Edilmiş Geri Ödeme Süresi (DPP) Hesaplaması
                if (!paybackFound && cumulativeDiscounted >= initialInvestment)
                {
                    decimal prevCumulative = cumulativeDiscounted - discountedAmount;
                    decimal needed = initialInvestment - prevCumulative;
                    paybackPeriod = (t - 1) + (discountedAmount > 0 ? (double)(needed / discountedAmount) : 0);
                    paybackFound = true;
                }

                result.ProjectedCashFlows.Add(new CashFlowDetailDto
                {
                    Period = t,
                    EstimatedAmount = Math.Round(estimatedAmount, 2),
                    DiscountedAmount = Math.Round(discountedAmount, 2),
                    CumulativeDiscountedCashFlow = Math.Round(cumulativeDiscounted, 2)
                });
            }

            decimal originalNpv = cumulativeDiscounted - initialInvestment;
            result.OriginalNPV = Math.Round(originalNpv, 2);
            result.StatusDecision = originalNpv >= 0 ? "Kabul Edilebilir (Pozitif NPV)" : "Kabul Edilemez (Negatif NPV)";

            result.IsPaybackAchieved = paybackFound;
            result.DiscountedPaybackPeriod = paybackFound ? Math.Round(paybackPeriod, 2) : 0;

            // 4. İç Verimlilik Oranı (IRR) Hesaplaması
            result.IRR = CalculateIRR(initialInvestment, result.ProjectedCashFlows.Select(x => x.EstimatedAmount).ToList());

            // 5. Hassasiyet Analizi
            result.MaxBearableDiscountRate = result.IRR;
            result.DiscountRateSafetyMargin = Math.Round(result.IRR - discountRatePercent, 2);

            // 6. Akıllı Öneriler
            GenerateSmartRecommendations(result);

            return result;
        }

        private decimal CalculateIRR(decimal initialInvestment, List<decimal> cashFlows)
        {
            if (cashFlows == null || !cashFlows.Any()) return 0m;

            double rate = 0.10;
            double tolerance = 0.00001;
            int maxIterations = 1000;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                double npv = -(double)initialInvestment;
                double derivativeNpv = 0;

                for (int t = 0; t < cashFlows.Count; t++)
                {
                    double cf = (double)cashFlows[t];
                    double factor = Math.Pow(1.0 + rate, t + 1);
                    npv += cf / factor;
                    derivativeNpv -= (t + 1) * cf / Math.Pow(1.0 + rate, t + 2);
                }

                if (Math.Abs(npv) < tolerance) break;
                if (Math.Abs(derivativeNpv) < 1e-10) break;

                double nextRate = rate - npv / derivativeNpv;
                if (Math.Abs(nextRate - rate) < tolerance)
                {
                    rate = nextRate;
                    break;
                }
                rate = nextRate;
            }

            if (double.IsNaN(rate) || double.IsInfinity(rate) || rate < -0.99) return 0m;

            return Math.Round((decimal)(rate * 100.0), 2);
        }

        private void GenerateSmartRecommendations(AnalysisResultDto result)
        {
            if (result.OriginalNPV < 0)
            {
                result.Recommendations.Add(new RecommendationDto
                {
                    Title = "Finansal Risk Uyarı",
                    Description = $"Proje mevcut iskonto oranı (%{result.DiscountRate}) ile {Math.Abs(result.OriginalNPV):N2} TL zarar üretmektedir.",
                    ActionableAdvice = "İlk yatırım maliyetini düşürmeyi veya sermaye maliyetini yeniden müzakere etmeyi deneyin.",
                    Risk = RiskLevel.High,
                    Category = "Karlılık"
                });
            }
            else
            {
                result.Recommendations.Add(new RecommendationDto
                {
                    Title = "Pozitif Yatırım Getirisi",
                    Description = $"Proje {result.OriginalNPV:N2} TL net bugünkü değer üretmektedir.",
                    ActionableAdvice = "Mevcut büyüme trendini koruyacak operasyonel yatırımları sürdürün.",
                    Risk = RiskLevel.Low,
                    Category = "Karlılık"
                });
            }

            if (result.DiscountRateSafetyMargin < 2 && result.OriginalNPV >= 0)
            {
                result.Recommendations.Add(new RecommendationDto
                {
                    Title = "Düşük İskonto Emniyet Marjı",
                    Description = $"İskonto oranı emniyet marjınız sadece %{result.DiscountRateSafetyMargin}. Sermaye maliyetindeki küçük artışlar projeyi zarara geçirebilir.",
                    ActionableAdvice = "Borç/özkaynak yapısını optimize ederek ağırlıklı ortalama sermaye maliyetini (WACC) düşürün.",
                    Risk = RiskLevel.Medium,
                    Category = "Hassasiyet"
                });
            }

            if (result.RSquared < 60)
            {
                result.Recommendations.Add(new RecommendationDto
                {
                    Title = "Düşük Regresyon Uyum Oranı",
                    Description = $"Gelecek tahmin modeli uyum başarısı %{result.RSquared} seviyesindedir.",
                    ActionableAdvice = "Tahmin kalitesini artırmak için daha fazla döneme ait geçmiş nakit akış verisi ekleyin.",
                    Risk = RiskLevel.Medium,
                    Category = "Veri Analitiği"
                });
            }
        }

        public AnalysisResultDto SimulateScenario(AnalysisResultDto baseAnalysis, ScenarioParametersDto scenario)
        {
            decimal simulatedGrowthDecimal = 1m + (scenario.CashFlowGrowthPercent / 100m);
            decimal simulatedDiscountRate = Math.Max(0.1m, baseAnalysis.DiscountRate + scenario.DiscountRateAdjustment);
            decimal simulatedInitialInvestment = baseAnalysis.InitialInvestment * (1m - (scenario.InitialInvestmentAdjustmentPercent / 100m));

            List<decimal> simulatedHistoricalCashFlows = baseAnalysis.ProjectedCashFlows
                .Select(x => x.EstimatedAmount * simulatedGrowthDecimal)
                .ToList();

            return AnalyzeProject(
                baseAnalysis.ProjectId,
                baseAnalysis.ProjectName,
                simulatedInitialInvestment,
                simulatedDiscountRate,
                simulatedHistoricalCashFlows,
                baseAnalysis.ProjectedCashFlows.Count
            );
        }

        // --- RAPORLAMA METODLARI ---

        public byte[] GeneratePdfReport(AnalysisResultDto analysis)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DarkGray);
                Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Black);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.White);

                Paragraph title = new Paragraph($"{analysis.ProjectName}\nFinansal Karar Destek Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 100;
                summaryTable.AddCell(new PdfPCell(new Phrase("Ilk Yatirim Maliyeti", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase($"{analysis.InitialInvestment:N2} TL", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("Net Bugünkü Deger (NPV)", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase($"{analysis.OriginalNPV:N2} TL", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("Ic Verimlilik Orani (IRR)", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase($"%{analysis.IRR:N2}", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("Iskonto Edilmis Geri Ödeme", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(analysis.IsPaybackAchieved ? $"{analysis.DiscountedPaybackPeriod:N2} Dönem" : "Geri Ödenemedi", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("Regresyon Uyum Başarisi (R2)", bodyFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase($"%{analysis.RSquared:N2}", bodyFont)));
                document.Add(summaryTable);

                document.Add(new Paragraph("\nTahmini Nakit Akislari Tablosu:\n\n", titleFont));

                PdfPTable cfTable = new PdfPTable(4);
                cfTable.WidthPercentage = 100;

                string[] headers = { "Donem", "Tahmini Akis", "Iskonto Edilmis", "Kumulatif Akis" };
                foreach (string h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = BaseColor.Gray;
                    cfTable.AddCell(cell);
                }

                foreach (var cf in analysis.ProjectedCashFlows)
                {
                    cfTable.AddCell(new PdfPCell(new Phrase(cf.Period.ToString(), bodyFont)));
                    cfTable.AddCell(new PdfPCell(new Phrase($"{cf.EstimatedAmount:N2} TL", bodyFont)));
                    cfTable.AddCell(new PdfPCell(new Phrase($"{cf.DiscountedAmount:N2} TL", bodyFont)));
                    cfTable.AddCell(new PdfPCell(new Phrase($"{cf.CumulativeDiscountedCashFlow:N2} TL", bodyFont)));
                }

                document.Add(cfTable);
                document.Close();

                return ms.ToArray();
            }
        }

        public byte[] GenerateExcelReport(AnalysisResultDto analysis)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(ms, System.Text.Encoding.UTF8))
            {
                writer.WriteLine($"Proje Adı;{analysis.ProjectName}");
                writer.WriteLine($"İlk Yatırım Maliyeti;{analysis.InitialInvestment}");
                writer.WriteLine($"Net Bugünkü Değer (NPV);{analysis.OriginalNPV}");
                writer.WriteLine($"İç Verimlilik Oranı (IRR);%{analysis.IRR}");
                writer.WriteLine($"İskonto Edilmiş Geri Ödeme;{(analysis.IsPaybackAchieved ? $"{analysis.DiscountedPaybackPeriod} Dönem" : "Geri Ödenemedi")}");
                writer.WriteLine($"Regresyon Uyum Başarısı (R²);%{analysis.RSquared}");
                writer.WriteLine();
                writer.WriteLine("Dönem;Tahmini Nakit Akışı (TL);İskonto Edilmiş Akış (TL);Kümilatif Akış (TL)");

                foreach (var cf in analysis.ProjectedCashFlows)
                {
                    writer.WriteLine($"{cf.Period};{cf.EstimatedAmount};{cf.DiscountedAmount};{cf.CumulativeDiscountedCashFlow}");
                }

                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}