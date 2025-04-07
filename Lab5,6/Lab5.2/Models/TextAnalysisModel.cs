using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab5._2.Models
{
    public class TextAnalysisModel
    {
        [Required(ErrorMessage = "Please enter text for analysis")]
        public string InputText { get; set; } = string.Empty;
        public List<EntityResult>? Entities { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class EntityResult
    {
        public string Name { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string? MatchText { get; set; }
    }
}