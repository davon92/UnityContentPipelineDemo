using System;
using System.IO;
using System.Linq;
using DavonAllen.ContentPipelineDemo;

namespace ValidationHarness
{
    internal static class Program
    {
        private static int Main()
        {
            string root = FindRepositoryRoot();
            string validCsvPath = Path.Combine(root, "Samples~", "ItemCatalog", "items.csv");
            string invalidCsvPath = Path.Combine(root, "Samples~", "ItemCatalog", "items_with_errors.csv");

            ItemCatalogPipeline pipeline = new ItemCatalogPipeline();
            ItemCatalogPipelineOptions options = new ItemCatalogPipelineOptions
            {
                GeneratedAtUtc = new DateTime(2026, 6, 16, 0, 0, 0, DateTimeKind.Utc)
            };

            ItemCatalogPipelineResult validResult = pipeline.Run(File.ReadAllText(validCsvPath), options);
            ItemCatalogPipelineResult invalidResult = pipeline.Run(File.ReadAllText(invalidCsvPath), options);

            bool validSamplePassed = !validResult.HasErrors && validResult.Items.Count == 4;
            bool invalidSamplePassed = invalidResult.HasErrors && invalidResult.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error) >= 4;
            bool jsonPassed = validResult.GeneratedJson.Contains("\"id\": \"storm_staff\"") &&
                              validResult.GeneratedJson.Contains("\"itemCount\": 4") &&
                              validResult.GeneratedJson.Contains("\"maxStackSize\": 99") &&
                              !validResult.GeneratedJson.Contains("old_training_blade");

            PrintResult("Valid sample", validSamplePassed);
            PrintResult("Invalid sample", invalidSamplePassed);
            PrintResult("Generated JSON contains expected content", jsonPassed);

            if (!validSamplePassed)
            {
                PrintDiagnostics("Valid sample diagnostics", validResult);
            }

            if (!invalidSamplePassed)
            {
                PrintDiagnostics("Invalid sample diagnostics", invalidResult);
            }

            return validSamplePassed && invalidSamplePassed && jsonPassed ? 0 : 1;
        }

        private static string FindRepositoryRoot()
        {
            string current = AppContext.BaseDirectory;

            while (!string.IsNullOrEmpty(current))
            {
                if (File.Exists(Path.Combine(current, "package.json")))
                {
                    return current;
                }

                DirectoryInfo parent = Directory.GetParent(current);
                current = parent != null ? parent.FullName : string.Empty;
            }

            throw new InvalidOperationException("Could not locate repository root.");
        }

        private static void PrintResult(string name, bool passed)
        {
            Console.WriteLine(name + ": " + (passed ? "PASS" : "FAIL"));
        }

        private static void PrintDiagnostics(string title, ItemCatalogPipelineResult result)
        {
            Console.WriteLine(title);

            foreach (PipelineDiagnostic diagnostic in result.Diagnostics)
            {
                Console.WriteLine("  " + diagnostic);
            }
        }
    }
}
