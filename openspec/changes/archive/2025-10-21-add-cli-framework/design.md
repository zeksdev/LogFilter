# Design: CLI Framework Integration

## Architecture Decision

### Why System.CommandLine?
System.CommandLine is Microsoft's official, modern command-line parsing library for .NET. It provides:
- **Type safety**: Strongly-typed options and arguments with automatic parsing and validation
- **Rich help generation**: Automatic `--help` with descriptions, examples, and option details
- **Extensibility**: Easy to add new options, subcommands, and validators in the future
- **Active development**: Part of .NET ecosystem with ongoing support
- **No bloat**: Focused on parsing and validation without UI concerns

### Command Structure

```
logfilter <keyword> <file-path> [options]
```

**Positional Arguments** (required, maintains backward compatibility):
- `keyword`: The search term (string)
- `file-path`: Path to log file (string, validated for existence)

**Options** (all optional):
- `--output <path>`, `-o <path>`: Custom output file location
- `--case-sensitive`, `-c`: Enable case-sensitive matching (default: case-insensitive)
- `--verbose`, `-v`: Show detailed progress information
- `--interactive`, `-i`: Interactive mode (prompts for inputs)

**Examples**:
```bash
# Basic usage (current behavior)
logfilter error app.log

# Custom output location
logfilter error app.log --output ./results/errors.log

# Case-sensitive search
logfilter ERROR app.log --case-sensitive

# Verbose output
logfilter warning app.log --verbose

# Interactive mode
logfilter --interactive
```

## Code Organization

### Current Structure
```
Program.cs (103 lines)
├── Main(string[] args)
│   ├── Manual argument parsing
│   ├── File validation
│   ├── Filtering logic
│   ├── Output file generation
│   └── Error handling
└── SanitizeFilename(string)
```

### Proposed Structure
```
Program.cs
├── Main(string[] args)
│   ├── RootCommand setup
│   ├── Option definitions
│   ├── Handler registration
│   └── InvocationContext execution
├── FilterCommand class (command handler)
│   ├── Execute(keyword, filePath, options)
│   ├── Interactive mode handling
│   ├── Filtering logic (extracted)
│   └── Result reporting
└── Utilities
    ├── SanitizeFilename(string)
    └── ValidateFilePath(string)
```

## Interactive Mode Design

When `--interactive` flag is used or no arguments provided:
1. Prompt: "Enter keyword to search for:"
2. Prompt: "Enter log file path:"
3. Prompt: "Case-sensitive search? (y/N):"
4. Prompt: "Custom output path? (leave empty for auto-generated):"
5. Execute filtering with collected inputs

**Note**: Interactive mode uses simple `Console.ReadLine()` for prompting (no Spectre.Console dependency). This keeps the implementation minimal and focused.

## Backward Compatibility

The existing usage pattern MUST continue to work:
```bash
logfilter error app.log
```

This is achieved by:
- Defining `keyword` and `file-path` as positional arguments (not options)
- Making all new flags optional with sensible defaults
- Preserving existing output filename format when `--output` is not specified

## Validation Strategy

System.CommandLine provides built-in validation:
- **File path validation**: Check file exists before processing
- **Output path validation**: Check directory exists and is writable
- **Keyword validation**: Ensure non-empty string

Manual testing remains sufficient (no unit test framework added).

## Performance Considerations

- System.CommandLine has minimal overhead (microseconds for parsing)
- Filtering logic remains unchanged - streaming approach preserved
- No impact on memory usage for large files
- Interactive mode adds user input latency but doesn't affect processing performance

## Future Extensibility

This design enables future enhancements:
- Regex pattern matching (new option: `--regex` or `-r`)
- Multiple keywords with AND/OR logic (new option: `--all-keywords`, `--any-keyword`)
- Output format options (JSON, CSV) (new option: `--format`)
- Subcommands for different operations (e.g., `logfilter stats` for log analysis)
- Configuration file support (e.g., `.logfilterrc`)
