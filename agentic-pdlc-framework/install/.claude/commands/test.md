Run the project's test suite, report results and coverage.

Arguments (optional): $ARGUMENTS
- If empty: run all tests
- If a path or pattern: run matching tests only (e.g., `src/auth/`, `AuthService`, `PROJ-123`)
- If `--watch`: run in watch mode

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load `commands.test` and `commands.testCoverage` from config.
3. Load `testing.coverageThresholds` from `config/project.yaml` — these are the pass/fail thresholds used in the report (statements, branches, functions, lines). Defaults: statements 80%, branches 75%, functions 85%, lines 80%.

## Execution

Follow `agentic-pdlc-framework/agents/05-testing-agent.md`.

### Step 1: Determine Scope
- If $ARGUMENTS is a file path: run tests for that specific path
- If $ARGUMENTS is a ticket ID: run tests in files related to that ticket's changes (`git diff --name-only HEAD`)
- If $ARGUMENTS is empty: run full suite with coverage

### Step 2: Run Tests
```bash
# Full suite with coverage (default)
[commands.testCoverage from config/project.yaml]

# Scoped run
[commands.test] [scope-filter]
```

### Step 3: Report Results

**Pass case:**
```
✅ Tests passed
━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Total:    [N] tests
Passed:   [N]
Failed:   0
Skipped:  [N] (list any with justification)
Duration: [X.Xs]

Coverage:
  Statements: [X%] (threshold: [Y%]) [✅/❌]
  Branches:   [X%] (threshold: [Y%]) [✅/❌]
  Functions:  [X%] (threshold: [Y%]) [✅/❌]
  Lines:      [X%] (threshold: [Y%]) [✅/❌]

Uncovered lines (if any):
  [file:lines] — [suggested test to add]

AC Traceability:
  [TICKET-ID]-AC-1  ✅  [N] tests passing  ([file:test-name])
  [TICKET-ID]-AC-2  ✅  [N] tests passing  ([file:test-name])
  [TICKET-ID]-AC-3  ❌  NO TEST — write before PR
```

**Fail case:**
For each failure, provide a diagnosis:
```
❌ [N] tests failed

Failure 1: [test name]
  File: [path:line]
  Error: [exact error message]
  Root cause: [code bug or test bug?]
  Fix: [specific fix needed in file:line]
```

### Step 4: Action Items
- If coverage below threshold: suggest specific tests to add for uncovered lines
- If tests failed due to code bugs: instruct how to fix (file + line + change needed)
- If tests failed due to test bugs: fix the tests, explain why
