using System.CommandLine;
using System.Diagnostics;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Define the root command
        var rootCommand = new RootCommand("LogFilter - A command-line utility for filtering log files by keyword")
        {
            TreatUnmatchedTokensAsErrors = true
        };

        // Define positional arguments
        var keywordArgument = new Argument<string?>(
            name: "keyword",
            description: "The keyword to search for (case-insensitive by default)",
            getDefaultValue: () => null
        );

        var filePathArgument = new Argument<string?>(
            name: "file-path",
            description: "Path to the log file to filter",
            getDefaultValue: () => null
        );

        // Define options
        var outputOption = new Option<string?>(
            aliases: new[] { "--output", "-o" },
            description: "Custom output file path (default: auto-generated timestamped file in source directory)"
        );

        var caseSensitiveOption = new Option<bool>(
            aliases: new[] { "--case-sensitive", "-c" },
            description: "Enable case-sensitive keyword matching (default: case-insensitive)",
            getDefaultValue: () => false
        );

        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Display detailed processing information (line counts, processing time)",
            getDefaultValue: () => false
        );

        var interactiveOption = new Option<bool>(
            aliases: new[] { "--interactive", "-i" },
            description: "Interactive mode - prompts for all inputs",
            getDefaultValue: () => false
        );

        // Add arguments and options to root command
        rootCommand.AddArgument(keywordArgument);
        rootCommand.AddArgument(filePathArgument);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(caseSensitiveOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(interactiveOption);

        // Set the handler
        rootCommand.SetHandler(
            async (string? keyword, string? filePath, string? output, bool caseSensitive, bool verbose, bool interactive) =>
            {
                try
                {
                    // Handle interactive mode
                    if (interactive || (keyword == null && filePath == null))
                    {
                        var result = await RunInteractiveMode();
                        if (result != 0)
                        {
                            Environment.ExitCode = result;
                            return;
                        }
                        return;
                    }

                    // Validate required arguments
                    if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(filePath))
                    {
                        Console.WriteLine("Error: Both keyword and file-path are required.");
                        Console.WriteLine("Use --help for usage information or --interactive for interactive mode.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    // Execute filtering
                    var exitCode = await ExecuteFilter(keyword, filePath, output, caseSensitive, verbose);
                    Environment.ExitCode = exitCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: An unexpected error occurred: {ex.Message}");
                    Environment.ExitCode = 1;
                }
            },
            keywordArgument,
            filePathArgument,
            outputOption,
            caseSensitiveOption,
            verboseOption,
            interactiveOption
        );

        return await rootCommand.InvokeAsync(args);
    }

    static async Task<int> RunInteractiveMode()
    {
        Console.WriteLine("=== LogFilter Interactive Mode ===");
        Console.WriteLine();

        // Prompt for keyword
        Console.Write("Enter keyword to search for: ");
        var keyword = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Error: Keyword cannot be empty.");
            return 1;
        }

        // Prompt for file path with validation
        string? filePath = null;
        while (true)
        {
            Console.Write("Enter log file path: ");
            filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Error: File path cannot be empty. Please try again.");
                continue;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found: {filePath}");
                Console.Write("Try again? (Y/n): ");
                var retry = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (retry == "n" || retry == "no")
                {
                    return 1;
                }
                continue;
            }

            break;
        }

        // Prompt for case sensitivity
        Console.Write("Case-sensitive search? (y/N): ");
        var caseSensitiveInput = Console.ReadLine()?.Trim().ToLowerInvariant();
        var caseSensitive = caseSensitiveInput == "y" || caseSensitiveInput == "yes";

        // Prompt for custom output path
        Console.Write("Custom output path? (leave empty for auto-generated): ");
        var output = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(output))
        {
            output = null;
        }

        Console.WriteLine();
        Console.WriteLine("Processing...");
        Console.WriteLine();

        // Execute filtering with verbose mode enabled in interactive
        return await ExecuteFilter(keyword, filePath, output, caseSensitive, verbose: true);
    }

    static async Task<int> ExecuteFilter(string keyword, string filePath, string? customOutputPath, bool caseSensitive, bool verbose)
    {
        // Validate source file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found: {filePath}");
            return 1;
        }

        // Validate custom output path directory if provided
        if (!string.IsNullOrWhiteSpace(customOutputPath))
        {
            var outputDir = Path.GetDirectoryName(customOutputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Console.WriteLine($"Error: Output directory does not exist: {outputDir}");
                Console.WriteLine("Please create the directory first or use --output with a valid path.");
                return 1;
            }
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var filteredLines = new List<string>();
            var totalLines = 0;

            if (verbose)
            {
                Console.WriteLine($"Reading file: {filePath}");
                Console.WriteLine($"Searching for: '{keyword}'");
                Console.WriteLine($"Case-sensitive: {caseSensitive}");
                Console.WriteLine();
            }

            // Core filtering logic
            using (var reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    totalLines++;
                    var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                    if (line.Contains(keyword, comparison))
                    {
                        filteredLines.Add(line);
                    }
                }
            }

            stopwatch.Stop();

            if (verbose)
            {
                Console.WriteLine($"Total lines read: {totalLines}");
                Console.WriteLine($"Lines matched: {filteredLines.Count}");
                Console.WriteLine($"Processing time: {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine();
            }

            // Determine output file path
            string outputFilePath;
            if (!string.IsNullOrWhiteSpace(customOutputPath))
            {
                // Handle custom output path
                if (Path.IsPathRooted(customOutputPath))
                {
                    // Absolute path
                    outputFilePath = customOutputPath;
                }
                else if (customOutputPath.Contains(Path.DirectorySeparatorChar) || customOutputPath.Contains(Path.AltDirectorySeparatorChar))
                {
                    // Relative path with directory
                    outputFilePath = Path.GetFullPath(customOutputPath);
                }
                else
                {
                    // Filename only - place in source file's directory
                    var sourceDirectory = Path.GetDirectoryName(filePath);
                    if (string.IsNullOrEmpty(sourceDirectory))
                    {
                        sourceDirectory = Directory.GetCurrentDirectory();
                    }
                    outputFilePath = Path.Combine(sourceDirectory, customOutputPath);
                }
            }
            else
            {
                // Auto-generated timestamped filename
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string sanitizedKeyword = SanitizeFilename(keyword);
                string outputFilename = $"filtered_{sanitizedKeyword}_{timestamp}.log";

                string? sourceDirectory = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(sourceDirectory))
                {
                    sourceDirectory = Directory.GetCurrentDirectory();
                }

                outputFilePath = Path.Combine(sourceDirectory, outputFilename);
            }

            // Write filtered lines to output file
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var line in filteredLines)
                {
                    await writer.WriteLineAsync(line);
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
