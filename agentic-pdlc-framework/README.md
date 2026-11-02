# PDLC Framework

An AI-agent system that automates the complete **Product Development Life Cycle** — from reading requirements through production maintenance — for any tech stack and any IDE.

```
Requirements → Analysis → Development → Testing → Review → PR → Deploy → Maintain
     ↑                                                                        │
     └──────────────────────── Feedback Loop ───────────────────────────────┘
```

---

## How to Use It

### Step 1 — Copy the framework into your project

```bash
# Clone or download this repo, then copy these folders into your project root:
cp -r .claude/             /path/to/your-project/
cp -r .cursor/             /path/to/your-project/
cp -r agentic-pdlc-framework/           /path/to/your-project/
cp .github/copilot-instructions.md  /path/to/your-project/.github/
cp .continue/config.json            /path/to/your-project/.continue/
```

### Step 2 — Configure your project

```bash
cd /path/to/your-project
cp agentic-pdlc-framework/config/project.template.yaml config/project.yaml
```

Open `config/project.yaml` and fill in the human-authored fields:
```yaml
project:
  name: "your-app"
  description: "What your project does"
  domain: "e-commerce"      # your business domain

integrations:
  jira:
    baseUrl: "https://your-org.atlassian.net"
    projectKey: "PROJ"
  git:
    provider: github
    owner: "your-org"
    repo: "your-repo"
```

> **Tip:** Copy the closest example config instead of the blank template:
> - `agentic-pdlc-framework/config/examples/react-javascript.yaml` — React + JS SPA
> - `agentic-pdlc-framework/config/examples/react-typescript.yaml` — Next.js + TypeScript
> - `agentic-pdlc-framework/config/examples/node-express-api.yaml` — Express REST API

### Step 3 — Configure integrations

**3a.** Copy `.env.example` to `.env` and fill in your credentials:
```bash
cp .env.example .env
```

Key credentials to set:
```bash
# Atlassian (Jira + Confluence — same token)
JIRA_API_TOKEN="your-atlassian-api-token"
JIRA_USER_EMAIL="you@company.com"

# GitHub
GITHUB_PERSONAL_ACCESS_TOKEN="ghp_xxxxxxxxxxxx"

# Figma (only if design.figma.enabled: true in config/integrations.yaml)
FIGMA_ACCESS_TOKEN="figd_xxxxxxxxxxxx"

# Slack (optional)
SLACK_BOT_TOKEN="xoxb-xxxxxxxxxxxx"
```

**3b.** Edit `config/integrations.yaml` with your non-secret settings (URLs, project keys, space keys):
```yaml
issueTracking.jira.baseUrl: "https://your-org.atlassian.net"
issueTracking.jira.projectKey: "PROJ"
sourceControl.github.owner: "your-org"
sourceControl.github.repo: "your-repo"
```

Load environment variables and validate all connections:
```bash
export $(cat .env | grep -v '^#' | xargs)
```
Then: `/setup --validate`

See `agentic-pdlc-framework/mcp/README.md` for full per-service setup instructions.

### Step 4 — Open your project in your IDE

| IDE | What loads automatically |
|-----|--------------------------|
| **Claude Code** | `.claude/CLAUDE.md` + slash commands from `.claude/commands/` |
| **Cursor** | `.cursor/rules/*.mdc` (all 9 rules auto-activate) |
| **VS Code + Copilot** | `.github/copilot-instructions.md` |
| **VS Code + Continue.dev** | `.continue/config.json` |
| **JetBrains + AI Assistant** | `.github/copilot-instructions.md` |

### Step 5 — Run project discovery

**Claude Code:**
```
/scan
```

**Cursor / natural language:**
```
Scan this project and set up config/project.yaml
```

The Discovery Agent will auto-detect your tech stack and fill in all `tech.*` and `commands.*` fields. It prints a summary and flags anything needing manual input.

### Step 6 — Start working

Use commands (Claude Code) or natural language (any IDE):

---

## Slash Commands (Claude Code)

All commands automatically run project discovery as a precheck if `config/project.yaml` is missing.

| Command | What it does |
|---------|-------------|
| `/scan` | Scan project, create/update `config/project.yaml` |
| `/start PROJ-123` | Full PDLC: requirements → design → implement → review → PR |
| `/requirements PROJ-123` | Requirements phase only — fetch ticket, structure ACs |
| `/design PROJ-123` | Design phase — solution approach, diagrams, file map |
| `/implement PROJ-123` | Implementation — skeleton first, tests, code, static analysis |
| `/test` | Run test suite, report coverage, analyze failures |
| `/test src/auth/` | Run tests for a specific path |
| `/review` | Self-review all changes — BLOCKER/WARNING report |
| `/review PROJ-123` | Review changes scoped to a specific ticket |
| `/pr PROJ-123` | Commit + create pull request with full template |
| `/fix PROJ-123` | Bug fix flow — triage, regression test, fix, PR |
| `/hotfix PROJ-123` | Emergency P0/P1 fast-track from production tag |
| `/release-notes v1.2.3` | Generate and push release notes to Confluence |
| `/release-notes PROJ-123` | Release notes for a specific feature ticket |
| `/docs-sync PROJ-123` | Sync design docs / API docs to Confluence |
| `/maintenance` | Weekly check: security audit, deps, health report |
| `/maintenance --security` | Security audit only |
| `/deps-update` | Analyze + apply dependency updates, create PRs |
| `/deps-update --patch` | Patch updates only (safe, automated) |
| `/health` | Project health dashboard (tests, coverage, PRs, debt) |
| `/health --confluence` | Health dashboard + publish to Confluence |
| `/setup` | Interactive wizard to configure all integrations |
| `/setup --validate` | Test all configured integrations and report status |
| `/learn "description"` | Capture a team learning into `config/learnings.yaml` |
| `/retro` | Sprint retrospective — analyze PR patterns, propose learnings |

---

## Common Workflows

### New feature (ticket to PR)
```
/start PROJ-123
```
Runs the full pipeline automatically. Pause at design to review before implementation.

### Only need a code review before pushing
```
/review
```
Produces a structured BLOCKER/WARNING/SUGGESTION report. Fix BLOCKERs, then `/pr PROJ-123`.

### Bug came in, need to fix it fast
```
/fix PROJ-456
```
Triages severity, finds root cause, writes regression test first, then fixes.

### Production is down (P0)
```
/hotfix PROJ-789
```
Branches from the production tag, applies surgical fix, fast-tracks to deployment.

### Weekly housekeeping
```
/maintenance
/deps-update --patch
```

### Just merged a PR — publish release notes
```
/release-notes PROJ-123
```
Generates plain-English release notes and pushes them to Confluence.

---

## Agents

| Agent | File | Phase |
|-------|------|-------|
| Project Discovery | `agentic-pdlc-framework/agents/01-project-discovery-agent.md` | Always (session start) |
| Requirements | `agentic-pdlc-framework/agents/02-requirements-agent.md` | Phase 1 |
| Architecture | `agentic-pdlc-framework/agents/03-architecture-agent.md` | Phase 2 |
| Development | `agentic-pdlc-framework/agents/04-development-agent.md` | Phase 3 |
| Testing | `agentic-pdlc-framework/agents/05-testing-agent.md` | Phase 3 |
| Code Review | `agentic-pdlc-framework/agents/06-review-agent.md` | Phase 3 |
| Deployment | `agentic-pdlc-framework/agents/07-deployment-agent.md` | Phase 4–5 |
| Maintenance | `agentic-pdlc-framework/agents/08-maintenance-agent.md` | Phase 6 |
| Confluence Docs | `agentic-pdlc-framework/agents/09-confluence-docs-agent.md` | Phase 4–5 |

---

## IDE Support

### Claude Code
- Context: `.claude/CLAUDE.md` (auto-loaded)
- Commands: `.claude/commands/*.md` → available as `/command`
- MCPs: `.claude/settings.json` (Jira, GitHub, Confluence, Slack)

### Cursor
- Rules: `.cursor/rules/*.mdc` (auto-loaded, 9 rules)
- Context: `.cursor/context.md`
- All agents invokable by natural language

### VS Code + GitHub Copilot
- Instructions: `.github/copilot-instructions.md` (auto-loaded by Copilot)
- Invoke agents with natural language in Copilot Chat
- Install recommended extensions: `.vscode/extensions.json`

### VS Code + Continue.dev
- Config: `.continue/config.json` (slash commands pre-configured)
- Commands: `/scan`, `/requirements`, `/design`, `/implement`, `/review`, `/fix`

### JetBrains (IntelliJ, PyCharm, WebStorm)
- AI Assistant reads `.github/copilot-instructions.md`
- Use natural language in AI Assistant chat panel

---

## MCP Integrations

Configured in `.claude/settings.json`. See `agentic-pdlc-framework/mcp/README.md` for setup.

| Service | MCP Package | What it enables |
|---------|------------|-----------------|
| Jira | `mcp-atlassian` | Fetch tickets, update status, add comments |
| Confluence | `mcp-atlassian` | Publish release notes, sync design docs |
| GitHub | `@modelcontextprotocol/server-github` | Create PRs, read CI status, manage issues |
| GitLab | `mcp-gitlab` | Create MRs, check pipelines (GitLab projects) |
| Figma | `figma-mcp` | Extract design specs, text strings, interactions |
| Slack | `@modelcontextprotocol/server-slack` | Post deployment and alert notifications |

---

## Documentation

| Doc | Description |
|-----|-------------|
| `agentic-pdlc-framework/docs/getting-started.md` | Detailed setup guide |
| `agentic-pdlc-framework/docs/agents-guide.md` | What each agent does and how to invoke it |
| `agentic-pdlc-framework/docs/configuration.md` | Complete `project.yaml` field reference |
| `agentic-pdlc-framework/docs/workflows.md` | Step-by-step scenario walkthroughs |
| `agentic-pdlc-framework/docs/tech-stack-guide.md` | React, TS, Python, Java, Go conventions |
| `agentic-pdlc-framework/mcp/README.md` | MCP installation and troubleshooting |

---

## Project Structure

```
.claude/
  CLAUDE.md              ← Claude Code context (auto-loaded)
  settings.json          ← MCP server configuration
  commands/              ← 19 slash commands
    scan.md, start.md, requirements.md, design.md,
    implement.md, test.md, review.md, pr.md,
    fix.md, hotfix.md, release-notes.md, docs-sync.md,
    maintenance.md, deps-update.md, health.md,
    learn.md, retro.md, setup.md

.cursor/
  context.md             ← Cursor context
  rules/                 ← 9 Cursor rules (auto-activate)

.continue/config.json    ← Continue.dev (VS Code)
.github/
  copilot-instructions.md ← Copilot + JetBrains AI context
.vscode/
  extensions.json        ← Recommended extensions
  settings.json          ← Editor settings

agentic-pdlc-framework/               ← All framework code
  agents/                ← 10 agent playbooks
  skills/                ← 8 reusable skills
  tools/                 ← 3 low-level tools
  mcp/                   ← MCP setup guides
  config/
    project.template.yaml  ← Config schema with comments
    examples/              ← React JS, React TS, Node Express
  docs/                  ← Framework documentation
  architecture.drawio    ← 6-phase PDLC flow diagram

config/                  ← Runtime project config (not framework)
  integrations.yaml
  learnings.yaml
docs/
  designs/               ← Project-generated output
```
