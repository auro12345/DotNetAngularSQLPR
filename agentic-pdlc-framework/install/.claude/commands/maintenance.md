Run the weekly maintenance check: security audit, dependency review, health report.

Options: $ARGUMENTS
- Empty: run full weekly check
- `--security`: security audit only
- `--deps`: dependency check only
- `--health`: health metrics only
- `--report`: generate and push report to Confluence

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load `commands.audit`, `maintenance.securityAuditSchedule`, `maintenance.dependencyUpdateSchedule` from config.
3. Load `config/learnings.yaml`: read `techDebt` entries and `postmortems` — surface these in the health report alongside automated findings.

## Execution

Follow `agentic-pdlc-framework/agents/08-maintenance-agent.md`.

### Module 1: Security Audit

Run the project's audit tool:
```bash
[commands.audit from config/project.yaml]
```

Parse results:
- Critical CVEs → immediate action items
- High CVEs → sprint-level action items
- Moderate/Low → batch update items

For each critical/high vulnerability:
- Package name and version
- CVE identifier
- CVSS score
- Fixed-in version (if available)
- Suggested action: upgrade / workaround / no-fix-available

### Module 2: Dependency Check

```bash
# Check for outdated packages
[appropriate command for the tech stack]
# npm outdated | pip list --outdated | mvn versions:display-dependency-updates
```

Categorize:
- Patch updates (X.Y.Z → X.Y.Z+1): safe to batch
- Minor updates (X.Y.Z → X.Y+1.0): check changelogs, group
- Major updates (X.Y.Z → X+1.0.0): individual assessment needed

### Module 3: Health Metrics

Check:
- Test pass rate: run `[commands.test]` and report
- Build time: time the build command
- Any `TODO`/`FIXME`/`HACK` comments added since last maintenance: `git log --since="7 days ago" --all -S "TODO\|FIXME\|HACK" --oneline`
- Open PRs older than 7 days (via GitHub MCP if configured)
- Failing CI builds (via GitHub MCP if configured)

### Module 4: Tech Debt Scan (if `--health` or full run)

Scan for:
- Functions exceeding `codingStandards.maxFunctionLines` limit
- Files exceeding `codingStandards.maxFileLines` limit
- `// TODO`, `// FIXME`, `// HACK` without ticket references

Report top 10 by severity.

### Report Format
```
Weekly Maintenance Report — [date]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SECURITY
  Critical:  [N] packages  ← immediate action
  High:      [N] packages  ← sprint action
  Moderate:  [N] packages  ← routine update

DEPENDENCIES  
  Patch updates available:  [N]
  Minor updates available:  [N]
  Major updates available:  [N] (requires assessment)

HEALTH
  Tests:     [N] passed, [N] failed
  Coverage:  [X%] ([+/-N%] vs last week)
  Open PRs:  [N] ([N] stale >7 days)

TECH DEBT
  New items this week:  [N]
  Total open items:     [N]

ACTION ITEMS
1. [URGENT] [action]
2. [SPRINT] [action]
3. [ROUTINE] Run: /deps-update
```

If `--report`: publish this to Confluence maintenance log page and post summary to Slack (if configured).
