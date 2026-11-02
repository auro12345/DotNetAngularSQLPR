Run the Implementation phase for the given ticket.

Ticket: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Look for a requirements document at `docs/designs/$ARGUMENTS-requirements.md`. If missing, STOP: "No requirements doc found for $ARGUMENTS. Run `/requirements $ARGUMENTS` first."
3. Look for a design document at `docs/designs/$ARGUMENTS*.md` (excluding `-requirements.md`). If none exists, STOP: "No design doc found for $ARGUMENTS. Run `/design $ARGUMENTS` first, or provide the design context now."
4. **Check current git branch**: run `git branch --show-current`. If it is NOT a branch containing `$ARGUMENTS`, create and switch to the feature branch now:
   ```bash
   git checkout -b feat/$ARGUMENTS-[short-description-from-design-doc]
   ```
   Print: "Branch feat/$ARGUMENTS-... created. No source files were modified before this point."
5. Load commands from `config/project.yaml`: `commands.test`, `commands.lint`, `commands.typecheck`, `commands.format`.

## Execution

Follow `agentic-pdlc-framework/agents/04-development-agent.md` strictly. Do NOT skip phases.

### Phase A — Skeleton (no implementation yet)
For each file in the design:
1. Create the file with all classes defined (with docstrings)
2. Define all method signatures with full typed parameters and return types
3. Add `// TODO: implement` in every method body (do not write any logic)
4. Verify single responsibility per class and per function

Do not proceed to Phase B until skeleton is complete.

### Phase B — Tests (before implementation)
Write all tests now. They MUST FAIL at this point — that is expected.
- One test file per source file
- Cover every Acceptance Criteria with at least one test case
- Cover: happy path, invalid input, error cases, edge/boundary values
- Follow AAA (Arrange-Act-Assert) pattern
- Use project's test framework (from `config/project.yaml > tech.testFramework`)

Run tests now to confirm they fail: `[commands.test from project.yaml]`
Report: "Tests written: N tests, N failing (expected)"

### Phase C — Implementation
Implement each method one at a time:
1. Replace the TODO comment with logic
2. Max lines per function: `[codingStandards.maxFunctionLines from project.yaml]` (default: 10)
3. If a function would exceed the limit, extract sub-functions first
4. Run tests after each method: `[commands.test]`
5. Refactor immediately if quality issues appear

### Phase D — Static Analysis
Run all configured analysis tools:
```
[commands.lint from project.yaml]
[commands.typecheck from project.yaml]  
[commands.format from project.yaml] --check
```
Fix ALL errors and warnings before continuing. Zero tolerance for:
- Unused variables/imports
- Type errors (`any` without justification)
- Linting rule violations

### Phase E — Full Test Run
```
[commands.testCoverage from project.yaml]
```
Required before proceeding:
- All tests pass (0 failures)
- Coverage meets thresholds from `config/project.yaml > testing.coverageThresholds`
- No console.error or unhandled promise rejections

### Phase F — Documentation
Update if applicable:
- API docs / OpenAPI spec (if new endpoints)
- `.env.example` (if new env vars)
- `README.md` (if setup changed)

### Output
```
✅ Implementation complete for $ARGUMENTS
Files created: [list]
Tests: [N passed, 0 failed] | Coverage: [X%]
Lint: ✅ | Type check: ✅
Next step: /review
```
