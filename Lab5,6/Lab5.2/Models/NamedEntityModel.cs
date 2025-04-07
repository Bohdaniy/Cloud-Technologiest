using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab5._2.Models
{
    public class NamedEntityModel
    {
        [Required(ErrorMessage = "Please enter text for analysis")]
        public string InputText { get; set; } = string.Empty;
        public List<NamedEntityResult>? Entities { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class NamedEntityResult
    {
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }
}