# log-filtering Specification Deltas

## MODIFIED Requirements

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

## ADDED Requirements

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

## MODIFIED Requirements (continued)

### Requirement: Case-Insensitive Keyword Matching
The application SHALL perform case-insensitive keyword matching by default, with an option to enable case-sensitive matching via the `--case-sensitive` flag.

#### Scenario: Default case-insensitive behavior
- **WHEN** user runs `LogFilter error app.log` without the `--case-sensitive` flag
- **THEN** lines containing "error", "ERROR", "Error", etc. are all matched

#### Scenario: Case-sensitive override
- **WHEN** user runs `LogFilter ERROR app.log --case-sensitive`
- **THEN** only lines containing "ERROR" in exact case are matched (existing scenarios from base spec still apply)

### Requirement: Timestamped Output File
The application SHALL create an auto-generated timestamped output file by default, but allow users to specify a custom output path via the `--output` option.

#### Scenario: Default auto-generated filename
- **WHEN** user does not specify `--output`
- **THEN** the output file uses the pattern `filtered_{keyword}_{timestamp}.log` in the source file's directory

#### Scenario: Custom output path overrides default
- **WHEN** user specifies `--output /custom/results.log`
- **THEN** the output file is written to `/custom/results.log` regardless of source file location or timestamp

#### Scenario: Custom output path with relative path
- **WHEN** user specifies `--output ./results/filtered.log`
- **THEN** the output file is written relative to the current working directory

#### Scenario: Custom output filename only
- **WHEN** user specifies `--output myresults.log` without a directory path
- **THEN** the output file is written to the source file's directory with the custom filename
