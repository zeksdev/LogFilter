# Design: Parallel Keyword Processing

## Context
The LogFilter application currently processes one keyword at a time. Users often need to extract multiple different keywords from the same log file (e.g., "ERROR", "WARNING", "CRITICAL"). Running the tool sequentially for each keyword means reading the same large log file multiple times, which is inefficient.

**Constraints**:
- Must maintain backward compatibility with existing single-keyword usage
- Large log files (>1GB) should not be loaded into memory
- Cross-platform compatibility (Windows, macOS, Linux)

**Stakeholders**:
- Developers and system administrators filtering production logs
- Users processing large log files with multiple search criteria

## Goals / Non-Goals

### Goals
- Accept multiple keywords in a single invocation
- Process each keyword in parallel to improve performance
- Create separate output files for each keyword
- Maintain line-by-line streaming for memory efficiency
- Provide clear progress feedback for multiple keyword processing

### Non-Goals
- Combining keywords with boolean operators (AND/OR) - each keyword is independent
- Filtering lines that match ALL keywords - each keyword produces its own output
- Merging output files - each keyword gets its own timestamped file
- Deduplication across keyword results

## Decisions

### Decision 1: Multiple Keyword Input Format
**Choice**: Support both space-separated and quoted keywords
- `LogFilter "ERROR" "WARNING" app.log` - space-separated (recommended)
- Interactive mode can prompt for comma-separated list

**Why**:
- Space-separated aligns with standard CLI conventions
- Allows keywords with spaces when quoted
- Simple parsing with System.CommandLine

**Alternatives considered**:
- Comma-separated only: Less intuitive, requires escaping commas in keywords
- File input: Over-engineered for most use cases
- Repeated flag (--keyword ERROR --keyword WARNING): Verbose

### Decision 2: Parallel Processing Strategy
**Choice**: Use `Parallel.ForEachAsync` with degree of parallelism = CPU cores

**Why**:
- Simple API, built-in to .NET
- Automatic work distribution
- Respects system resources
- Each keyword reads the file independently (no shared state complexity)

**Alternatives considered**:
- `Task.WhenAll` with manual task creation: More boilerplate, same result
- Sequential processing: Slower, wastes CPU during I/O
- Single file read with multiple filters: Complex state management, memory issues with large files

### Decision 3: File Reading Strategy
**Choice**: Each parallel task reads the log file independently using separate StreamReader instances

**Why**:
- Maintains line-by-line streaming (no memory issues)
- No shared state or synchronization needed
- OS-level file caching makes multiple reads efficient
- Simplest implementation

**Alternatives considered**:
- Single read, distribute lines: Requires loading into memory or complex buffering
- Memory-mapped file: Platform-specific behavior, added complexity
- Buffered reader with broadcast: Complex synchronization

### Decision 4: Output File Naming
**Choice**: Same timestamp for all files in a single invocation
- `filtered_ERROR_20251021143045.log`
- `filtered_WARNING_20251021143045.log`

**Why**:
- Groups related outputs from same run
- Easy to identify which files belong together
- Maintains existing naming convention

**Alternatives considered**:
- Different timestamps per keyword: Harder to correlate
- Subdirectory per run: Additional complexity, directory management

### Decision 5: Backward Compatibility
**Choice**: Single keyword works exactly as before
- `LogFilter ERROR app.log` still works
- No breaking changes to existing scripts/automation

**Why**:
- Minimizes disruption
- Gradual adoption
- Existing documentation remains valid

## Architecture

```
Main()
  ├─ Parse arguments (keyword array)
  ├─ If interactive: prompt for comma-separated keywords, split into array
  └─ ProcessKeywordsAsync(keywords[], filePath, options)
      ├─ Generate shared timestamp
      ├─ Parallel.ForEachAsync(keywords, async keyword =>
      │    └─ ExecuteFilterForKeyword(keyword, filePath, timestamp, options)
      │         ├─ Open StreamReader
      │         ├─ Filter line-by-line
      │         └─ Write to output file
      └─ Display summary
```

## Risks / Trade-offs

### Risk 1: File System Contention
**Risk**: Multiple parallel reads of the same file might cause I/O bottleneck

**Mitigation**:
- OS file caching makes this efficient in practice
- Limit parallelism to CPU core count
- Users can disable parallelism if needed (future enhancement)

### Risk 2: Disk Space for Multiple Outputs
**Risk**: Multiple output files consume more disk space than single merged file

**Mitigation**:
- This is intentional behavior (separate files per keyword)
- Document in help text
- Users control which keywords to search

### Risk 3: Error Handling Complexity
**Risk**: One keyword failing shouldn't abort others

**Mitigation**:
- Wrap each keyword's processing in try-catch
- Collect errors and display at end
- Exit code reflects partial success

## Migration Plan

### Phase 1: Implementation
1. Update CLI parsing to accept array of keywords
2. Implement parallel processing
3. Update interactive mode

### Phase 2: Testing
1. Test backward compatibility (single keyword)
2. Test multiple keywords (2-10 keywords)
3. Test with large files (>1GB)
4. Test error scenarios

### Phase 3: Rollout
1. Update help text with examples
2. No migration needed (backward compatible)

### Rollback Plan
- Code change is additive
- Single keyword path unchanged
- Can revert without data loss

## Open Questions

1. **Should we limit the maximum number of keywords?**
   - Proposal: Soft limit of 100 keywords with warning
   - Prevents accidental resource exhaustion
   - Can be increased if needed

2. **Should we add a flag to disable parallel processing?**
   - Proposal: Not for initial version
   - Can add `--sequential` flag in future if requested
   - Keeps initial implementation simple

3. **How to handle interactive mode for multiple keywords?**
   - Proposal: Prompt for comma-separated list
   - Example: "Enter keywords (comma-separated): ERROR, WARNING, CRITICAL"
   - Simple UX, matches expected behavior
