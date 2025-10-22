# Testing Clickable File Links

This document explains how to test the terminal hyperlink feature added in the `add-file-list-links` OpenSpec change.

## What Was Implemented

The LogFilter application now outputs clickable file paths in supported terminals using OSC 8 escape sequences. When you run the application, output file paths will be clickable hyperlinks that open the file directly.

## Supported Terminals

The feature automatically detects and works with:
- **Windows Terminal** (WT_SESSION environment variable)
- **iTerm2** (macOS)
- **VS Code integrated terminal**
- **GNOME Terminal 3.x+** (VTE-based terminals)
- **Konsole** and other modern terminals with truecolor support

For unsupported terminals, the feature gracefully falls back to plain text paths.

## How to Test

### 1. Build the Application

```bash
cd LogFilter
dotnet build
dotnet run
```

### 2. Test Single Keyword with Verbose Mode

```bash
dotnet run -- error test-sample.log -v
```

Expected output:
```
Reading file: test-sample.log
Searching for: 'error'
Case-sensitive: False

Total lines read: 13
Lines matched: 4
Processing time: Xms

Success: Filtered 4 line(s) containing 'error'
Output file: [CLICKABLE LINK]/path/to/filtered_error_TIMESTAMP.log
```

In supported terminals, the output file path will be underlined or styled, and clicking it will open the file.

### 3. Test Multiple Keywords

```bash
dotnet run -- error warning info test-sample.log -v
```

Expected output:
```
Processing 3 keywords in parallel:
  - error
  - warning
  - info

=== Processing Summary ===
✓ 'error': 4 line(s) → [CLICKABLE LINK]filtered_error_TIMESTAMP.log
✓ 'info': 5 line(s) → [CLICKABLE LINK]filtered_info_TIMESTAMP.log
✓ 'warning': 3 line(s) → [CLICKABLE LINK]filtered_warning_TIMESTAMP.log

Completed: 3/3 keywords processed successfully
```

### 4. Test Interactive Mode

```bash
dotnet run -- -i
```

Follow the prompts and verify the final output file path is clickable.

### 5. Test Fallback (Unsupported Terminal)

To test the plain text fallback, unset the terminal detection environment variables:

```bash
unset WT_SESSION TERM_PROGRAM VTE_VERSION COLORTERM
dotnet run -- error test-sample.log -v
```

The output should show plain text paths without hyperlink formatting.

### 6. Verify Hyperlink Format

In a supported terminal, you can verify the OSC 8 escape sequences are being output correctly by piping to `cat -v`:

```bash
dotnet run -- error test-sample.log -v | cat -v
```

You should see escape sequences like:
```
^[]8;;file:///path/to/file.log^[\path/to/file.log^[]8;;^[\
```

## Implementation Details

### Terminal Detection (Program.cs:20-56)
The `DetectHyperlinkSupport()` method checks environment variables:
- `WT_SESSION` → Windows Terminal
- `TERM_PROGRAM` → iTerm2, VS Code
- `VTE_VERSION` → GNOME Terminal (version >= 5000)
- `COLORTERM=truecolor` + TERM → Konsole, xterm variants

### Hyperlink Formatting (Program.cs:61-70)
The `FormatPath()` method wraps file paths with OSC 8 sequences:
```
\x1b]8;;file://URI\x1b\\TEXT\x1b]8;;\x1b\\
```

### File URI Generation (Program.cs:75-92)
The `PathToFileUri()` method creates proper file:// URIs:
- Windows: `file:///C:/path/to/file.log`
- Unix/Linux/macOS: `file:///path/to/file.log`

### Output Integration
Hyperlinks are applied in:
- Multi-keyword summary (Program.cs:332-333)
- Verbose single-keyword output (Program.cs:465-466)

## Troubleshooting

### Links not clickable
1. Verify your terminal supports hyperlinks (check list above)
2. Update your terminal to the latest version
3. Check if terminal detection is working (add debug output if needed)

### Links open but file not found
1. Ensure file paths are absolute (should be handled automatically)
2. Check file:// URI format matches your OS conventions
3. Verify the file actually exists at the specified path

### Plain text shown instead of links
This is expected behavior for terminals without hyperlink support. The feature is working correctly by falling back gracefully.
