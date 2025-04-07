using Azure;
using Azure.AI.TextAnalytics;
using Lab5._2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace Lab5._2.Controllers
{
    public class HomeController : Controller
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(TextAnalyticsClient textAnalyticsClient, ILogger<HomeController> logger)
        {
            _textAnalyticsClient = textAnalyticsClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new TextAnalysisModel());
        }

        public IActionResult Education()
        {
            return View(new EducationModel
            {
                SampleTexts = GetSampleTexts()
            });
        }

        public IActionResult NamedEntities()
        {
            return View(new NamedEntityModel());
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(TextAnalysisModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            try
            {
                Response<LinkedEntityCollection> response = await _textAnalyticsClient.RecognizeLinkedEntitiesAsync(model.InputText);
                model.Entities = response.Value
                    .Select(e => {
                        var match = e.Matches.FirstOrDefault();
                        return new EntityResult
                        {
                            Name = e.Name ?? "Без назви",
                            Source = e.DataSource ?? "Невідоме джерело",
                            Url = e.Url?.AbsoluteUri ?? string.Empty,
                            EntityId = e.DataSourceEntityId ?? string.Empty,
                            Confidence = match.ConfidenceScore,
                            MatchText = match.Text
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Помилка: {ex.Message}";
                _logger.LogError(ex, "Text analysis error");
            }

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeEducation(EducationModel model)
        {
            model.SampleTexts = GetSampleTexts();

            var selectedSample = model.SampleTexts.FirstOrDefault(s => s.Id == model.SelectedTextId);
            if (selectedSample == null)
            {
                model.ErrorMessage = "Будь ласка, виберіть текст для аналізу";
                return View("Education", model);
            }

            try
            {
                Response<LinkedEntityCollection> response = await _textAnalyticsClient.RecognizeLinkedEntitiesAsync(selectedSample.Content);
                model.AnalysisResults = response.Value
                    .Select(e => {
                        var match = e.Matches.FirstOrDefault();
                        return new EducationEntityResult
                        {
                            Name = e.Name ?? "Без назви",
                            Source = e.DataSource ?? "Невідоме джерело",
                            Url = e.Url?.AbsoluteUri ?? string.Empty,
                            Category = GetCategory(e.DataSource),
                            Importance = match.ConfidenceScore
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Помилка аналізу: {ex.Message}";
                _logger.LogError(ex, "Education analysis error");
            }

            return View("Education", model);
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeNamedEntities(NamedEntityModel model)
        {
            if (!ModelState.IsValid) return View("NamedEntities", model);

            try
            {
                Response<CategorizedEntityCollection> response = await _textAnalyticsClient.RecognizeEntitiesAsync(model.InputText);
                model.Entities = response.Value
                    .Select(e => new NamedEntityResult
                    {
                        Text = e.Text,
                        Category = e.Category.ToString(),
                        SubCategory = e.SubCategory ?? "Немає",
                        Confidence = e.ConfidenceScore
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Помилка: {ex.Message}";
                _logger.LogError(ex, "NER analysis error");
            }

            return View("NamedEntities", model);
        }

        private List<EducationSample> GetSampleTexts()
        {
            return new List<EducationSample>
            {
                new EducationSample { Id = 1, Title = "Історичний текст", Content = "Kyiv was founded in the 5th century. In 882, Prince Oleg captured the city and made it the capital of Rus'." },
                new EducationSample { Id = 2, Title = "Науковий текст", Content = "Albert Einstein published the special theory of relativity in 1905. This theory changed the way we think about space and time." },
                new EducationSample { Id = 3, Title = "Географічний текст", Content = "The Dnieper is the longest river in Ukraine. It originates in Russia, flows through Belarus, and empties into the Black Sea." }
            };
        }

        private static string GetCategory(string? dataSource)
        {
            return dataSource switch
            {
                "Wikipedia" => "Загальні знання",
                "Bing" => "Пошукова інформація",
                _ => "Інше"
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}