# LogFilter Usage Examples

## Basic Usage

### Single Keyword (Backward Compatible)
```bash
# Filter for a single keyword
LogFilter error app.log

# Filter with case-sensitive matching
LogFilter ERROR app.log --case-sensitive

# Filter with verbose output
LogFilter error app.log --verbose
```

## Multiple Keywords (New Feature)

### Basic Multiple Keywords
```bash
# Filter for multiple keywords in parallel
LogFilter error warning critical app.log

# Each keyword creates its own output file:
# - filtered_error_20251021143045.log
# - filtered_warning_20251021143045.log
# - filtered_critical_20251021143045.log
```

### Multiple Keywords with Options
```bash
# Case-sensitive matching for multiple keywords
LogFilter ERROR WARNING CRITICAL app.log --case-sensitive

# Verbose output shows processing summary
LogFilter error warning critical app.log --verbose

# Output:
# Processing 3 keywords in parallel:
#   - error
#   - warning
#   - critical
#
# === Processing Summary ===
# ✓ 'critical': 1 line(s) → /path/to/filtered_critical_20251021143045.log
# ✓ 'error': 3 line(s) → /path/to/filtered_error_20251021143045.log
# ✓ 'warning': 2 line(s) → /path/to/filtered_warning_20251021143045.log
#
# Completed: 3/3 keywords processed successfully
```

### Multi-word Keywords
```bash
# Use quotes for keywords containing spaces
LogFilter "connection error" "timeout exception" app.log
```

## Interactive Mode

### Single Keyword Interactive
```bash
LogFilter --interactive

# Prompts:
# Enter keyword(s) to search for (comma-separated for multiple): error
# Enter log file path: app.log
# Case-sensitive search? (y/N): n
# Custom output path? (leave empty for auto-generated):
```

### Multiple Keywords Interactive
```bash
LogFilter -i

# Prompts:
# Enter keyword(s) to search for (comma-separated for multiple): error, warning, critical
# Enter log file path: app.log
# Case-sensitive search? (y/N): n
# Custom output path? (leave empty for auto-generated):
```

## Custom Output

### Single Keyword with Custom Output
```bash
# Custom output file
LogFilter error app.log --output errors.log

# Custom output directory
LogFilter error app.log --output /custom/path/errors.log
```

### Multiple Keywords with Custom Output Directory
```bash
# When using --output with multiple keywords, specify a directory
# All output files will be created in that directory
LogFilter error warning app.log --output /custom/path/

# Results in:
# /custom/path/filtered_error_20251021143045.log
# /custom/path/filtered_warning_20251021143045.log
```

## Help and Version

```bash
# Show help
LogFilter --help
LogFilter -h

# Show version
LogFilter --version
```

## Sample Test Data

Create a test log file:
```bash
cat > test.log << 'EOF'
2025-10-21 10:00:01 INFO Application started
2025-10-21 10:00:05 ERROR Failed to connect to database
2025-10-21 10:00:10 WARNING Memory usage at 80%
2025-10-21 10:00:15 INFO User login successful
2025-10-21 10:00:20 ERROR Null reference exception in module X
2025-10-21 10:00:25 CRITICAL System crash imminent
2025-10-21 10:00:30 WARNING High CPU usage detected
2025-10-21 10:00:35 INFO Processing completed
2025-10-21 10:00:40 ERROR Timeout waiting for response
2025-10-21 10:00:45 INFO Backup completed successfully
EOF
```

Test with multiple keywords:
```bash
LogFilter ERROR WARNING CRITICAL test.log --verbose

# Expected output:
# Processing 3 keywords in parallel:
#   - ERROR
#   - WARNING
#   - CRITICAL
#
# === Processing Summary ===
# ✓ 'CRITICAL': 1 line(s) → filtered_CRITICAL_20251021143045.log
# ✓ 'ERROR': 3 line(s) → filtered_ERROR_20251021143045.log
# ✓ 'WARNING': 2 line(s) → filtered_WARNING_20251021143045.log
#
# Completed: 3/3 keywords processed successfully
```
