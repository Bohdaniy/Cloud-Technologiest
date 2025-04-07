using System.Collections.Generic;

namespace Lab5._2.Models
{
    public class EducationModel
    {
        public List<EducationSample> SampleTexts { get; set; } = new List<EducationSample>();
        public int SelectedTextId { get; set; } = 1;
        public List<EducationEntityResult>? AnalysisResults { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class EducationSample
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class EducationEntityResult
    {
        public string Name { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Category { get; set; } = "Інше";
        public double Importance { get; set; }
    }
}