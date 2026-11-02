Generate a project health dashboard — tests, coverage, build, open PRs, tech debt.

Options: $ARGUMENTS
- Empty: print health dashboard to terminal
- `--confluence`: also publish the report to Confluence
- `--slack`: also post summary to Slack

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load monitoring config from `config/project.yaml > integrations.monitoring`.

## Execution

### Step 1: Test Health
```bash
[commands.testCoverage from config/project.yaml]
```
Record: total tests, pass/fail, coverage per category.

### Step 2: Build Health
```bash
time [commands.build from config/project.yaml] 2>&1
```
Record: success/fail, build time.

### Step 3: Lint Health
```bash
[commands.lint from config/project.yaml]
```
Record: error count, warning count.

### Step 4: Security Health
```bash
[commands.audit from config/project.yaml]
```
Record: critical, high, moderate, low vulnerability counts.

### Step 5: Git Health (via GitHub MCP if configured)
- Open PRs count and oldest PR age
- PRs awaiting review > 2 days
- Failed CI checks on open PRs
- Branches not merged for > 14 days

### Step 6: Tech Debt Snapshot
Scan for:
- `TODO`/`FIXME`/`HACK` comment count (by file)
- Functions exceeding line limit
- Files exceeding line limit

### Dashboard Output
```
╔══════════════════════════════════════════════════════════╗
║               PROJECT HEALTH DASHBOARD                   ║
║               [Project Name] — [Date]                    ║
╠══════════════════════════════════════════════════════════╣
║ TESTS                                                    ║
║   Status:    ✅ Passing  /  ❌ [N] failing               ║
║   Total:     [N] tests ([N] skipped)                     ║
║   Coverage:  Stmts [X%] · Branches [X%] · Fns [X%]      ║
╠══════════════════════════════════════════════════════════╣
║ BUILD                                                    ║
║   Status:    ✅ Clean  /  ❌ Failing                     ║
║   Duration:  [X.Xs]                                      ║
║   Lint:      ✅ [0 errors]  /  ⚠️  [N warnings]          ║
╠══════════════════════════════════════════════════════════╣
║ SECURITY                                                 ║
║   Critical:  [N]  ← fix immediately                     ║
║   High:      [N]  ← fix this sprint                     ║
║   Moderate:  [N]  (routine)                              ║
╠══════════════════════════════════════════════════════════╣
║ GIT HEALTH                                               ║
║   Open PRs:     [N] ([N] stale >7 days)                  ║
║   CI failures:  [N]                                      ║
╠══════════════════════════════════════════════════════════╣
║ TECH DEBT                                                ║
║   TODO/FIXME comments:  [N]                              ║
║   Long functions (>[limit] lines):  [N]                  ║
║   Large files (>[limit] lines):     [N]                  ║
╠══════════════════════════════════════════════════════════╣
║ OVERALL SCORE: [A/B/C/D]                                 ║
║   A = all green · B = warnings only · C = fixable issues ║
╚══════════════════════════════════════════════════════════╝

Recommended actions:
1. [highest priority action]
2. [second action]
```

If `--confluence`: publish to `integrations.confluence.healthDashboardPage`.
If `--slack`: post summary to `integrations.notifications.channel`.
