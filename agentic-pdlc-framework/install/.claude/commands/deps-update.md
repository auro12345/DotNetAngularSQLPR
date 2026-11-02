Analyze and apply dependency updates, creating appropriately scoped PRs.

Scope: $ARGUMENTS
- Empty: full analysis + interactive update
- `--patch`: apply patch updates only (automated, no changelog review)
- `--minor`: apply minor updates (with changelog review)
- `--security`: apply only security-critical fixes
- `--dry-run`: show what would be updated without changing anything

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load `maintenance.dependencyUpdateSchedule`, `maintenance.licensing` from config.
3. Load `commands.install`, `commands.test`, `commands.audit` from config.

## Execution

Follow `agentic-pdlc-framework/agents/08-maintenance-agent.md > Dependency Management`.

### Step 1: Full Audit
```bash
[commands.audit]  # security check first
```

Show security findings. If critical CVEs: handle them before routine updates.

### Step 2: List Outdated Packages
```bash
# JS/TS: npm outdated --json
# Python: pip list --outdated --format=json
# Java: mvn versions:display-dependency-updates
# Go: go list -m -u all
```

Organize by update type:
```
PATCH UPDATES (safe to batch):
  [package]: [current] → [latest]
  ...

MINOR UPDATES (review changelogs):
  [package]: [current] → [latest]
  ...

MAJOR UPDATES (individual assessment):
  [package]: [current] → [latest] — requires migration review
  ...
```

### Step 3: License Check (before installing anything)
For any new package versions, verify licenses remain within `maintenance.licensing.allowed` list.
Block anything in `maintenance.licensing.blocked`.

### Step 4: Apply Updates

**Patch updates (if --patch or full run):**
```bash
# Update all packages within current semver ranges
[appropriate command for the stack]
[commands.test]  # must pass
```
If tests pass → create PR:
- Branch: `chore/deps-patch-[YYYY-MM-DD]`
- Commit: `chore(deps): apply patch dependency updates [YYYY-MM-DD]`

**Minor updates (if --minor, with confirmation):**
For each minor update, read the package changelog, summarize breaking changes, then apply and test.
Create one PR per ecosystem (e.g., all React-related, all testing-related).

**Major updates:**
Report only — do NOT apply automatically. Create Jira tickets for each:
```
⚠️  Major updates deferred (require manual assessment):
  - [package] v[X] → v[Y]: create ticket for migration
```

### Step 5: Post-Update Report
```
✅ Dependency update complete

Applied:
  Patch: [N] packages updated
  Minor: [N] packages updated

Deferred (create tickets):
  Major: [N] packages — see list above

PRs created:
  [branch]: [URL]

Tests: ✅ all passing after updates
Security: ✅ [N] critical/high CVEs resolved
```
