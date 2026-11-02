# PDLC Framework — Claude Code Context

This file is automatically loaded by Claude Code. It provides the full context for the PDLC (Product Development Life Cycle) AI framework.

## What This Framework Is

An AI-agent system that automates the complete software development lifecycle — requirements through production maintenance — for any tech stack. Agents are defined in `agentic-pdlc-framework/agents/`, behavior rules in `.cursor/rules/`, project metadata in `config/project.yaml`, and all external tool connections in `config/integrations.yaml`.

## Mandatory First Step: Project Discovery + Learnings

**Before every task**, check whether `config/project.yaml` exists in the target project:
- **If missing**: Run the Project Discovery Agent (see `agentic-pdlc-framework/agents/01-project-discovery-agent.md`) to scan the project and create it.
- **If present but older than 7 days**: Offer to re-scan.
- **If present and fresh**: Load it silently and proceed.

**Also load `config/learnings.yaml`** if it exists:
- Apply `agentHints.*` entries as additional context for the relevant phase
- Apply `patterns.avoid` (blocker) and `patterns.preferred` to all code generation and review
- Apply `reviewOverrides` in place of `config/project.yaml` thresholds where specified
- Apply `process.*` to adjust workflow behavior (skip steps, auto-modes, path exclusions)

Print a one-line summary after loading: `Project: [name] | Stack: [lang]/[framework] | Tests: [runner] | CI: [ciCd] | Learnings: [N entries]`

## Agent Roster

| File | Phase | Invoked When |
|------|-------|-------------|
| `agentic-pdlc-framework/agents/01-project-discovery-agent.md` | Always | Session start, `/scan` |
| `agentic-pdlc-framework/agents/02-requirements-agent.md` | Phase 1 | `/start`, `/requirements`, `/fix` |
| `agentic-pdlc-framework/agents/03-architecture-agent.md` | Phase 2 | `/design` |
| `agentic-pdlc-framework/agents/04-development-agent.md` | Phase 3 | `/implement` |
| `agentic-pdlc-framework/agents/05-testing-agent.md` | Phase 3 | `/test` |
| `agentic-pdlc-framework/agents/06-review-agent.md` | Phase 3 | `/review` |
| `agentic-pdlc-framework/agents/07-deployment-agent.md` | Phase 4–5 | `/pr`, `/hotfix` |
| `agentic-pdlc-framework/agents/08-maintenance-agent.md` | Phase 6 | `/maintenance`, `/deps-update`, `/health` |
| `agentic-pdlc-framework/agents/09-confluence-docs-agent.md` | Phase 4–5 | `/release-notes`, `/docs-sync` |
| `agentic-pdlc-framework/agents/10-framework-evolution-agent.md` | Continuous | `/learn`, `/retro` |

## Available Slash Commands

Run these from the Claude Code terminal with `/command [args]`:

| Command | What it does |
|---------|-------------|
| `/scan` | Scan project, create/update config/project.yaml |
| `/setup [--validate]` | Configure and validate all external tool integrations |
| `/start PROJ-123` | Full PDLC flow: requirements → design → implement → review → PR |
| `/requirements PROJ-123` | Requirements phase only |
| `/design PROJ-123` | Design phase only (requires requirements to be done) |
| `/implement PROJ-123` | Implementation phase only |
| `/test` | Run tests and report coverage |
| `/review` | Self-review all changes, produce BLOCKER/WARNING report |
| `/pr PROJ-123` | Commit + PR creation |
| `/fix PROJ-123` | Bug fix flow (triage → regression test → fix → PR) |
| `/hotfix PROJ-123` | Emergency P0/P1 hotfix fast-track |
| `/release-notes v1.2.3` | Generate and push release notes to Confluence |
| `/docs-sync PROJ-123` | Sync PR docs to Confluence |
| `/maintenance` | Weekly maintenance: security audit, deps, health |
| `/deps-update` | Dependency update analysis and batch PRs |
| `/health` | Project health dashboard |
| `/learn "description"` | Capture a team learning into config/learnings.yaml |
| `/retro` | Sprint retrospective — analyze patterns and propose learnings |

## Pipeline Gates — Mandatory Sequence for Ticket Work

When any request involves a ticket ID (pattern `[A-Z]+-\d+`), these gates apply in strict order. **You cannot skip a gate or proceed to the next phase without the prior gate's artifact existing on disk.**

| Gate | Artifact Required | Blocks |
|------|------------------|--------|
| 1. Discovery | `config/project.yaml` loaded | Everything |
| 2. Requirements | `docs/designs/TICKET-requirements.md` with `TICKET-AC-N` IDs | Design, Branch, Code |
| 3. Design | `docs/designs/TICKET-design.md` | Branch creation, Code |
| 4. **Branch** | `feat/TICKET-*` or `fix/TICKET-*` branch checked out | Any file modification |
| 5. Skeleton | All classes/signatures created, no logic | Writing tests |
| 6. Tests | Tests written and FAILING | Implementation |
| 7. Implementation | All tests passing, lint clean | Review |
| 8. Review | 0 BLOCKERs, all AC IDs have test coverage | PR |
| 9. PR | Branch pushed, PR opened | Done |

**Branch creation (Gate 4) happens BEFORE the first file is created or modified — not at PR time.**

If a request tries to skip phases (e.g. "just implement it", "skip requirements"), do NOT comply. Run the missing phases first and explain why.

## Key Rules (apply to all interactions)

1. **Pipeline first** — Any ticket-based work request triggers the full pipeline from Gate 1. No exceptions without `--autonomous`.
2. **Branch before code** — Create `feat/TICKET-description` or `fix/TICKET-description` branch before touching any source file.
3. **Skeleton before implementation** — Create structure (classes, signatures, TODOs) before writing any logic.
4. **Tests before code** — Write failing tests against the skeleton, then make them pass.
5. **Max 10 lines per function** — Override with `config/project.yaml > codingStandards.maxFunctionLines`.
6. **Every commit traces to a ticket** — Use Conventional Commits with `Implements: TICKET-ID` footer.
7. **No PR without passing review** — Review Agent must produce 0 BLOCKERs first.
8. **Security**: No secrets in code, SQL parameterized, inputs validated at boundaries.
9. **Config-driven**: Read `config/project.yaml` for thresholds, commands, and patterns. Read `config/integrations.yaml` for all external tool connections (Jira, GitHub, Figma, Confluence, Slack). Read `config/learnings.yaml` for team-specific overrides and accumulated knowledge.
10. **Continuously improving**: After every sprint, run `/retro` to encode learnings. After any production incident, run `/learn` to capture it. The framework gets smarter with every cycle.

## Coding Standards Quick Reference

Loaded from `config/project.yaml` — these are the defaults:
- Max function lines: 10
- Max cyclomatic complexity: 8
- Max file lines: 300
- Coverage thresholds: statements 80%, branches 75%, functions 85%
- Commit format: Conventional Commits (`feat/fix/chore/docs/refactor/perf/test`)
- Branch naming: `feat/TICKET-ID-description`, `fix/TICKET-ID-description`

## MCP Integrations

MCPs are configured in `.claude/settings.json`. Available when keys are set in environment.
Non-secret settings (URLs, project keys) live in `config/integrations.yaml`. Credentials (tokens) live in `.env`.

| MCP | Tool Prefix | Provides |
|-----|-------------|---------|
| Jira | `mcp__jira__*` | Read/update tickets, search issues, transitions |
| GitHub | `mcp__github__*` | PRs, issues, commits, branches, reviews |
| GitLab | `mcp__gitlab__*` | MRs, issues, pipelines (GitLab projects) |
| Confluence | `mcp__confluence__*` | Read/create/update pages and spaces |
| Figma | `mcp__figma__*` | Design spec extraction — frames, components, text, tokens |
| Slack | `mcp__slack__*` | Post deployment/release notifications |

## Project Config Location

```
[your-project-root]/
└── config/
    ├── project.yaml          ← tech stack, commands, coding standards (auto-created by /scan)
    ├── integrations.yaml     ← ALL external tool connections: Jira, GitHub, Figma, Confluence, Slack
    ├── learnings.yaml        ← team knowledge: patterns, hints, overrides, postmortems (/learn, /retro)
└── agentic-pdlc-framework/config/project.template.yaml ← schema reference
.env                          ← credentials/tokens (never commit)
.env.example                  ← template for all supported env vars
```

See `agentic-pdlc-framework/config/project.template.yaml` for the project.yaml schema.
See `config/integrations.yaml` for the integrations schema (edit this to enable/configure tools).
See `config/learnings.yaml` to add/review team-specific rules (or use `/learn` and `/retro`).
See `agentic-pdlc-framework/docs/configuration.md` for field-by-field reference.
See `agentic-pdlc-framework/config/examples/` for React, TypeScript, Express, and other stacks.

## Documentation

- `agentic-pdlc-framework/docs/README.md` — Framework overview
- `agentic-pdlc-framework/docs/getting-started.md` — Setup guide
- `agentic-pdlc-framework/docs/agents-guide.md` — Detailed agent reference
- `agentic-pdlc-framework/docs/configuration.md` — project.yaml reference
- `agentic-pdlc-framework/docs/workflows.md` — Step-by-step scenario walkthroughs
- `agentic-pdlc-framework/docs/tech-stack-guide.md` — Language-specific conventions
- `agentic-pdlc-framework/mcp/README.md` — MCP setup and configuration guide
