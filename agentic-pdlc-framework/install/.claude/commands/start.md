Run the full PDLC flow for the given ticket from requirements through to a ready-to-merge pull request.

Ticket: $ARGUMENTS

## Precheck

Before anything else:
1. Check if `config/project.yaml` exists in the project root. If missing or stale (older than 7 days), run the Project Discovery scan (same logic as `/scan`) and create/update it first. Print the one-line stack summary.
2. If no ticket was provided in $ARGUMENTS, ask: "Which ticket should I work on? (e.g., PROJ-123)"

## Execution — Full PDLC Flow

Work through each phase in sequence. Complete each phase fully before moving to the next.

### Phase 1 — Requirements
Follow `agentic-pdlc-framework/agents/02-requirements-agent.md`:
- Fetch the full ticket via `mcp__jira__get_issue($ARGUMENTS)` — description, ACs, links, attachments, comments
- Process all external sources (Figma, Confluence, attachments, linked URLs)
- Structure requirements with `$ARGUMENTS-AC-N` IDs and empty Traceability Matrix
- Save to `docs/designs/$ARGUMENTS-requirements.md`

**Clarification gate** (cannot be skipped in standard mode):
- If ambiguities exist: post questions to Jira via `mcp__jira__add_comment`, save clarification state, and **STOP**.
  Print: "⏸ Requirements paused — [N] questions posted to Jira. Re-run `/requirements $ARGUMENTS` after the team responds, then `/start $ARGUMENTS` again."
  Do NOT proceed to Phase 2.
- If NO ambiguities, or `--autonomous` flag is passed: proceed immediately to Phase 2.
- Check `docs/designs/$ARGUMENTS-clarification.md` — if it exists and has pending questions, stop with the same pause message.

Pass `--autonomous` to skip the Jira loop and make documented assumptions instead:
```
/start $ARGUMENTS --autonomous
```

### Phase 2 — Design
Follow `agentic-pdlc-framework/agents/03-architecture-agent.md`:
- Analyze the existing codebase structure and patterns (from config/project.yaml)
- Produce a concise design: approach, file impact map, sequence/component diagrams in Mermaid
- Document API contract if new endpoints are involved
- Save design doc to `docs/designs/$ARGUMENTS-[short-description].md`
- Validate design against `codingStandards.patterns` in project.yaml

### Phase 3 — Branch + Implementation
Follow `agentic-pdlc-framework/agents/04-development-agent.md`:

**Step 3.0 — Create branch (FIRST, before any file changes)**
```bash
git checkout -b feat/$ARGUMENTS-[short-description]
# or fix/$ARGUMENTS-... for bugs
```
Confirm: "Branch feat/$ARGUMENTS-... created and checked out."
Do NOT proceed to skeleton until the branch is confirmed.

**Step 3.1 — Skeleton**: Create all files with class/method signatures and TODOs. No logic yet.

**Step 3.2 — Tests**: Write all tests referencing `$ARGUMENTS-AC-N` IDs. Run them — they must FAIL.
Report: "Tests written: N tests, N failing (expected at skeleton stage)"

**Step 3.3 — Implementation**: Implement method by method. Run tests after each method.
Max lines per function from `config/project.yaml > codingStandards.maxFunctionLines`.

**Step 3.4 — Static analysis**: Run `[commands.lint]`, `[commands.typecheck]`. Fix ALL errors.

**Step 3.5 — Full test run**: Run `[commands.testCoverage]`. All tests must pass, coverage must meet thresholds.

### Phase 4 — Review
Follow `agentic-pdlc-framework/agents/06-review-agent.md`:
- Run full review checklist: correctness, security, performance, quality, tests, AC traceability
- AC traceability check: `grep -r "$ARGUMENTS-AC-" .` — every AC ID must have at least one test hit
- If BLOCKERs found: fix them, then re-review
- Report final verdict before PR

### Phase 5 — Pull Request
Follow `agentic-pdlc-framework/agents/07-deployment-agent.md`:
- Stage specific changed files (never `git add -A`)
- Commit with Conventional Commits format:
  - Subject: `feat(scope): description`
  - Footer: `Implements: $ARGUMENTS`, `Acceptance: $ARGUMENTS-AC-1, $ARGUMENTS-AC-2`, `Tests: file:[AC-ID]`
- Open PR with full template (summary, AC traceability table, test plan, checklist)
- Report PR URL

## Completion

Print:
```
✅ PDLC COMPLETE for $ARGUMENTS
Branch: feat/$ARGUMENTS-...
PR: [URL]
Tests: [N passed] | Coverage: [X%]
```
