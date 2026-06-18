using System.ComponentModel.DataAnnotations;

namespace FinancialKDS.Models
{
    public class AnalysisResult
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        [Display(Name = "Beta 0")]
        public decimal Beta0 { get; set; }

        [Display(Name = "Beta 1")]
        public decimal Beta1 { get; set; }

        [Display(Name = "NPV")]
        public decimal NPV { get; set; }

        [Display(Name = "Karar")]
        public string Decision { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 