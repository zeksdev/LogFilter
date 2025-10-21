using System;
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        // 1. Command-Line Argument Parsing
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: LogFilter <keyword> <log-file-path>");
            Console.WriteLine("  <keyword>         The keyword to search for (case-insensitive)");
            Console.WriteLine("  <log-file-path>   Path to the log file to filter");
            return 1;
        }

        string keyword = args[0];
        string sourceFilePath = args[1];

        // Validate source file exists
        if (!File.Exists(sourceFilePath))
        {
            Console.WriteLine($"Error: File not found: {sourceFilePath}");
            return 1;
        }

        try
        {
            // 2. Core Filtering Logic
            var filteredLines = new List<string>();

            using (var reader = new StreamReader(sourceFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredLines.Add(line);
                    }
                }
            }

            // 3. Output File Generation
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string sanitizedKeyword = SanitizeFilename(keyword);
            string outputFilename = $"filtered_{sanitizedKeyword}_{timestamp}.log";

            string? sourceDirectory = Path.GetDirectoryName(sourceFilePath);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = Directory.GetCurrentDirectory();
            }

            string outputFilePath = Path.Combine(sourceDirectory, outputFilename);

            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var line in filteredLines)
                {
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine($"Success: Filtered {filteredLines.Count} line(s) containing '{keyword}'");
            Console.WriteLine($"Output file: {outputFilePath}");
            return 0;
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: File not found: {ex.Message}");
            return 1;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Error: Access denied. Check file permissions: {ex.Message}");
            return 1;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: I/O error occurred: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: An unexpected error occurred: {ex.Message}");
            return 1;
        }
    }

    static string SanitizeFilename(string filename)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string sanitized = filename;

        foreach (char c in invalidChars)
        {
            sanitized = sanitized.Replace(c, '_');
        }

        return sanitized;
    }
}