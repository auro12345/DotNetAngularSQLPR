Run a sprint retrospective — analyze recent PRs, tickets, and review patterns to identify learnings worth encoding into config/learnings.yaml.

Arguments (optional): $ARGUMENTS
- Empty: analyze the last 2 weeks of activity
- `--since YYYY-MM-DD`: analyze since a specific date
- `--pr 42,43,44`: analyze specific PR numbers only
- `--quick`: skip PR analysis, just ask the team what was learned (interactive)

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first. Load it for tech stack context — it informs how patterns are classified (e.g. a Java pattern vs a TypeScript pattern).
2. Load `config/learnings.yaml` to understand what's already captured (avoid duplicates).
3. Load `config/integrations.yaml` for GitHub/GitLab connection details.

## Execution

Follow `agentic-pdlc-framework/agents/10-framework-evolution-agent.md` — the `/retro` flow.

### Step 1: Gather signals

**From source control** (skip if `--quick`):
```
mcp__github__list_pull_requests(state: "closed", per_page: 20)
→ For each merged PR:
  - Read PR description for BLOCKER/WARNING counts
  - Read review comments for recurring themes
  - Note files changed (recurring modules = signal)
  - Note time from open to merge (unusually long = friction signal)
```

**From Jira clarification rounds** (skip if `--quick`):
```
ls docs/designs/*-clarification.md
→ Count rounds per ticket
→ Identify question categories that recurred
```

**From test reports** (skip if `--quick`):
```
→ Look for modules consistently below coverage threshold
→ Look for recurring test failures that were fixed multiple times
```

### Step 2: Identify patterns

For each signal, apply the detection rules from `agentic-pdlc-framework/agents/10-framework-evolution-agent.md`:
- Same BLOCKER in 3+ PRs → `patterns.avoid` (blocker)
- Same review comment in 2+ PRs → `patterns.preferred` or `agentHints.review`
- Same clarification question in 2+ tickets → `agentHints.requirements`
- Coverage gap in same module across sprints → `techDebt`
- Process friction recurring → `process.*`

### Step 3: Interactive mode (`--quick`)

If `--quick` or if signal analysis found nothing significant, ask the team directly:

```
RETROSPECTIVE — Quick Mode
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Answer any that apply (or press Enter to skip):

1. What pattern kept showing up in code reviews that shouldn't?
2. What did the AI agent miss that a human caught?
3. What question kept coming up in Jira tickets that slows things down?
4. What part of the codebase needs more care than the agents know about?
5. Did any production incident reveal something the framework doesn't check for?
6. What would you change about how the AI handles [feature work / bug fixes / reviews]?
```

### Step 4: Present proposals

For all signals found (from analysis or quick mode), present as a numbered list:

```
RETROSPECTIVE FINDINGS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PRs analyzed: [N] | Tickets analyzed: [N] | Period: [date range]

PROPOSED UPDATES TO config/learnings.yaml:

[1] patterns.avoid (severity: blocker)
    "Never call Prisma directly from controllers"
    Seen in: PR #42, PR #45, PR #51
    Effect: Review Agent will flag this as BLOCKER going forward
    → Apply? (y/n/edit)

[2] agentHints.requirements
    "Check linked Confluence page for DB migration details — Jira tickets rarely mention it"
    Seen in: 3 clarification rounds (PROJ-123, PROJ-145, PROJ-167)
    Effect: Requirements Agent will check Confluence links proactively
    → Apply? (y/n/edit)

[3] techDebt (severity: critical)
    "src/notifications/ has 0% test coverage and growing"
    Seen in: 4 consecutive sprint coverage reports
    Effect: Maintenance Agent will surface this in /health reports
    → Apply? (y/n/edit)

[4] process.skipDesignDocForTypes: ["bug"]
    "Design docs for simple bug fixes add friction without value"
    Seen in: 5 bug-fix PRs skipped design doc manually
    Effect: Architecture Agent will skip design doc for bug-type tickets
    → Apply? (y/n/edit)
```

### Step 5: Apply approved entries

For each approved entry:
1. Write to `config/learnings.yaml`
2. Update `lastUpdated` and `updatedBy` at the top of the file

After all entries are applied:

```
✅ Retrospective complete
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
[N] learnings captured in config/learnings.yaml

EFFECTS GOING FORWARD:
  Review Agent: 1 new BLOCKER rule, 2 new hints
  Requirements Agent: 1 new hint
  Maintenance Agent: 1 new tech debt item tracked
  Process: design docs skipped for bug-type tickets

Run /health to see the full active learnings report.
```

### Step 6: Flag framework-level issues (don't auto-apply)

If any finding points to a bug or gap in the core framework files:

```
⚠️  Framework improvement needed (not applied automatically):

  In agentic-pdlc-framework/agents/02-requirements-agent.md, Step 3 should also scan
  Confluence parent page when issue has label "db-change".
  This is a framework gap, not a team preference.

  To fix: edit agentic-pdlc-framework/agents/02-requirements-agent.md Step 3 manually,
  or run: /learn "framework: requirements agent should scan Confluence when ticket has db-change label"
  and I'll propose the exact edit.
```
