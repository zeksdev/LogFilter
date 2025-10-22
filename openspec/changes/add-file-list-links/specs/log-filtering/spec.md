# log-filtering Specification Delta

## ADDED Requirements

### Requirement: Terminal Hyperlink Support
The application SHALL output clickable file path links in supported terminals using OSC 8 hyperlink escape sequences. For terminals without hyperlink support, the application SHALL gracefully fall back to plain text file paths.

#### Scenario: Hyperlink output in supported terminal
- **WHEN** user runs the application in a terminal that supports hyperlinks (Windows Terminal, iTerm2, GNOME Terminal 3.x+, VS Code terminal)
- **THEN** output file paths are displayed as clickable hyperlinks that open the file when clicked

#### Scenario: Plain text fallback in unsupported terminal
- **WHEN** user runs the application in a terminal without hyperlink support
- **THEN** output file paths are displayed as plain text (identical to current behavior)

#### Scenario: Single keyword success message with hyperlink
- **WHEN** filtering completes successfully with one keyword in a supported terminal
- **THEN** the success message displays the output file path as a clickable link: "Filtered results written to [clickable-path]"

#### Scenario: Multiple keywords success message with hyperlinks
- **WHEN** filtering completes successfully with multiple keywords in a supported terminal
- **THEN** the success message lists all output file paths as clickable links

#### Scenario: Verbose mode output with hyperlinks
- **WHEN** verbose mode is enabled in a supported terminal
- **THEN** all output file path references in verbose messages are displayed as clickable links

#### Scenario: Interactive mode output with hyperlinks
- **WHEN** interactive mode completes successfully in a supported terminal
- **THEN** the final success message shows the output file path as a clickable link

#### Scenario: File URI formatting on Windows
- **WHEN** running on Windows with drive letters (e.g., C:\Users\...\output.log)
- **THEN** the file URI is correctly formatted as `file:///C:/Users/.../output.log` for hyperlink functionality

#### Scenario: File URI formatting on Unix-like systems
- **WHEN** running on Unix-like systems (Linux, macOS) with absolute paths
- **THEN** the file URI is correctly formatted as `file:///path/to/output.log` for hyperlink functionality

#### Scenario: Terminal capability detection
- **WHEN** the application starts
- **THEN** it detects terminal hyperlink support by checking environment variables (TERM_PROGRAM, WT_SESSION, VTE_VERSION, COLORTERM)

#### Scenario: Hyperlink format compliance
- **WHEN** generating hyperlinks
- **THEN** the application uses the correct OSC 8 escape sequence format: `\e]8;;file://path\e\\text\e]8;;\e\\`
