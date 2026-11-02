# Tool: Static Analyzer

## Purpose
Runs configurable static analysis checks on changed files and produces a structured report. Aggregates results from linters, type checkers, formatters, and complexity tools. Called by Review Agent.

## Inputs
- List of changed files (from git diff)
- `config/project.yaml` (tools config, thresholds)

## Outputs
- Structured analysis report (JSON + human-readable)
- Pass/fail verdict per check category
- Line-level issue list

## Analysis Pipeline

Run checks in this order, report all results:

```
1. Formatter check     (fastest — fail fast on formatting)
2. Linter              (style and correctness)
3. Type checker        (type safety)
4. Complexity check    (function size, cyclomatic complexity)
5. Security scan       (SAST patterns)
```

## Command Resolution

All commands resolved from `config/project.yaml > commands`:

```yaml
commands:
  lint: npx eslint src/ --ext .ts,.tsx --format json
  typecheck: npx tsc --noEmit
  format: npx prettier --check src/
  test: npx vitest run
  audit: npm audit --audit-level=moderate
```

If a command is not in config, detect from installed tools (see `skills/code-analysis.md`).

## Complexity Checks

### Function Length
Scan all source files for functions exceeding the configured limit:

```bash
# Pattern: count lines between function start and closing brace
# Works for TS/JS, Python, Java, Go, etc.
# Report: file:line:functionName (N lines, limit M)
```

Threshold from `config/project.yaml > codingStandards.maxFunctionLines` (default: 10).

### Cyclomatic Complexity
Number of independent paths through a function:
- 1-4: Simple, good
- 5-8: Moderate, acceptable
- 9+: Complex, refactor recommended
- 15+: Very complex, BLOCKER

```bash
# TypeScript/JavaScript (via ESLint)
npx eslint --rule '{"complexity": ["error", 8]}' [files]

# Python
radon cc [files] -nb  # show only blocks with complexity >= threshold

# Java (PMD)
mvn pmd:check -Dpmd.failOnViolation=true
```

## Structured Output

```json
{
  "timestamp": "2024-01-15T10:30:00Z",
  "project": "my-app",
  "changedFiles": 5,
  "checks": {
    "formatter": {
      "status": "passed",
      "tool": "Prettier",
      "issues": []
    },
    "linter": {
      "status": "failed",
      "tool": "ESLint",
      "issues": [
        {
          "file": "src/auth/auth.service.ts",
          "line": 34,
          "column": 5,
          "severity": "error",
          "rule": "no-unused-vars",
          "message": "'token' is defined but never used",
          "fixable": true
        }
      ]
    },
    "typecheck": {
      "status": "passed",
      "tool": "tsc",
      "issues": []
    },
    "complexity": {
      "status": "warning",
      "issues": [
        {
          "file": "src/orders/order.service.ts",
          "line": 45,
          "function": "processOrder",
          "lines": 23,
          "threshold": 10,
          "severity": "warning"
        }
      ]
    },
    "security": {
      "status": "passed",
      "tool": "ESLint security plugin",
      "issues": []
    }
  },
  "summary": {
    "errors": 1,
    "warnings": 1,
    "info": 0,
    "fixable": 1,
    "verdict": "FAILED"
  }
}
```

## Auto-Fix Mode

When `--fix` flag is passed, apply safe automatic fixes:
```bash
# Format
npx prettier --write [files]

# Lint (fixable rules only)
npx eslint [files] --fix

# Python
black [files]
isort [files]
ruff check [files] --fix
```

Report what was auto-fixed vs what requires manual attention.

## Ignore Patterns

Respect project ignore files:
- `.eslintignore`
- `.prettierignore`
- `.gitignore` (for file discovery)
- Per-file disable comments (`// eslint-disable-next-line rule-name`)

Never suppress issues without a visible comment explaining why.
