Run the bug fix flow for the given ticket.

Ticket: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Fetch the bug ticket using Jira MCP. If $ARGUMENTS is not provided, ask: "Which bug ticket? (e.g., PROJ-456)"
3. Create requirements doc `docs/designs/$ARGUMENTS-requirements.md` from the bug report with `$ARGUMENTS-AC-N` IDs (e.g. AC-1: regression test passes, AC-2: existing tests pass, AC-3: specific behaviour fixed).
4. **Create and check out a fix branch BEFORE any file changes**:
   ```bash
   git checkout -b fix/$ARGUMENTS-[short-description]
   ```
   Print: "Branch fix/$ARGUMENTS-... created and checked out." Do NOT touch any source files before this step.

## Execution

Follow `agentic-pdlc-framework/agents/08-maintenance-agent.md` for triage, then `agentic-pdlc-framework/agents/04-development-agent.md` for the fix.

### Step 1: Triage
Fetch $ARGUMENTS from Jira MCP. Extract:
- What users observe (symptom)
- Environment where it occurs
- Steps to reproduce
- Error messages / stack traces

Classify severity:
- P0 (production down / data loss / security breach) → escalate, suggest `/hotfix $ARGUMENTS`
- P1 (core feature broken, no workaround) → fast-track in current sprint
- P2 (degraded, workaround exists) → standard sprint
- P3 (cosmetic / edge case) → backlog

Print: `Severity: [P0/P1/P2/P3] | [description]`

### Step 2: Reproduce
Identify the exact code path that produces the bug. Find the file and line.

Ask: "I can reproduce the issue in [file:line]. The root cause is [description]. Shall I proceed with the fix?"

### Step 3: Root Cause Analysis
- Find when this was introduced: `git log --all --oneline -- [affected-file] | head -20`
- Is it a regression? State the introducing commit if found.
- Are there similar patterns elsewhere in the codebase that have the same bug?

### Step 4: Write Regression Test First
Write a test that FAILS with the current code and proves the bug exists.
```
Test name: [bugDescription]_[scenario]_[expectation]
Expected: test FAILS on current code
```
Run the test to confirm it fails. Report: "Regression test written and failing (confirms bug exists)"

### Step 5: Apply Fix
- Apply the MINIMAL change that fixes the bug. No refactoring. No extra features.
- Functions must stay within `[codingStandards.maxFunctionLines]` lines.
- Run regression test — it must now PASS.
- Run full test suite — all existing tests must still PASS.

### Step 6: Review and PR
Run: `/review $ARGUMENTS`
If 0 BLOCKERs, create PR:
- Stage specific files (never `git add -A`)
- Commit: `fix(scope): description` with footer `Resolves: $ARGUMENTS`, `Acceptance: $ARGUMENTS-AC-1`, `Tests: [file]:[AC-ID]`
- Push branch and open PR with AC traceability table
- Update Jira ticket status to "In Review"

### Output
```
✅ Bug fix complete for $ARGUMENTS
Severity:  [P0-P3]
Root cause: [one sentence]
Fix:        [file:line] — [change summary]
Regression test: ✅ passes
All tests: ✅ [N passed]
PR: [url]
```
