Perform a thorough automated code review of the current changes before creating a PR.

Scope (optional): $ARGUMENTS
- If empty: review all uncommitted and staged changes (`git diff HEAD`)
- If a ticket ID (e.g., PROJ-123): scope review to files changed for that ticket
- If a file path: review only that file

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load: `commands.lint`, `commands.typecheck`, `commands.test` from config.
3. Get the list of changed files: `git diff --name-only HEAD` (or `git diff --staged --name-only`)

## Execution

Follow `agentic-pdlc-framework/agents/06-review-agent.md` fully.

### Step 1: Run Automated Checks
```bash
[commands.lint]       # Zero tolerance for errors
[commands.typecheck]  # Zero tolerance for type errors
[commands.test]       # All tests must pass
```
Report pass/fail for each.

### Step 1b: AC Traceability Check (runs before code review)

Load the requirements doc for the ticket (`docs/designs/$ARGUMENTS-requirements.md` if a ticket ID was given, else infer from changed files).

For each `[TICKET-ID]-AC-N` found in the requirements doc:
```bash
grep -r "TICKET-ID-AC-N" .   # search all test files
```

Report:
```
AC Traceability Check
━━━━━━━━━━━━━━━━━━━━━━━━━━━
[TICKET-ID]-AC-1  ✅  [file] ([N] tests)
[TICKET-ID]-AC-2  ✅  [file] ([N] tests)
[TICKET-ID]-AC-3  ❌  NOT FOUND → [BLOCKER]
━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

**If any AC is uncovered**: raise a BLOCKER immediately and stop the review. The missing test must be written first.

### Step 2: Review Each Changed File

For each file in scope, check all of the following:

**Correctness**
- Does the implementation match the acceptance criteria?
- Are all edge cases handled?
- No off-by-one errors, no unhandled null/undefined, no incorrect operator precedence
- Async operations properly awaited

**Security**
- User inputs validated/sanitized at entry points
- No secrets or credentials in code
- No SQL string concatenation (parameterized queries only)
- Auth checks on all protected routes
- No sensitive data in logs

**Performance**
- No N+1 database query patterns
- Large data sets paginated (not loaded entirely)
- No synchronous blocking in async contexts

**Code Quality**
- Functions ≤ `[codingStandards.maxFunctionLines]` lines
- Complexity ≤ `[codingStandards.maxCyclomaticComplexity]` branches
- No dead code, debug statements, or commented-out code
- No magic numbers or strings (use named constants)
- Naming is intention-revealing

**Tests**
- New code has tests
- Tests cover error cases and edge cases
- No skipped tests without ticket reference

### Step 3: Produce Review Report

```markdown
# Code Review Report
Scope: $ARGUMENTS (or "all changes")
Files reviewed: [N] (+[X]/-[Y] lines)

## BLOCKERS (fix before PR)
### [BLOCKER-1] [Category]: [title]
File: [path:line]
Issue: [description]
Fix: [exact change needed]

## WARNINGS (should fix)
### [WARN-1] [Category]: [title]
File: [path:line]
Issue: [description]
Suggestion: [improvement]

## SUGGESTIONS (nice to have)
...

## AC Traceability
| AC ID | Summary | Tests | Status |
|-------|---------|-------|--------|
| [TICKET-ID]-AC-1 | [summary] | [file:name] | ✅/❌ |
| [TICKET-ID]-AC-2 | [summary] | [file:name] | ✅/❌ |

AC Coverage: [N]/[total] ([X%])

## Summary
| Check | Status | Issues |
|-------|--------|--------|
| AC Traceability | ✅/❌ | [N untested ACs] |
| Lint | ✅/❌ | [N] |
| Type Check | ✅/❌ | [N] |
| Tests | ✅/❌ | [N passed, N failed] |
| Coverage | ✅/❌ | [X%] |
| Security | ✅/❌ | [N] |
| Quality | ✅/❌ | [N] |

Verdict: ✅ READY FOR PR / ❌ NOT READY ([N] blockers)
```

If READY: `Next step: /pr [ticket-id]`
If NOT READY: List the BLOCKERs to fix, then re-run `/review`
