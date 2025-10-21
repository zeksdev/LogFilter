# log-filtering Specification Delta

## MODIFIED Requirements

### Requirement: Command-Line Interface
The application SHALL accept one or more keywords and file path as positional arguments OR via interactive prompts, with optional flags for customization. Multiple keywords SHALL be processed in parallel, each producing its own output file.

#### Scenario: Single keyword (backward compatible)
- **WHEN** user runs `LogFilter error app.log`
- **THEN** the application processes the file with a single keyword as before (case-insensitive, auto-generated output filename)

#### Scenario: Multiple keywords with space separation
- **WHEN** user runs `LogFilter error warning critical app.log`
- **THEN** the application processes all three keywords in parallel, creating three separate output files

#### Scenario: Multiple keywords with quoted keywords
- **WHEN** user runs `LogFilter "error occurred" "warning detected" app.log`
- **THEN** the application processes both multi-word keywords correctly

#### Scenario: Multiple keywords with custom output directory
- **WHEN** user runs `LogFilter error warning app.log --output /custom/path/`
- **THEN** all output files are created in `/custom/path/` with auto-generated filenames

#### Scenario: Multiple keywords with verbose output
- **WHEN** user runs `LogFilter error warning app.log --verbose`
- **THEN** the application displays processing information for each keyword and a final summary

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
- **THEN** the application prompts for keyword(s), file path, case sensitivity preference, and optional custom output path

#### Scenario: Interactive mode with short flag
- **WHEN** user runs `LogFilter -i`
- **THEN** the application prompts for all required inputs

#### Scenario: Help text
- **WHEN** user runs `LogFilter --help` or `LogFilter -h` or `LogFilter` with no arguments
- **THEN** the application displays usage instructions, option descriptions, and examples including multiple keyword usage

#### Scenario: Version information
- **WHEN** user runs `LogFilter --version`
- **THEN** the application displays its version number

#### Scenario: Invalid output directory
- **WHEN** user specifies `--output` with a path in a non-existent directory
- **THEN** the application displays an error message indicating the directory does not exist and exits with error code

#### Scenario: Combining options
- **WHEN** user runs `LogFilter error app.log --case-sensitive --verbose --output results.log`
- **THEN** the application performs case-sensitive filtering, displays verbose output, and writes to `results.log`

### Requirement: Timestamped Output File
The application SHALL create output files in the same directory as the source file (or custom output directory) with a name format of `filtered_{keyword}_{timestamp}.log`. When processing multiple keywords, all output files from a single invocation SHALL use the same timestamp to indicate they belong to the same processing run.

#### Scenario: Single keyword output file creation
- **WHEN** filtering a file located at `/path/to/logs/app.log` with keyword "error" at 2025-10-21 14:30:45
- **THEN** the output file is created as `/path/to/logs/filtered_error_20251021143045.log`

#### Scenario: Multiple keywords with shared timestamp
- **WHEN** filtering with keywords "error", "warning", "critical" at 2025-10-21 14:30:45
- **THEN** three output files are created:
  - `/path/to/logs/filtered_error_20251021143045.log`
  - `/path/to/logs/filtered_warning_20251021143045.log`
  - `/path/to/logs/filtered_critical_20251021143045.log`

#### Scenario: Special characters in keyword
- **WHEN** the keyword contains special characters that are invalid in filenames (e.g., `/`, `\`, `:`)
- **THEN** those characters are sanitized or replaced with safe alternatives (e.g., underscore)

#### Scenario: Timestamp format
- **WHEN** creating the output filename
- **THEN** the timestamp uses the format `yyyyMMddHHmmss` for consistent sorting and uniqueness

### Requirement: Interactive Prompts
The application SHALL provide an interactive mode that prompts users for required inputs when the `--interactive` flag is used. Interactive mode SHALL support entering multiple keywords via comma-separated input.

#### Scenario: Interactive keyword prompt for single keyword
- **WHEN** interactive mode is active and user enters a single keyword
- **THEN** the application processes that single keyword

#### Scenario: Interactive keyword prompt for multiple keywords
- **WHEN** interactive mode is active and user enters "error, warning, critical"
- **THEN** the application processes all three keywords in parallel

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
The application SHALL provide comprehensive help text via `--help` flag that includes usage patterns, option descriptions, and examples including multiple keyword usage.

#### Scenario: Help text structure
- **WHEN** user requests help via `--help` or `-h`
- **THEN** the output includes application description, usage syntax, list of options with descriptions, and usage examples

#### Scenario: Help text examples
- **WHEN** displaying help text
- **THEN** at least 3 practical examples are shown covering basic usage (including multiple keywords), options, and interactive mode

#### Scenario: Option documentation
- **WHEN** displaying help text
- **THEN** each option shows both long form (--option) and short form (-o), along with value type and description

## ADDED Requirements

### Requirement: Parallel Keyword Processing
The application SHALL process multiple keywords in parallel when more than one keyword is provided, with each keyword being filtered independently from the same source log file.

#### Scenario: Parallel processing of multiple keywords
- **WHEN** user provides 3 keywords for a large log file
- **THEN** all 3 keywords are processed concurrently (not sequentially)

#### Scenario: Independent keyword filtering
- **WHEN** processing multiple keywords
- **THEN** each keyword's filtering is independent (a line matching "error" goes to error's output file, lines matching "warning" go to warning's output file)

#### Scenario: Performance improvement
- **WHEN** processing multiple keywords in parallel
- **THEN** total processing time is significantly less than running the tool sequentially for each keyword

#### Scenario: Resource utilization
- **WHEN** processing multiple keywords
- **THEN** the application utilizes multiple CPU cores efficiently

### Requirement: Multi-Keyword Error Handling
The application SHALL handle errors for individual keywords without aborting the processing of other keywords. A summary of successes and failures SHALL be displayed at the end.

#### Scenario: Partial success with one keyword failing
- **WHEN** processing 3 keywords and one keyword's output file cannot be written
- **THEN** the other 2 keywords complete successfully and the failure is reported in the summary

#### Scenario: All keywords succeed
- **WHEN** all keywords process successfully
- **THEN** the application displays success for each keyword and exits with code 0

#### Scenario: All keywords fail
- **WHEN** all keywords fail to process
- **THEN** the application displays errors for each keyword and exits with non-zero code

#### Scenario: Error summary in verbose mode
- **WHEN** processing multiple keywords with verbose mode enabled
- **THEN** the application displays a summary showing which keywords succeeded and which failed with their error messages

### Requirement: Multi-Keyword Progress Reporting
The application SHALL provide progress feedback when processing multiple keywords in verbose mode, showing the status of each keyword's processing.

#### Scenario: Verbose mode with multiple keywords
- **WHEN** user runs with verbose flag and multiple keywords
- **THEN** the application displays:
  - Starting message with list of keywords to process
  - Progress/completion message for each keyword
  - Final summary with total lines processed per keyword

#### Scenario: Non-verbose mode with multiple keywords
- **WHEN** user runs without verbose flag and multiple keywords
- **THEN** the application displays only the final summary of output files created

#### Scenario: Single keyword verbose output (backward compatible)
- **WHEN** user runs with verbose flag and single keyword
- **THEN** the output format matches the original single-keyword verbose output
