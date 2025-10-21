# Add CLI Framework

## Why
The current console application uses basic argument parsing (positional args only) which limits extensibility and user experience. Adopting System.CommandLine will provide:
- Modern, type-safe command-line parsing with rich help generation
- Support for options/flags (--output, --case-sensitive, --verbose) for better control
- Interactive mode for less technical users who prefer prompts over command-line arguments
- Better error messages and validation
- Foundation for future features like multiple keywords, regex patterns, and output formats

## What Changes
- Add System.CommandLine NuGet package dependency
- Replace manual argument parsing with System.CommandLine's RootCommand and Option system
- Add optional flags:
  - `--output <path>` or `-o <path>`: Custom output file path (overrides auto-generated timestamped filename)
  - `--case-sensitive` or `-c`: Disable case-insensitive matching (default is case-insensitive)
  - `--verbose` or `-v`: Display additional processing information (line count, processing time)
- Add interactive mode (`--interactive` or `-i`): Prompt user for keyword and file path if not provided as arguments
- Maintain backward compatibility: positional arguments still work (keyword and file path)
- Enhance help text with examples and detailed option descriptions
- Refactor Program.cs to separate concerns: argument parsing, filtering logic, and output generation

## Impact
- **Affected specs**: `log-filtering` (modified - enhanced CLI capabilities)
- **Affected code**:
  - `LogFilter/LogFilter.csproj` (add NuGet package reference)
  - `LogFilter/Program.cs` (refactored for System.CommandLine integration)
- **Breaking changes**: None - existing positional argument syntax remains supported
- **Dependencies**: Adds System.CommandLine NuGet package (Microsoft.Extensions.Hosting compatible)
