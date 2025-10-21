# Implementation Tasks

## 1. Command-Line Argument Parsing
- [x] 1.1 Add command-line argument validation to check for exactly 2 arguments
- [x] 1.2 Extract keyword and source file path from arguments
- [x] 1.3 Display usage help message when arguments are invalid
- [x] 1.4 Validate that source file exists before processing

## 2. Core Filtering Logic
- [x] 2.1 Implement streaming file reader using `StreamReader` for memory efficiency
- [x] 2.2 Implement case-insensitive keyword matching using `String.Contains` with `StringComparison.OrdinalIgnoreCase`
- [x] 2.3 Process each line and collect matching lines

## 3. Output File Generation
- [x] 3.1 Generate timestamp string in `yyyyMMddHHmmss` format
- [x] 3.2 Sanitize keyword for use in filename (replace invalid characters with underscores)
- [x] 3.3 Construct output filename: `filtered_{keyword}_{timestamp}.log`
- [x] 3.4 Determine output directory (same as source file directory)
- [x] 3.5 Write filtered lines to output file using `StreamWriter`

## 4. Error Handling
- [x] 4.1 Add try-catch for `FileNotFoundException` with user-friendly message
- [x] 4.2 Add try-catch for `UnauthorizedAccessException` for read/write permission errors
- [x] 4.3 Add try-catch for `IOException` for general I/O failures
- [x] 4.4 Display success message with output file path on completion
- [x] 4.5 Return appropriate exit codes (0 for success, 1 for errors)

## 5. Manual Testing
- [x] 5.1 Test with small log file containing keyword in various cases
- [x] 5.2 Test with log file where keyword appears in some lines but not others
- [x] 5.3 Test with non-existent file path
- [x] 5.4 Test with missing command-line arguments
- [x] 5.5 Test output file naming and location
- [x] 5.6 Test with keyword containing special characters