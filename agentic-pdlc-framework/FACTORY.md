---
name: PDLC Framework
description: "Spec-driven PDLC workflow: requirements, architecture, development, testing, review, PR/deployment, and maintenance."
owner: "IHTC-BL01"
authors:
  - "Nitishchandra Patel <Nitishchandra_Patel@epam.com>"
  - "Swetlina Satpathy <Swetlina_Satpathy@epam.com>"
  - "Mayank Jain <mayank_jain@epam.com>"
  - "Gautam T <gautam_mohandas@epam.com>"
  - "Rohit Gautam <Rohit_Gautam@epam.com>"
  - "Mantasha Qamar <mantasha_qamar@epam.com>"
sdlc_phase: Accelerated Feature Development
support_level: Self-Serve
project_deployments:
  - IHTC-BL01
use_cases:
  - Full feature development from ticket to PR
  - Bug fix with regression-first flow
  - Emergency hotfix
  - Dependency security patch
  - Monthly dependency update
  - Project onboarding
  - Performance investigation
  - End-of-sprint retrospective
  - Postmortem capture into learnings
---

# PDLC Framework

An AI-agent system that automates the complete Product Development Life Cycle — from reading requirements through production maintenance — for any tech stack and any IDE.

This factory packages the existing playbooks in this repository. It does not add new agents or skills. After the install files are copied into a product repository (see [Setup](#setup-in-a-product-repository)), agents are invoked by slash commands or natural language and follow the gated sequence below.

```
Requirements → Analysis → Development → Testing → Review → PR → Deploy → Maintain
     ↑                                                                        │
     └──────────────────────── Feedback Loop ─────────────────────────────────┘
```

Source for this flow: `README.md`, `docs/README.md`, `docs/workflows.md`.

## Cross-role, cross-phase stitch

Each step hands artifacts to the next. Later steps must not start until the documented gate artifacts exist (`install/.cursor/rules/09-pipeline-enforcement.mdc`, `install/.claude/CLAUDE.md`, `install/.claude/commands/start.md`).

| Order | Role (from this repo) | Agent playbook | Primary skills | Gate / output |
|------|------------------------|----------------|----------------|---------------|
| Always first | Project context | `agents/01-project-discovery-agent.md` | `skills/project-scanning.md` (uses `tools/tech-stack-detector.md`) | `config/project.yaml` loaded |
| Phase 1 | Requirements | `agents/02-requirements-agent.md` | `skills/jira-integration.md`, `skills/figma-integration.md`, `skills/external-sources.md`, `skills/git-operations.md` | `docs/designs/TICKET-requirements.md` with `TICKET-AC-N` IDs |
| Phase 2 | Architecture | `agents/03-architecture-agent.md` | (codebase + `config/project.yaml` patterns) | Design doc under `docs/designs/` |
| Phase 3 | Development | `agents/04-development-agent.md` | `skills/git-operations.md`, `skills/code-analysis.md`, `skills/test-execution.md` | Branch `feat/TICKET-*` or `fix/TICKET-*`, then skeleton, then tests, then implementation |
| Phase 3 | Testing | `agents/05-testing-agent.md` | `skills/test-execution.md` | Tests mapped to AC IDs; coverage vs `config/project.yaml` |
| Phase 3 | Review | `agents/06-review-agent.md` | `skills/code-analysis.md`; `tools/static-analyzer.md`, `tools/dependency-analyzer.md` | 0 BLOCKERs; AC IDs covered by tests |
| Phase 4–5 | PR / deployment | `agents/07-deployment-agent.md` | `skills/git-operations.md`, `skills/pr-management.md`, `skills/jira-integration.md` | Conventional commit + pull request |
| Phase 6 | Maintenance | `agents/08-maintenance-agent.md` | `tools/dependency-analyzer.md`, `tools/static-analyzer.md` | Triage, audits, health, dependency PRs |
| Phase 4–5 (docs) | Documentation | `agents/09-confluence-docs-agent.md` | Confluence/Slack MCP as documented in `mcp/README.md` | Release notes / doc sync |
| Continuous | Framework evolution | `agents/10-framework-evolution-agent.md` | Writes `config/learnings.yaml` for other agents | `/learn`, `/retro` |

Ticket work is routed after discovery (`install/.cursor/rules/00-project-discovery.mdc`): start/implement → full pipeline; fix/bug → bug pipeline; phase-only prompts stay on that phase.

**Not included in this factory stitch:** `agents/11-errorHandling-review-agent.md` and `agents/12-errorHandling-fix-agent.md`. They exist in this repository as a separate review-then-fix pair for named `apps/` services and are not part of the documented `/start` pipeline or the agent roster in `install/.claude/CLAUDE.md`.

## Commands that run the workflow

Copy `install/.claude/commands/` into the product repo as `.claude/commands/`. Commands documented in `README.md` and `install/.claude/CLAUDE.md`:

| Command | Role in the factory |
|---------|---------------------|
| `/scan` | Discovery (`agents/01-project-discovery-agent.md`) |
| `/setup`, `/setup --validate` | Integration setup (`install/.claude/commands/setup.md`) |
| `/start TICKET` | Full flow: requirements → design → implement → review → PR (`install/.claude/commands/start.md`) |
| `/requirements TICKET` | Phase 1 only |
| `/design TICKET` | Phase 2 only (requirements doc must exist) |
| `/implement TICKET` | Phase 3 development |
| `/test` | Testing agent |
| `/review` | Review agent |
| `/pr TICKET` | Deployment agent (commit + PR) |
| `/fix TICKET` | Bug flow (`install/.claude/commands/fix.md`: maintenance triage + development + review + PR) |
| `/hotfix TICKET` | Emergency P0/P1 path |
| `/release-notes`, `/docs-sync` | Confluence docs agent |
| `/maintenance`, `/deps-update`, `/health` | Maintenance agent |
| `/learn`, `/retro` | Framework evolution agent |

Cursor and other IDEs use the same playbooks via natural language (`install/.cursor/rules/`, `install/.github/copilot-instructions.md`, `install/.continue/config.json`).

## Documented scenarios

Step-by-step scenarios live in `docs/workflows.md`:

1. Full feature development (ticket to PR)
2. Bug fix
3. Dependency security patch
4. Monthly dependency update
5. Project onboarding
6. Performance investigation
7. End-of-sprint retrospective
8. Postmortem capture (`/learn`)

## Skills and tools (existing only)

**Skills** (`skills/`): `jira-integration.md`, `figma-integration.md`, `external-sources.md`, `git-operations.md`, `code-analysis.md`, `test-execution.md`, `pr-management.md`, `project-scanning.md`.

**Tools** (`tools/`): `tech-stack-detector.md`, `static-analyzer.md`, `dependency-analyzer.md`.

**MCP setup:** `mcp/README.md` (Jira, Confluence, GitHub, GitLab, Figma, Slack, filesystem).

**Rules copied with the factory:** `install/.cursor/rules/00-project-discovery.mdc` through `09-pipeline-enforcement.mdc`.

## Setup in a product repository

There is no installer package in this repository. Setup is the copy-and-configure sequence in `README.md` and `docs/getting-started.md`.

1. Copy into the product project root:
   - `install/.claude/` → `.claude/`
   - `install/.cursor/` → `.cursor/`
   - this framework tree as `agentic-pdlc-framework/` (including `agents/`, `skills/`, `tools/`, `mcp/`, `config/`, `docs/`)
   - `install/.github/copilot-instructions.md`, `install/.continue/config.json` as documented in `README.md`
2. Copy `config/project.template.yaml` to `config/project.yaml` (or an example under `config/examples/`).
3. Copy `install/.env.example` to `.env` and fill credentials. Edit `config/integrations.yaml` for non-secret URLs and keys.
4. Load environment variables and run `/setup --validate`.
5. Run `/scan` (or equivalent natural language) so the Discovery agent fills `tech.*` and `commands.*`.
6. Use `/start TICKET-ID` (or the prompt sequence in `docs/getting-started.md`) for ticket work.

After copy, agent paths inside commands are `agentic-pdlc-framework/agents/...` as in `install/.claude/commands/start.md`.

## Pipeline gates (do not skip)

From `install/.claude/CLAUDE.md` and `install/.cursor/rules/09-pipeline-enforcement.mdc`:

1. Discovery — `config/project.yaml`
2. Requirements — `docs/designs/TICKET-requirements.md` with `TICKET-AC-N`
3. Design — `docs/designs/TICKET-design.md` (or the design doc produced by the architecture agent)
4. Branch — `feat/TICKET-*` or `fix/TICKET-*` before source changes
5. Skeleton — signatures only
6. Tests — written and failing
7. Implementation — tests pass, lint clean
8. Review — 0 BLOCKERs
9. PR — branch pushed, PR opened

`--autonomous` on `/start` skips only the Jira clarification loop, not the other gates (`install/.claude/commands/start.md`).

## Further documentation in this repository

- `docs/getting-started.md` — setup
- `docs/agents-guide.md` — agent reference
- `docs/configuration.md` — `project.yaml`
- `docs/workflows.md` — scenarios
- `docs/tech-stack-guide.md` — language conventions
- `architecture.drawio` — phase diagram
- `mcp/README.md` — MCP installation

## Useful Links

- [Demo of aPDLC in action](https://epam.sharepoint.com/:v:/s/EPAMEPAMIHTCAthenaEMR/IQASl4ZsWnw2S5YN59Sqaq0cAVAkrt3fIGMusd014wx8dOc)
- [How to use agenticPDLC Framework](https://epam.sharepoint.com/:v:/r/sites/EPAMEPAMIHTCAthenaEMR/Shared%20Documents/General/AI%20Run%202026/Demo%20-%20Agentic%20PDLC%20Framework/AgenticPDLC-setup.mp4)
