# Add Keyword Filtering

## Why
LogFilter currently has no functionality beyond a "Hello World" placeholder. Users need a simple command-line tool to extract relevant log lines containing specific keywords from large log files without manual searching.

## What Changes
- Add command-line argument parsing to accept keyword and source log file path
- Implement line-by-line log file reading with streaming (memory-efficient for large files)
- Add case-insensitive keyword matching for each line
- Create timestamped output files in the same directory as source file
- Output filename format: `filtered_{keyword}_{timestamp}.log`
- Display helpful error messages for file access issues and invalid arguments

## Impact
- **Affected specs**: `log-filtering` (new capability)
- **Affected code**: `LogFilter/Program.cs` (complete rewrite from template)
- **Breaking changes**: None (initial feature implementation)