# log-filtering Specification

## Purpose
TBD - created by archiving change add-keyword-filtering. Update Purpose after archive.
## Requirements
### Requirement: Command-Line Interface
The application SHALL accept keyword and file path as positional arguments OR via interactive prompts, with optional flags for customization.

#### Scenario: Positional arguments (backward compatible)
- **WHEN** user runs `LogFilter <keyword> <file-path>`
- **THEN** the application processes the file with default options (case-insensitive, auto-generated output filename)

#### Scenario: Custom output path
- **WHEN** user runs `LogFilter error app.log --output /custom/path/errors.log`
- **THEN** the filtered results are written to `/custom/path/errors.log` instead of the auto-generated filename

#### Scenario: Custom output path with short flag
- **WHEN** user runs `LogFilter error app.log -o /custom/path/errors.log`
- **THEN** the filtered results are written to `/custom/path/errors.log`

#### Scenario: Case-sensitive matching
- **WHEN** user runs `LogFilter ERROR app.log --case-sensitive`
- **THEN** only lines containing "ERROR" in exact case are matched (not "error" or "Error")

#### Scenario: Case-sensitive matching with short flag
- **WHEN** user runs `LogFilter ERROR app.log -c`
- **THEN** only lines containing "ERROR" in exact case are matched

#### Scenario: Verbose output
- **WHEN** user runs `LogFilter error app.log --verbose`
- **THEN** the application displays processing information including total lines read, lines matched, and processing time

#### Scenario: Verbose output with short flag
- **WHEN** user runs `LogFilter error app.log -v`
- **THEN** the application displays detailed processing information

#### Scenario: Interactive mode
- **WHEN** user runs `LogFilter --interactive`
- **THEN** the application prompts for keyword, file path, case sensitivity preference, and optional custom output path

#### Scenario: Interactive mode with short flag
- **WHEN** user runs `LogFilter -i`
- **THEN** the application prompts for all required inputs

#### Scenario: Help text
- **WHEN** user runs `LogFilter --help` or `LogFilter -h` or `LogFilter` with no arguments
- **THEN** the application displays usage instructions, option descriptions, and examples

#### Scenario: Version information
- **WHEN** user runs `LogFilter --version`
- **THEN** the application displays its version number

#### Scenario: Invalid output directory
- **WHEN** user specifies `--output` with a path in a non-existent directory
- **THEN** the application displays an error message indicating the directory does not exist and exits with error code

#### Scenario: Combining options
- **WHEN** user runs `LogFilter error app.log --case-sensitive --verbose --output results.log`
- **THEN** the application performs case-sensitive filtering, displays verbose output, and writes to `results.log`

### Requirement: Line-by-Line Filtering
The application SHALL read the source log file line-by-line and write matching lines to the output file without loading the entire file into memory.

#### Scenario: Small log file
- **WHEN** processing a log file smaller than available memory
- **THEN** the application streams through the file line-by-line

#### Scenario: Large log file
- **WHEN** processing a log file larger than 1GB
- **THEN** the application handles it efficiently using streaming without memory issues

### Requirement: Case-Insensitive Keyword Matching
The application SHALL perform case-insensitive keyword matching when searching for the keyword within each line.

#### Scenario: Exact case match
- **WHEN** a line contains the keyword with exact case (e.g., keyword "ERROR" matches "ERROR in database")
- **THEN** the line is written to the filtered output file

#### Scenario: Different case match
- **WHEN** a line contains the keyword in different case (e.g., keyword "error" matches "ERROR in database")
- **THEN** the line is written to the filtered output file

#### Scenario: No match
- **WHEN** a line does not contain the keyword in any case variation
- **THEN** the line is not written to the filtered output file

### Requirement: Timestamped Output File
The application SHALL create an output file in the same directory as the source file with a name format of `filtered_{keyword}_{timestamp}.log`.

#### Scenario: Output file creation
- **WHEN** filtering a file located at `/path/to/logs/app.log` with keyword "error" at 2025-10-21 14:30:45
- **THEN** the output file is created as `/path/to/logs/filtered_error_20251021143045.log`

#### Scenario: Special characters in keyword
- **WHEN** the keyword contains special characters that are invalid in filenames (e.g., `/`, `\`, `:`)
- **THEN** those characters are sanitized or replaced with safe alternatives (e.g., underscore)

#### Scenario: Timestamp format
- **WHEN** creating the output filename
- **THEN** the timestamp uses the format `yyyyMMddHHmmss` for consistent sorting and uniqueness

### Requirement: Error Handling
The application SHALL provide clear error messages for common failure scenarios and exit with appropriate error codes.

#### Scenario: File access denied
- **WHEN** the application cannot read the source file due to permissions
- **THEN** a clear error message is displayed indicating permission issue and the application exits with error code

#### Scenario: Output file write failure
- **WHEN** the application cannot write to the output file (e.g., disk full, permissions)
- **THEN** a clear error message is displayed indicating the write failure and the application exits with error code

#### Scenario: Success completion
- **WHEN** filtering completes successfully
- **THEN** the application displays a success message with the output file path and exits with code 0

### Requirement: Interactive Prompts
The application SHALL provide an interactive mode that prompts users for required inputs when the `--interactive` flag is used.

#### Scenario: Interactive keyword prompt
- **WHEN** interactive mode is active
- **THEN** the application prompts "Enter keyword to search for:" and accepts user input

#### Scenario: Interactive file path prompt
- **WHEN** interactive mode is active
- **THEN** the application prompts "Enter log file path:" and validates the file exists

#### Scenario: Interactive case sensitivity prompt
- **WHEN** interactive mode is active
- **THEN** the application prompts "Case-sensitive search? (y/N):" with default to No

#### Scenario: Interactive output path prompt
- **WHEN** interactive mode is active
- **THEN** the application prompts "Custom output path? (leave empty for auto-generated):" and accepts empty input for default behavior

#### Scenario: Interactive validation failure
- **WHEN** interactive mode receives invalid input (e.g., non-existent file path)
- **THEN** the application displays an error and re-prompts for valid input

### Requirement: Command-Line Help and Documentation
The application SHALL provide comprehensive help text via `--help` flag that includes usage patterns, option descriptions, and examples.

#### Scenario: Help text structure
- **WHEN** user requests help via `--help` or `-h`
- **THEN** the output includes application description, usage syntax, list of options with descriptions, and usage examples

#### Scenario: Help text examples
- **WHEN** displaying help text
- **THEN** at least 3 practical examples are shown covering basic usage, options, and interactive mode

#### Scenario: Option documentation
- **WHEN** displaying help text
- **THEN** each option shows both long form (--option) and short form (-o), along with value type and description

