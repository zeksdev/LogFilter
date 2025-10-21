# Add Parallel Keyword Processing

## Why
Users need to filter log files for multiple keywords simultaneously, creating separate output files for each keyword. Currently, filtering multiple keywords requires running the application multiple times sequentially, which is time-consuming for large log files.

## What Changes
- Add support for multiple keywords as input (space-separated or comma-separated list)
- Process each keyword in parallel to improve performance
- Generate a separate output file for each keyword with the same naming convention (`filtered_{keyword}_{timestamp}.log`)
- Maintain backward compatibility with single keyword usage
- Add progress reporting when processing multiple keywords in verbose mode

## Impact
- **Affected specs**: log-filtering
- **Affected code**:
  - LogFilter/Program.cs (main filtering logic, argument parsing, parallel processing)
- **Breaking changes**: None (backward compatible - single keyword still works as before)
- **Performance**: Improved throughput when filtering for multiple keywords (parallel processing)
