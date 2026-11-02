# PDLC Framework — Cursor Project Context

## What This Project Is

This repository contains the **PDLC (Product Development Life Cycle) Framework** — a set of AI agent definitions, rules, skills, tools, and configuration templates that automate end-to-end software development and maintenance.

**This is the framework itself, not a product being built.**
When you work in this repository, you are developing and improving the framework.

## IDE Support

This framework supports multiple IDEs by providing IDE-specific context files:

| IDE | File | How it loads |
|-----|------|-------------|
| **Cursor** | `.cursor/context.md` + `.cursor/rules/*.mdc` | Auto-loaded |
| **Claude Code** | `.claude/CLAUDE.md` + `.claude/commands/` | Auto-loaded + slash commands |
| **VS Code Copilot** | `.github/copilot-instructions.md` | Auto-loaded by Copilot |
| **Continue.dev** | `.continue/config.json` | Auto-loaded |

All IDEs reference the same `agentic-pdlc-framework/agents/`, `agentic-pdlc-framework/skills/`, `agentic-pdlc-framework/tools/`, and `agentic-pdlc-framework/config/` directories.

## Repository Structure

```
PDLC/
├── .claude/
│   ├── CLAUDE.md               ← Claude Code context
│   ├── settings.json           ← MCP server config (Jira, GitHub, Confluence, Slack)
│   └── commands/               ← 14 slash commands (/scan, /start, /review, etc.)
├── .cursor/
│   ├── context.md              ← YOU ARE HERE
│   └── rules/                  ← Cursor rules (loaded automatically)
│       ├── 00-project-discovery.mdc
│       ├── 01-requirements.mdc
│       ├── 02-analysis-design.mdc
│       ├── 03-development.mdc
│       ├── 04-testing.mdc
│       ├── 05-code-review.mdc
│       ├── 06-deployment.mdc
│       ├── 07-maintenance.mdc
│       └── 08-project-config.mdc
│
├── .continue/
│   └── config.json             ← Continue.dev (VS Code alternative to Copilot)
├── .github/
│   └── copilot-instructions.md ← GitHub Copilot + JetBrains AI context
├── .vscode/
│   ├── extensions.json
│   └── settings.json
│
├── agentic-pdlc-framework/                  ← Framework code (agents, skills, tools, docs)
│   ├── agents/                 ← Agent playbooks (how each agent works)
│   │   ├── 01-project-discovery-agent.md
│   │   ├── 02-requirements-agent.md
│   │   ├── 03-architecture-agent.md
│   │   ├── 04-development-agent.md
│   │   ├── 05-testing-agent.md
│   │   ├── 06-review-agent.md
│   │   ├── 07-deployment-agent.md
│   │   ├── 08-maintenance-agent.md
│   │   ├── 09-confluence-docs-agent.md
│   │   └── 10-framework-evolution-agent.md
│   ├── skills/                 ← Reusable capability specs
│   │   ├── jira-integration.md
│   │   ├── git-operations.md
│   │   ├── code-analysis.md
│   │   ├── test-execution.md
│   │   ├── pr-management.md
│   │   ├── project-scanning.md
│   │   ├── figma-integration.md
│   │   └── external-sources.md
│   ├── tools/                  ← Low-level tool specifications
│   │   ├── tech-stack-detector.md
│   │   ├── dependency-analyzer.md
│   │   └── static-analyzer.md
│   ├── mcp/                    ← MCP setup guides
│   │   ├── README.md           ← Setup overview and troubleshooting
│   │   ├── jira-mcp.md
│   │   ├── github-mcp.md
│   │   ├── confluence-mcp.md
│   │   └── slack-mcp.md
│   ├── config/                 ← Framework config templates
│   │   ├── project.template.yaml ← Blank template for new projects
│   │   └── examples/
│   │       ├── react-javascript.yaml
│   │       ├── react-typescript.yaml
│   │       └── node-express-api.yaml
│   ├── docs/                   ← Framework documentation
│   │   ├── README.md
│   │   ├── getting-started.md
│   │   ├── agents-guide.md
│   │   ├── configuration.md
│   │   ├── workflows.md
│   │   └── tech-stack-guide.md
│   ├── architecture.drawio     ← PDLC flow diagram (all 6 phases)
│   └── README.md               ← Framework setup and usage guide
│
├── config/                     ← Runtime project config (stays at root)
│   ├── integrations.yaml
│   └── learnings.yaml
│
└── docs/
    └── designs/                ← Project-generated output (stays at root)
```

## Key Design Principles of This Framework

1. **Project Discovery First** — Every AI session starts by scanning the target project and loading its `config/project.yaml`. Nothing proceeds without this context.

2. **Config-Driven Behavior** — All agent behavior adapts to the project's tech stack and standards via `config/project.yaml`. Rules reference config, not hard-code values.

3. **Skeleton-First Development** — Agents always create structure (classes, signatures, TODOs) before implementation. This enables review of the design before writing logic.

4. **Tests Before Implementation** — Tests are written against the skeleton (and fail) before implementation begins. Tests define the contract.

5. **Traceability** — Every commit traces to a ticket. Every test traces to an acceptance criterion.

6. **Full Lifecycle** — The framework covers all 6 phases: Requirements → Analysis → Development → Testing → Review → PR → Deploy → Maintain → (back to Requirements).

## How This Framework Is Used

1. **Copy framework files** into a target project (the `.cursor/` folder, `config/` template)
2. **Create `config/project.yaml`** for the target project (auto-populated by discovery agent)
3. **Open the target project in Cursor** — rules load automatically
4. **Invoke agents by natural language** — "Start work on PROJ-123", "Review my changes", etc.

## When Working on This Framework

- `.cursor/rules/*.mdc` files — these are Cursor IDE rules; follow the `.mdc` format with frontmatter
- `agentic-pdlc-framework/agents/*.md` — plain markdown, structured as agent playbooks
- `agentic-pdlc-framework/skills/*.md` — reusable capability specs, include code snippets for each operation
- `agentic-pdlc-framework/tools/*.md` — low-level utilities, include exact commands and output schemas
- `agentic-pdlc-framework/config/project.template.yaml` — every new field must have a comment explaining it
- `docs/*.md` — user-facing documentation; keep it accurate and up to date

## Extending the Framework

### Adding support for a new tech stack
1. Add detection logic to `agentic-pdlc-framework/tools/tech-stack-detector.md`
2. Add commands to `agentic-pdlc-framework/skills/code-analysis.md` and `agentic-pdlc-framework/skills/test-execution.md`
3. Add example config to `agentic-pdlc-framework/config/examples/[stack].yaml`
4. Add conventions section to `agentic-pdlc-framework/docs/tech-stack-guide.md`

### Adding a new agent
1. Create `agentic-pdlc-framework/agents/NN-[name]-agent.md` with the standard structure
2. Add an entry to the agents table in `agentic-pdlc-framework/docs/README.md` and `agentic-pdlc-framework/docs/agents-guide.md`
3. Update `architecture.drawio` if the agent adds a new phase
4. Add a `.cursor/rules/NN-[name].mdc` rule if the agent needs auto-activation

### Modifying an existing rule
1. Edit the `.cursor/rules/` file
2. Update corresponding documentation in `docs/`
3. If it adds a new `project.yaml` field, update `agentic-pdlc-framework/config/project.template.yaml`
