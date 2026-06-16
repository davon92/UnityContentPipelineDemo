using System.Collections.Generic;
using System.Text;

namespace DavonAllen.ContentPipelineDemo
{
    public static class CsvLineParser
    {
        public static IReadOnlyList<string> ParseLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder current = new StringBuilder();
            bool insideQuotes = false;

            for (int index = 0; index < line.Length; index++)
            {
                char character = line[index];

                if (character == '"')
                {
                    bool escapedQuote = insideQuotes && index + 1 < line.Length && line[index + 1] == '"';

                    if (escapedQuote)
                    {
                        current.Append('"');
                        index++;
                        continue;
                    }

                    insideQuotes = !insideQuotes;
                    continue;
                }

                if (character == ',' && !insideQuotes)
                {
                    fields.Add(current.ToString().Trim());
                    current.Length = 0;
                    continue;
                }

                current.Append(character);
            }

            fields.Add(current.ToString().Trim());
            return fields;
        }
    }
}
