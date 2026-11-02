# Agents Guide

Complete reference for all PDLC agents — when to use them, how to invoke them, and what they produce.

## Agent Overview

The PDLC framework uses 10 specialized agents. They can run sequentially (full PDLC flow) or individually (on-demand).

```
Session Start
     │
     ▼
[01] Project Discovery Agent  ← ALWAYS runs first
     │
     ▼ (for new work)
[02] Requirements Agent  →  [03] Architecture Agent
                                      │
                        ┌─────────────┼─────────────┐
                        ▼             ▼             ▼
               [04] Development  [05] Testing  [06] Review
                        └─────────────┴─────────────┘
                                      │
                                      ▼
                          [07] Deployment Agent
                                 │          │
                                 ▼          ▼
                     [08] Maintenance  [09] Confluence Docs
                                           (release notes, doc sync)

[10] Framework Evolution Agent  ← runs any time (/learn, /retro)
     Encodes team learnings into config/learnings.yaml
     Makes all other agents smarter over time
```

---

## 01 — Project Discovery Agent

**File:** [agents/01-project-discovery-agent.md](../agents/01-project-discovery-agent.md)

**When it runs:** Automatically at session start. Manually when you say "scan project" or "update config."

**What it does:**
- Scans file system for tech stack signals (package.json, build files, config files)
- Detects: language, framework, test runner, linter, CI/CD, deployment target
- Creates or updates `config/project.yaml`
- Prints a discovery summary before any other work begins

**Invoke manually:**
```
"Scan this project and update the config"
"Re-discover the tech stack"
"What tech stack does this project use?"
```

**Outputs:**
- Updated `config/project.yaml`
- Discovery summary in terminal

---

## 02 — Requirements Agent

**File:** [agents/02-requirements-agent.md](../agents/02-requirements-agent.md)

**When to use:** At the start of every new task, feature, or bug fix.

**What it does:**
- Fetches ticket from Jira/GitHub Issues using the ticket ID
- Reads linked tickets, attachments, and documentation
- Restates the requirement in its own words
- Identifies ambiguities and asks clarification questions (all at once)
- Produces structured requirement doc with acceptance criteria

**Invoke:**
```
"Start work on PROJ-123"
"Read the requirements for PROJ-123 and tell me what needs to be built"
"Analyze this requirement: [paste description]"
```

**Outputs:**
- Structured requirement document (in chat context)
- List of clarification questions
- Acceptance criteria checklist

---

## 03 — Architecture & Design Agent

**File:** [agents/03-architecture-agent.md](../agents/03-architecture-agent.md)

**When to use:** After requirements are validated, before writing any code.

**What it does:**
- Analyzes existing codebase for patterns and integration points
- Designs a solution approach with alternatives considered
- Produces file impact map (create/modify/delete)
- Creates Mermaid diagrams (sequence, component, flowchart)
- Documents API contracts if new endpoints
- Validates design against project patterns in `config/project.yaml`

**Invoke:**
```
"Design the solution for PROJ-123"
"How should we implement [feature]?"
"Create a design document for the authentication flow"
```

**Outputs:**
- Design document saved to `docs/designs/TICKET-ID-description.md`
- Mermaid diagrams
- API contract specification
- Implementation checklist

---

## 04 — Development Agent

**File:** [agents/04-development-agent.md](../agents/04-development-agent.md)

**When to use:** After design is approved.

**What it does:**
1. Creates skeleton (classes + method signatures + TODOs) — no implementation yet
2. Writes all tests (they fail at this point — that's expected)
3. Implements each method (max 10 lines per function)
4. Runs static analysis (lint, typecheck)
5. Confirms all tests pass
6. Updates documentation if needed

**Invoke:**
```
"Implement PROJ-123 using the design in docs/designs/PROJ-123-auth.md"
"Build the skeleton for the auth service"
"Implement the login method in AuthService"
```

**Outputs:**
- New/modified source files
- Test files
- Static analysis report

**Constraint:** Will not implement without a design document or explicit requirements.

---

## 05 — Testing Agent

**File:** [agents/05-testing-agent.md](../agents/05-testing-agent.md)

**When to use:** During development (write tests first), after development (validate coverage).

**What it does:**
- Maps acceptance criteria to test cases
- Writes unit tests (AAA pattern, descriptive names)
- Writes integration tests for API and service layers
- Runs tests and parses results
- Analyzes failures and identifies code bugs vs test bugs
- Reports coverage against thresholds from `config/project.yaml`

**Invoke:**
```
"Write tests for the AuthService class"
"Run the tests and explain any failures"
"Check test coverage for src/auth/"
"Why is this test failing: [paste test name]"
```

**Outputs:**
- Test files
- Test execution report (pass/fail/coverage)
- Failure analysis (if failures exist)

---

## 06 — Code Review Agent

**File:** [agents/06-review-agent.md](../agents/06-review-agent.md)

**When to use:** After development is complete, before creating a PR.

**What it does:**
- Reviews changed files against requirements (correctness)
- Security review (injection, auth, secrets, CVEs)
- Performance review (N+1, pagination, caching)
- Code quality review (function size, complexity, naming, duplication)
- Test coverage review
- Produces structured report with BLOCKER / WARNING / SUGGESTION items

**Invoke:**
```
"Review all my changes before I create a PR"
"Check the security of the auth changes"
"Is this code ready for PR?"
```

**Outputs:**
- Structured review report (BLOCKER/WARNING/SUGGESTION)
- Go/No-Go recommendation
- Specific file:line references for all issues

**Gate:** Development Agent waits for BLOCKER-free review before creating PR.

---

## 07 — Deployment Agent

**File:** [agents/07-deployment-agent.md](../agents/07-deployment-agent.md)

**When to use:** After Review Agent approves with no BLOCKERs.

**What it does:**
- Creates branch with correct naming pattern
- Stages specific files (never `git add -A`)
- Writes Conventional Commit message with ticket traceability
- Opens PR with full template (summary, changes, test plan, checklist)
- Monitors CI pipeline
- Manages staging and production deployment after merge
- Creates release tags if applicable

**Invoke:**
```
"Create a PR for PROJ-123"
"Commit and push these changes"
"Deploy to staging"
"Create the release tag v1.2.3"
```

**Outputs:**
- Git commit
- Pull Request (with URL)
- Deployment confirmation
- Release tag

---

## 08 — Maintenance Agent

**File:** [agents/08-maintenance-agent.md](../agents/08-maintenance-agent.md)

**When to use:** For bug fixes, dependency updates, security patches, and periodic health checks.

**What it does:**
- Triages bug reports (P0–P3 severity classification)
- Performs root cause analysis
- Manages hotfix fast-track path for P0/P1
- Runs security dependency audit
- Groups dependency updates by semver safety
- Maintains tech debt register
- Produces weekly/monthly health reports

**Invoke:**
```
"Triage this bug: [paste report]"
"Run the weekly maintenance check"
"Update all patch-level dependencies"
"What's our tech debt status?"
"Generate the monthly health report"
```

**Outputs:**
- Triage report with severity classification
- Dependency audit report
- Tech debt register update
- Health reports
- Update PRs (for dependency updates)

---

## 09 — Confluence Documentation Agent

**File:** [agents/09-confluence-docs-agent.md](../agents/09-confluence-docs-agent.md)

**When to use:** After a PR is merged, when design docs are created, for periodic documentation audits.

**What it does:**
- Generates plain-English release notes from PR descriptions and Jira tickets
- Publishes release notes as Confluence pages under the configured parent
- Syncs design documents (`docs/designs/*.md`) to Confluence
- Syncs API documentation (OpenAPI spec or `docs/API.md`) to Confluence
- Publishes Architecture Decision Records (ADRs) to Confluence
- Updates `CHANGELOG.md` following Keep a Changelog format
- Posts Slack deployment notifications

**Invoke (slash commands):**
```
/release-notes v1.2.3          → full version release notes
/release-notes PROJ-123        → release notes for one feature
/docs-sync PROJ-123            → sync design + API docs to Confluence
/docs-sync all                 → sync all stale local docs
```

**Invoke (natural language):**
```
"Publish release notes for v1.2.3 to Confluence"
"Update the Confluence design docs for PROJ-123"
"Generate release notes and post to #deployments Slack channel"
```

**Outputs:**
- Confluence pages (created or updated) with URLs
- Updated `CHANGELOG.md`
- Jira tickets transitioned to "Done" / "Released"
- Slack notification (if configured)
- Release tag comment on GitHub PR

**Requirements:**
- `integrations.confluence.*` fields set in `config/project.yaml`
- `CONFLUENCE_API_TOKEN`, `CONFLUENCE_BASE_URL`, `CONFLUENCE_USER_EMAIL` env vars
- Confluence MCP configured in `.claude/settings.json`

---

## Running the Full PDLC Flow

For a complete feature from ticket to deployment:

```
1. "Start work on PROJ-123"
   → Discovery Agent loads context
   → Requirements Agent reads ticket

2. "Design the solution"
   → Architecture Agent creates design doc

3. "Implement it"
   → Development Agent: skeleton → tests → implementation
   → Testing Agent: validates coverage

4. "Review and create PR"
   → Review Agent: checks for BLOCKERs
   → Deployment Agent: creates PR

5. [After PR merges]
6. "Deploy to production"
   → Deployment Agent: coordinates CI/CD
```

---

## 10 — Framework Evolution Agent

**File:** [agents/10-framework-evolution-agent.md](../agents/10-framework-evolution-agent.md)

**When it runs:** Any time — after sprints, after incidents, after a review discussion reveals a recurring pattern.

**What it does:**
- Captures a single learning from free text: `/learn "description"`
- Runs a sprint retrospective analysis: `/retro`
- Classifies each learning (code pattern / review rule / agent hint / process change / tech debt / postmortem)
- Writes structured entries to `config/learnings.yaml`
- All other agents read `learnings.yaml` and apply team-specific rules automatically

**Invoke manually:**
```
/learn "Don't call Prisma directly from controllers — always go through the service layer"
/learn "After the JWT rotation incident: cross-service impact check required before auth deploys"
/retro
/retro --since 2025-01-01
```

**Outputs:**
- Updated `config/learnings.yaml` with new entries
- Summary of which agents are affected and how their behaviour changes

---

## Customizing Agent Behavior

All agent behavior is driven by three config files loaded in this order:

1. **`config/project.yaml`** — tech stack, commands, coding standards (auto-detected by `/scan`)
2. **`config/learnings.yaml`** — team-specific knowledge: patterns, hints, overrides, postmortems (managed by `/learn` and `/retro`)
3. **`config/integrations.yaml`** — external tool connections (Jira, GitHub, Figma, Confluence, Slack)

And two rule layers:
4. **`.cursor/rules/*.mdc`** — behavioral rules (auto-loaded in Cursor IDE)
5. **Agent definition files** — the playbooks in `agentic-pdlc-framework/agents/`

**To change a behaviour:**
| What you want to change | Where to change it |
|------------------------|-------------------|
| Code quality threshold (e.g., max function lines) | `config/project.yaml > codingStandards` or `config/learnings.yaml > reviewOverrides` |
| Block a specific code pattern in all PRs | `config/learnings.yaml > patterns.avoid` (severity: blocker) |
| Give an agent project-specific context | `config/learnings.yaml > agentHints.[agentName]` |
| Skip a PDLC step for certain ticket types | `config/learnings.yaml > process` |
| Change workflow for all projects | Edit `.cursor/rules/*.mdc` |
| Change agent logic | Edit `agentic-pdlc-framework/agents/*.md` |

**Quickest path:** Use `/learn "what you want to change"` — the Framework Evolution Agent classifies and routes it automatically.
