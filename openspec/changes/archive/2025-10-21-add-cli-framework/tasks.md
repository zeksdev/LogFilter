# Implementation Tasks

## 1. Add System.CommandLine Dependency
- [x] 1.1 Add System.CommandLine NuGet package to LogFilter.csproj
- [x] 1.2 Verify package installation with `dotnet restore`
- [x] 1.3 Verify build succeeds after adding package

## 2. Define Command Structure
- [x] 2.1 Create RootCommand with description
- [x] 2.2 Define `keyword` argument (string, required position 0)
- [x] 2.3 Define `file-path` argument (string, required position 1)
- [x] 2.4 Add file path existence validation to `file-path` argument
- [x] 2.5 Define `--output` / `-o` option (string, optional, file path)
- [x] 2.6 Define `--case-sensitive` / `-c` option (boolean flag, default false)
- [x] 2.7 Define `--verbose` / `-v` option (boolean flag, default false)
- [x] 2.8 Define `--interactive` / `-i` option (boolean flag, default false)
- [x] 2.9 Add output directory validation for `--output` option

## 3. Refactor Core Filtering Logic
- [x] 3.1 Extract filtering logic into separate method `FilterLogFile(keyword, filePath, caseSensitive, verbose)`
- [x] 3.2 Add verbose logging for total lines read
- [x] 3.3 Add verbose logging for lines matched count
- [x] 3.4 Add verbose logging for processing time (start/end timestamps)
- [x] 3.5 Modify keyword matching to respect `caseSensitive` parameter (use `StringComparison.Ordinal` when true)

## 4. Implement Output Path Handling
- [x] 4.1 Update output generation to check if custom output path is provided
- [x] 4.2 Use custom output path when `--output` is specified
- [x] 4.3 Fall back to auto-generated timestamped filename when `--output` is not specified
- [x] 4.4 Handle relative vs absolute paths for custom output
- [x] 4.5 Handle filename-only output (place in source file's directory)

## 5. Implement Interactive Mode
- [x] 5.1 Detect when `--interactive` flag is set or no arguments provided
- [x] 5.2 Prompt for keyword input and read from console
- [x] 5.3 Prompt for file path input and validate existence, re-prompt on failure
- [x] 5.4 Prompt for case-sensitive preference (y/N) and parse boolean response
- [x] 5.5 Prompt for custom output path (allow empty for default)
- [x] 5.6 Execute filtering with collected interactive inputs

## 6. Enhance Help and Documentation
- [x] 6.1 Add descriptive text to RootCommand
- [x] 6.2 Add descriptions to all arguments and options
- [x] 6.3 Add usage examples to help text (at least 3 examples)
- [x] 6.4 Configure help option to trigger on no arguments, `--help`, or `-h`
- [x] 6.5 Add version information (can use assembly version)

## 7. Integrate Command Handler
- [x] 7.1 Create command handler method that accepts all arguments and options
- [x] 7.2 Wire handler to RootCommand using `SetHandler`
- [x] 7.3 Replace manual argument parsing in Main with RootCommand.Invoke
- [x] 7.4 Ensure exit codes are properly returned (0 for success, 1 for errors)

## 8. Error Handling Updates
- [x] 8.1 Update error messages to reference new option names (e.g., "use --output to specify custom path")
- [x] 8.2 Add validation error for invalid output directory
- [x] 8.3 Ensure all existing exception handlers remain functional
- [x] 8.4 Add error handling for interactive mode input failures

## 9. Testing and Validation
- [x] 9.1 Test basic positional arguments (backward compatibility): `logfilter error app.log`
- [x] 9.2 Test `--output` with absolute path
- [x] 9.3 Test `--output` with relative path
- [x] 9.4 Test `--output` with filename only
- [x] 9.5 Test `--case-sensitive` flag with mixed-case keyword
- [x] 9.6 Test `--verbose` flag to verify detailed output
- [x] 9.7 Test combining multiple flags: `--case-sensitive --verbose --output`
- [x] 9.8 Test `--interactive` mode with valid inputs
- [x] 9.9 Test interactive mode with invalid file path (should re-prompt)
- [x] 9.10 Test `--help` displays comprehensive usage information
- [x] 9.11 Test with no arguments (should show help or enter interactive mode)
- [x] 9.12 Test short flags: `-o`, `-c`, `-v`, `-i`, `-h`
- [x] 9.13 Test error handling for non-existent output directory
- [x] 9.14 Verify build succeeds with no warnings
- [x] 9.15 Verify all existing scenarios from log-filtering spec still work
