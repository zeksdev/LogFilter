using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// Utility class for terminal hyperlink support (OSC 8 escape sequences)
/// </summary>
static class TerminalHyperlink
{
    private static readonly bool _supportsHyperlinks;

    static TerminalHyperlink()
    {
        _supportsHyperlinks = DetectHyperlinkSupport();
    }

    /// <summary>
    /// Detects if the current terminal supports hyperlinks (OSC 8)
    /// </summary>
    private static bool DetectHyperlinkSupport()
    {
        // Check for known terminal emulators that support hyperlinks
        var termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM");
        var wtSession = Environment.GetEnvironmentVariable("WT_SESSION");
        var vteVersion = Environment.GetEnvironmentVariable("VTE_VERSION");
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");

        // Windows Terminal
        if (!string.IsNullOrEmpty(wtSession))
            return true;

        // iTerm2 (macOS)
        if (termProgram == "iTerm.app")
            return true;

        // VS Code terminal
        if (termProgram == "vscode")
            return true;

        // GNOME Terminal 3.x+ (VTE-based terminals)
        if (!string.IsNullOrEmpty(vteVersion))
        {
            if (int.TryParse(vteVersion, out int version) && version >= 5000)
                return true;
        }

        // Konsole and other modern terminals
        if (colorTerm == "truecolor")
        {
            var term = Environment.GetEnvironmentVariable("TERM");
            if (term?.Contains("konsole") == true || term?.Contains("xterm") == true)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Formats a file path as a clickable hyperlink if the terminal supports it
    /// </summary>
    public static string FormatPath(string filePath)
    {
        if (!_supportsHyperlinks)
            return filePath;

        var fileUri = PathToFileUri(filePath);

        // OSC 8 format: \e]8;;URI\e\\TEXT\e]8;;\e\\
        return $"\x1b]8;;{fileUri}\x1b\\{filePath}\x1b]8;;\x1b\\";
    }

    /// <summary>
    /// Converts a file path to a proper file:// URI
    /// </summary>
    private static string PathToFileUri(string filePath)
    {
        // Get absolute path
        var absolutePath = Path.GetFullPath(filePath);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows: file:///C:/path/to/file.log
            // Replace backslashes with forward slashes and encode spaces
            var uriPath = absolutePath.Replace('\\', '/');
            return $"file:///{Uri.EscapeDataString(uriPath).Replace("%2F", "/").Replace("%3A", ":")}";
        }
        else
        {
            // Unix/Linux/macOS: file:///path/to/file.log
            return $"file://{Uri.EscapeDataString(absolutePath).Replace("%2F", "/")}";
        }
    }
}

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
        var keywordArgument = new Argument<string[]>(
            name: "keywords",
            description: "One or more keywords to search for (space-separated, case-insensitive by default)"
        )
        {
            Arity = ArgumentArity.ZeroOrMore
        };

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
            async (string[] keywords, string? filePath, string? output, bool caseSensitive, bool verbose, bool interactive) =>
            {
                try
                {
                    // Handle interactive mode
                    if (interactive || (keywords.Length == 0 && filePath == null))
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
                    if (keywords.Length == 0 || string.IsNullOrWhiteSpace(filePath))
                    {
                        Console.WriteLine("Error: Both keywords and file-path are required.");
                        Console.WriteLine("Use --help for usage information or --interactive for interactive mode.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    // Execute filtering
                    var exitCode = await ProcessKeywords(keywords, filePath, output, caseSensitive, verbose);
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

        // Prompt for keywords
        Console.Write("Enter keyword(s) to search for (comma-separated for multiple): ");
        var keywordInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(keywordInput))
        {
            Console.WriteLine("Error: Keyword cannot be empty.");
            return 1;
        }

        // Parse keywords (split by comma and trim)
        var keywords = keywordInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToArray();

        if (keywords.Length == 0)
        {
            Console.WriteLine("Error: No valid keywords provided.");
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
        return await ProcessKeywords(keywords, filePath, output, caseSensitive, verbose: true);
    }

    static async Task<int> ProcessKeywords(string[] keywords, string filePath, string? customOutputPath, bool caseSensitive, bool verbose)
    {
        // Validate source file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found: {filePath}");
            return 1;
        }

        // Generate shared timestamp for all output files
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        if (verbose && keywords.Length > 1)
        {
            Console.WriteLine($"Processing {keywords.Length} keywords in parallel:");
            foreach (var kw in keywords)
            {
                Console.WriteLine($"  - {kw}");
            }
            Console.WriteLine();
        }

        // Track results for each keyword
        var results = new List<(string Keyword, bool Success, string? OutputPath, string? ErrorMessage, int MatchCount)>();

        // Process keywords in parallel
        await Parallel.ForEachAsync(keywords, async (keyword, cancellationToken) =>
        {
            try
            {
                var (success, outputPath, errorMessage, matchCount) = await ExecuteFilterForKeyword(
                    keyword, filePath, customOutputPath, caseSensitive, verbose && keywords.Length == 1, timestamp);

                lock (results)
                {
                    results.Add((keyword, success, outputPath, errorMessage, matchCount));
                }
            }
            catch (Exception ex)
            {
                lock (results)
                {
                    results.Add((keyword, false, null, ex.Message, 0));
                }
            }
        });

        // Display results
        if (keywords.Length > 1)
        {
            Console.WriteLine("=== Processing Summary ===");
            var successCount = 0;
            foreach (var result in results.OrderBy(r => r.Keyword))
            {
                if (result.Success)
                {
                    successCount++;
                    var formattedPath = TerminalHyperlink.FormatPath(result.OutputPath!);
                    Console.WriteLine($"✓ '{result.Keyword}': {result.MatchCount} line(s) → {formattedPath}");
                }
                else
                {
                    Console.WriteLine($"✗ '{result.Keyword}': Failed - {result.ErrorMessage}");
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Completed: {successCount}/{keywords.Length} keywords processed successfully");

            return successCount > 0 ? 0 : 1;
        }
        else
        {
            // Single keyword - return result directly
            var result = results.First();
            return result.Success ? 0 : 1;
        }
    }

    static async Task<(bool Success, string? OutputPath, string? ErrorMessage, int MatchCount)> ExecuteFilterForKeyword(
        string keyword, string filePath, string? customOutputPath, bool caseSensitive, bool verbose, string timestamp)
    {

        // Validate custom output path directory if provided
        if (!string.IsNullOrWhiteSpace(customOutputPath))
        {
            var outputDir = Path.GetDirectoryName(customOutputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                var errorMsg = $"Output directory does not exist: {outputDir}";
                if (verbose)
                {
                    Console.WriteLine($"Error: {errorMsg}");
                }
                return (false, null, errorMsg, 0);
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
                // Auto-generated timestamped filename using the provided timestamp
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

            if (verbose)
            {
                Console.WriteLine($"Success: Filtered {filteredLines.Count} line(s) containing '{keyword}'");
                var formattedPath = TerminalHyperlink.FormatPath(outputFilePath);
                Console.WriteLine($"Output file: {formattedPath}");
            }

            return (true, outputFilePath, null, filteredLines.Count);
        }
        catch (FileNotFoundException ex)
        {
            var errorMsg = $"File not found: {ex.Message}";
            if (verbose)
            {
                Console.WriteLine($"Error: {errorMsg}");
            }
            return (false, null, errorMsg, 0);
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorMsg = $"Access denied. Check file permissions: {ex.Message}";
            if (verbose)
            {
                Console.WriteLine($"Error: {errorMsg}");
            }
            return (false, null, errorMsg, 0);
        }
        catch (IOException ex)
        {
            var errorMsg = $"I/O error occurred: {ex.Message}";
            if (verbose)
            {
                Console.WriteLine($"Error: {errorMsg}");
            }
            return (false, null, errorMsg, 0);
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
