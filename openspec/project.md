# Project Context

## Purpose
LogFilter is a command-line utility that reads log files, filters lines by input keywords, and writes the filtered results to a separate file. The tool is designed to help developers and system administrators quickly extract relevant information from large log files.

### Key Goals
- Simple and efficient log file filtering
- Keyword-based line matching
- Output filtered results to separate files for easy analysis

## Tech Stack
- **Language**: C#
- **Framework**: .NET 9.0
- **Project Type**: Console Application
- **Features**: Implicit usings enabled, nullable reference types enabled

## Project Conventions

### Code Style
- Follow **Microsoft C# coding conventions**
- Use PascalCase for public members and types
- Use camelCase for private fields and local variables
- Nullable reference types are enabled - handle null cases appropriately
- Implicit usings are enabled - avoid redundant using statements

### Architecture Patterns
- Console application with single entry point (Program.cs)
- Focus on simple, readable code for file I/O and string processing
- Keep business logic separate from I/O operations for maintainability

### Testing Strategy
- **No formal testing framework** - this is a quick utility project
- Manual testing with sample log files is acceptable

### Git Workflow
- Working on **master** branch
- Use descriptive commit messages
- OpenSpec workflow for planning larger changes (via `/openspec:proposal`, `/openspec:apply`, `/openspec:archive`)

## Domain Context
- **Log Files**: The tool processes text-based log files with line-by-line structure
- **Filtering**: Keyword matching should be straightforward (exact match or contains)
- **Output**: Filtered results are written to a new file, preserving original line format

## Important Constraints
- Must handle large log files efficiently without loading entire file into memory
- Should provide clear error messages for file access issues
- Cross-platform compatibility (runs on Windows, macOS, Linux via .NET)

## External Dependencies
- .NET 9.0 runtime (no additional NuGet packages currently)
