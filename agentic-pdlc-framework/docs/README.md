# PDLC Framework Documentation

The **PDLC (Product Development Life Cycle) Framework** is an AI-agent-powered system that automates and governs every phase of software development — from reading requirements to deploying code and maintaining the product.

## What This Framework Does

The framework orchestrates a set of **specialized AI agents**, each responsible for one phase of the PDLC. Together they enforce consistent quality, traceability, and best practices — regardless of which tech stack you're using.

```
Requirements → Analysis → Development → Testing → Review → PR → Deploy → Maintain
     ↑                                                                       |
     └───────────────────── Feedback Loop ──────────────────────────────────┘
```

## Quick Navigation

| Document | Description |
|----------|-------------|
| [Getting Started](getting-started.md) | Set up the framework for a new or existing project |
| [Agents Guide](agents-guide.md) | Detailed reference for each agent and when to use them |
| [Configuration](configuration.md) | Complete `project.yaml` field reference |
| [Workflows](workflows.md) | Step-by-step walkthroughs of common scenarios |
| [Tech Stack Guide](tech-stack-guide.md) | Language and framework-specific conventions |

## Core Concepts

### Agents
Specialized AI agents, each owning one phase of the PDLC:

| Agent | Phase | Responsibility |
|-------|-------|----------------|
| [Project Discovery](../agents/01-project-discovery-agent.md) | Always | Scans project, updates config, loads context |
| [Requirements](../agents/02-requirements-agent.md) | Phase 1 | Gathers and validates requirements |
| [Architecture](../agents/03-architecture-agent.md) | Phase 2 | Designs solutions and produces blueprints |
| [Development](../agents/04-development-agent.md) | Phase 3 | Implements code skeleton-first |
| [Testing](../agents/05-testing-agent.md) | Phase 3 | Writes and runs tests |
| [Review](../agents/06-review-agent.md) | Phase 3 | Self-review before PR |
| [Deployment](../agents/07-deployment-agent.md) | Phase 4-5 | Commits, PRs, and deploys |
| [Maintenance](../agents/08-maintenance-agent.md) | Phase 6 | Bug triage, updates, health |
| [Confluence Docs](../agents/09-confluence-docs-agent.md) | Phase 4–5 | Release notes, design docs, ADRs to Confluence |
| [Framework Evolution](../agents/10-framework-evolution-agent.md) | Continuous | Captures team learnings into config/learnings.yaml |

### Rules
Cursor rules (`.cursor/rules/`) that govern agent behavior automatically:

| Rule File | Purpose |
|-----------|---------|
| `00-project-discovery.mdc` | Auto-scan project on session start |
| `01-requirements.mdc` | Requirements gathering standards |
| `02-analysis-design.mdc` | Design and diagramming standards |
| `03-development.mdc` | Code quality and skeleton-first approach |
| `04-testing.mdc` | Test coverage and quality standards |
| `05-code-review.mdc` | Self-review checklist |
| `06-deployment.mdc` | Commit messages and PR standards |
| `07-maintenance.mdc` | Bug triage and health monitoring |
| `08-project-config.mdc` | Config file governance |

### Skills
Reusable capabilities called by agents:

| Skill | Purpose |
|-------|---------|
| [jira-integration](../skills/jira-integration.md) | Fetch/update Jira tickets |
| [figma-integration](../skills/figma-integration.md) | Extract design specs from Figma |
| [external-sources](../skills/external-sources.md) | Fetch and analyze URLs, attachments, docs |
| [git-operations](../skills/git-operations.md) | Branching, commits, PRs |
| [code-analysis](../skills/code-analysis.md) | Lint, type check, format |
| [test-execution](../skills/test-execution.md) | Run tests, parse results |
| [pr-management](../skills/pr-management.md) | Multi-provider PR operations |
| [project-scanning](../skills/project-scanning.md) | File system tech stack detection |

### Tools
Low-level utilities:

| Tool | Purpose |
|------|---------|
| [tech-stack-detector](../tools/tech-stack-detector.md) | Detect frameworks from project files |
| [dependency-analyzer](../tools/dependency-analyzer.md) | Security audits and version analysis |
| [static-analyzer](../tools/static-analyzer.md) | Aggregated static analysis runner |

### Configuration
Project-level settings:

| File | Purpose |
|------|---------|
| `agentic-pdlc-framework/config/project.template.yaml` | Blank template for new projects |
| `config/integrations.yaml` | External tool connections (Jira, GitHub, Figma, Confluence, Slack) |
| `config/learnings.yaml` | Team knowledge: patterns, hints, overrides, postmortems |
| `agentic-pdlc-framework/config/examples/react-javascript.yaml` | Example: React + JS SPA |
| `agentic-pdlc-framework/config/examples/react-typescript.yaml` | Example: Next.js + TypeScript |
| `agentic-pdlc-framework/config/examples/node-express-api.yaml` | Example: Express REST API |

## Architecture

See [architecture.drawio](../architecture.drawio) for the visual PDLC flow diagram showing all 6 phases and their relationships.

## Key Principles

1. **Discovery first** — Every session starts by scanning the project and loading context
2. **Requirements traceability** — Every line of code traces back to a ticket
3. **Skeleton before implementation** — Design the shape before filling it in
4. **Tests before code** — Tests define the contract
5. **Self-review before PR** — No human review requested until automated review passes
6. **Maintenance is part of PDLC** — The cycle doesn't end at deployment
7. **Continuously learning** — Run `/retro` after each sprint. The framework evolves with the team.
