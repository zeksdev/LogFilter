# Add Clickable File Links in Shell Output

## Why
Users want to quickly open created/filtered output files directly from the terminal without manually copying file paths. Modern terminals support hyperlinks (OSC 8 escape sequences), allowing users to click on file paths to open them directly. This improves user experience by reducing the friction between seeing output file information and accessing those files.

## What Changes
- Add terminal hyperlink support for output file paths in success messages
- Detect terminal hyperlink capability (check for supported terminals)
- Gracefully degrade to plain text paths on terminals without hyperlink support
- Apply hyperlinks to all output file paths shown in success messages and verbose output
- Maintain backward compatibility with existing output format

## Impact
- **Affected specs**: log-filtering
- **Affected code**:
  - LogFilter/Program.cs (output formatting, success messages, terminal detection)
- **Breaking changes**: None (backward compatible - plain text fallback for unsupported terminals)
- **User experience**: Improved workflow with clickable file paths in supported terminals (Windows Terminal, iTerm2, GNOME Terminal, etc.)
