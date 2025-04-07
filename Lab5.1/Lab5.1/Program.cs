using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string endpoint = "https://labitoss5.cognitiveservices.azure.com/";
        string apiKey = "9hhl7bcPKgyfrDoIPP9agktDkDI70judyfkeqGgAha5tfrfaEnjZJQQJ99BCACYeBjFXJ3w3AAAaACOGy03I";

        var client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Entity Linking Analyzer");
        Console.WriteLine("Enter text for analysis (or 'exit' to quit):");

        while (true)
        {
            Console.Write("> ");
            string text = Console.ReadLine() ?? string.Empty;

            if (text.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Please enter some text.");
                continue;
            }

            AnalyzeText(client, text);
        }
    }

    static void AnalyzeText(TextAnalyticsClient client, string text)
    {
        try
        {
            Response<LinkedEntityCollection> response = client.RecognizeLinkedEntities(text);
            LinkedEntityCollection entities = response.Value;

            if (entities.Count == 0)
            {
                Console.WriteLine("\nNo linked entities found.");
                Console.WriteLine("Try using English text with proper names like:");
                Console.WriteLine("- 'Bill Gates founded Microsoft in the United States'\n");
            }
            else
            {
                PrintEntities(entities);
            }
        }
        catch (RequestFailedException e) when (e.ErrorCode == "UnsupportedLanguageCode")
        {
            Console.WriteLine("\nError: Only English text is supported.");
            Console.WriteLine("Please try again with English text.\n");
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine($"\nAzure Error: {e.Message}");
            Console.WriteLine($"Status Code: {e.Status}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nGeneral Error: {ex.Message}\n");
        }
    }

    static void PrintEntities(LinkedEntityCollection entities)
    {
        Console.WriteLine($"\nFound {entities.Count} entities:");
        Console.WriteLine("----------------------------------------");

        foreach (LinkedEntity entity in entities)
        {
            Console.WriteLine($"Entity: {entity.Name}");
            Console.WriteLine($"Source: {entity.DataSource ?? "Unknown"}");
            Console.WriteLine($"URL: {entity.Url?.AbsoluteUri ?? "No URL"}");
            Console.WriteLine($"ID: {entity.DataSourceEntityId ?? "No ID"}");

            if (entity.Matches.Any())
            {
                var firstMatch = entity.Matches.First();
                Console.WriteLine($"Confidence: {firstMatch.ConfidenceScore:P0}");
                Console.WriteLine($"Match: '{firstMatch.Text}' at position {firstMatch.Offset}");
            }

            Console.WriteLine("----------------------------------------");
        }
        Console.WriteLine();
    }
}