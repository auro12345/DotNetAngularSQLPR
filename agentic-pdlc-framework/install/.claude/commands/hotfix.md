Emergency hotfix fast-track for P0/P1 production incidents. Bypasses normal sprint cycle.

Ticket: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery (fast-path only, no full scan).
2. Confirm this is a genuine P0 or P1 incident. If severity is P2 or below, use `/fix $ARGUMENTS` instead.

## ⚠️ Hotfix Protocol

Hotfixes are surgical. No refactoring. No extra changes. Minimum footprint.
Design doc is **skipped** for hotfixes — root cause + fix are recorded in the incident requirements doc instead.

### Step 1: Fetch Incident Details
Use `mcp__jira__get_issue($ARGUMENTS)`. Confirm:
- Severity: P0 (production down) or P1 (critical feature broken)
- Impact: number of users affected, revenue impact
- Error: exact error message or symptom

### Step 2: Create Minimal Incident Requirements Doc
Before touching any code, save `docs/designs/$ARGUMENTS-requirements.md` with:
```markdown
# $ARGUMENTS — Hotfix Requirements
Severity: P0/P1
Impact: [users/services affected]
Symptom: [what users/monitors observe]

## Acceptance Criteria
- [$ARGUMENTS-AC-1] Regression test that reproduced the bug now passes
- [$ARGUMENTS-AC-2] All existing tests continue to pass
- [$ARGUMENTS-AC-3] [Specific behaviour that was broken] is restored

## Root Cause
[To be filled after Step 3]

## Fix
[To be filled after Step 3]
```

### Step 3: Branch from Production Tag
```bash
# Get the latest production tag
git fetch --tags
git describe --tags --abbrev=0  # shows latest tag e.g., v1.2.3

# Branch BEFORE touching any source file
git checkout -b hotfix/$ARGUMENTS-[description] v[X.Y.Z]
```
Print: "Branch hotfix/$ARGUMENTS-... created from v[X.Y.Z]. No source files modified yet."

### Step 4: Identify the Fix
Find the exact code path causing the issue. State:
- File: [path:line]
- Root cause: [one sentence]
- Fix: [specific change needed]

Update the requirements doc `## Root Cause` and `## Fix` fields.
Ask for confirmation: "Root cause at [file:line]: [description]. Fix: [change]. Proceed?"

### Step 5: Write Regression Test FIRST (must fail before fix)
Write a test that **fails with the current code** — this proves the bug exists:
```
Test name: [bugSymptom]_reproduces_$ARGUMENTS
```
Run the test: it MUST FAIL now. Report: "Regression test written and FAILING — bug confirmed."
Do NOT apply the fix until this is confirmed failing.

### Step 6: Apply Fix + Verify
1. Apply MINIMAL fix only — no refactoring, no extra features
2. Run the regression test: it MUST NOW PASS
3. Run full test suite: ALL existing tests must pass
4. Run lint and typecheck: zero errors

Report: "Regression test ✅ passing. All [N] tests passing."

### Step 7: Fast Review
Run a focused security and correctness review of the changed lines only:
- No new injection vectors introduced
- No secrets added
- Change is truly minimal (no scope creep)
- Regression test correctly validates the fix

If any BLOCKER found: fix it before proceeding.

### Step 8: Create Fast-Track PR
PR title: `[HOTFIX][P0] fix([scope]): [description]`

PR body:
```markdown
## 🚨 HOTFIX — Severity: P0/P1

**Incident:** $ARGUMENTS
**Impact:** [users/services affected]
**Root Cause:** [one sentence]

## Change
[file:line] — [description of fix]

## Test
- [x] Regression test added and passing
- [x] Full test suite passing
- [x] Verified fix addresses root cause

## Deployment
- [ ] Deploy to staging → smoke test
- [ ] Deploy to production
- [ ] Tag patch release vX.Y.(Z+1)
- [ ] Merge hotfix back to main/develop
```

Use `mcp__github__create_pull_request` (GitHub) or `mcp__gitlab__create_merge_request` (GitLab) with reviewers from config.
Use `mcp__jira__transition_issue` to move $ARGUMENTS to "In Review" and `mcp__jira__add_comment` with the PR link.

### Step 9: Post-Hotfix Actions
After PR is merged:
```bash
git tag -a v[X.Y.Z+1] -m "Hotfix v[X.Y.Z+1] — $ARGUMENTS"
git push origin v[X.Y.Z+1]
# Merge hotfix branch back to main/develop
```

Create post-mortem note in Jira/Confluence if P0.

### Output
```
🚨 HOTFIX ready for $ARGUMENTS
Severity:  P0/P1
Branch:    hotfix/$ARGUMENTS-[description] (from v[X.Y.Z])
Fix:       [file:line]
PR:        [url]  ← needs fast review approval
Next tag:  v[X.Y.Z+1]
```
