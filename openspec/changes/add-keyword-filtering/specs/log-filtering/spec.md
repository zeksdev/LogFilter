# Log Filtering Capability

## ADDED Requirements

### Requirement: Command-Line Interface
The application SHALL accept two required command-line arguments: a keyword to search for and a path to the source log file.

#### Scenario: Valid arguments provided
- **WHEN** user runs `LogFilter <keyword> <source-file-path>`
- **THEN** the application processes the file and filters lines

#### Scenario: Missing arguments
- **WHEN** user runs the application without providing both required arguments
- **THEN** the application displays usage instructions and exits with error code

#### Scenario: Invalid file path
- **WHEN** user provides a file path that does not exist
- **THEN** the application displays a clear error message indicating the file was not found and exits with error code

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