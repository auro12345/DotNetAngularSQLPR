# Getting Started with the PDLC Framework

This guide walks you through setting up the PDLC framework in a new or existing project.

## Prerequisites

- Cursor IDE (for `.cursor/rules/` support) or Claude Code CLI
- Git installed
- Project hosted on GitHub, GitLab, or Bitbucket (for PR automation)
- Optional: Jira account (for ticket integration)

## Setup: New Project

### Step 1: Copy the Framework Files

Copy the PDLC framework structure into your project root:

```
your-project/
├── .cursor/
│   └── rules/           ← copy all .mdc files here
├── config/
│   ├── project.yaml     ← will be created by discovery agent
│   ├── project.template.yaml
│   └── examples/
├── docs/                ← framework docs (this folder)
│   └── designs/         ← create this for architecture docs
```

Minimum required files:
- `.cursor/rules/` — all 9 rule files
- `agentic-pdlc-framework/config/project.template.yaml`
- `config/integrations.yaml`
- `.env.example`

### Step 2: Configure Integrations

**2a. Non-secret settings** — edit `config/integrations.yaml` (safe to commit):
```yaml
sourceControl:
  provider: github
  github:
    owner: "your-org"
    repo: "your-repo"

issueTracking:
  provider: jira
  jira:
    baseUrl: "https://your-org.atlassian.net"
    projectKey: "PROJ"

design:
  figma:
    enabled: false   # set true if your team uses Figma

documentation:
  confluence:
    baseUrl: "https://your-org.atlassian.net/wiki"
    spaceKey: "PROJ"
```

**2b. Credentials** — copy `.env.example` to `.env` (never commit):
```bash
cp .env.example .env
# Then fill in tokens/passwords in .env
```

Key credentials to set:
```bash
JIRA_API_TOKEN=your-atlassian-api-token
JIRA_USER_EMAIL=you@company.com
GITHUB_PERSONAL_ACCESS_TOKEN=ghp_xxxxxxxxxxxx
FIGMA_ACCESS_TOKEN=figd_xxxxxxxxxxxx   # only if design.figma.enabled: true
```

Run the setup wizard to validate everything is wired up correctly:
```
/setup --validate
```

### Step 2b: Create Your Project Config

Copy the template for project metadata (tech stack, commands, standards):
```bash
cp agentic-pdlc-framework/config/project.template.yaml config/project.yaml
```

Fill in the human-authored fields:
```yaml
project:
  name: "your-project-name"
  description: "What your project does"
  domain: "your domain (e.g., e-commerce)"
  team: "Your Team"
  owner: "You"
```

### Step 3: Let Discovery Agent Auto-Fill Tech Stack

Open your project in Cursor and say:
```
Scan this project and update config/project.yaml with the tech stack
```

The Project Discovery Agent will:
1. Scan your package.json / pyproject.toml / pom.xml
2. Detect framework, test runner, linter, CI, etc.
3. Fill in all `tech.*` and `commands.*` fields
4. Print a discovery summary

### Step 4: Verify the Config

After discovery, review `config/project.yaml` and confirm:
- [ ] `tech.primaryLanguage` is correct
- [ ] `tech.framework` is correct
- [ ] `commands.test` runs your tests when executed manually
- [ ] `commands.lint` runs your linter when executed manually

Also confirm `config/integrations.yaml`:
- [ ] `issueTracking.jira.projectKey` matches your Jira project
- [ ] `sourceControl.github.owner` and `.repo` are correct
- [ ] `design.figma.enabled` reflects whether your team uses Figma

### Step 5: Load Environment Variables

```bash
# Load from .env into current shell session
export $(cat .env | grep -v '^#' | xargs)

# Or add to ~/.zshrc / ~/.bashrc for persistence
```

For GitHub, you can also use the `gh` CLI:
```bash
gh auth login
```

See `.env.example` for all supported variables, and `agentic-pdlc-framework/mcp/README.md` for per-service setup instructions.

### Step 6: Start the learning loop

`config/learnings.yaml` already exists with commented-out examples. You don't need to fill it in now — it grows over time. After your first sprint or first production incident, run:

```bash
# Capture a single learning immediately:
/learn "description of what the team learned"

# After a sprint — analyze recent PRs and propose learnings:
/retro
```

The framework encodes what your team learns and applies it automatically to every future agent run.

---

## Setup: Existing Project

Same steps as new project, with one addition:

**Before auto-discovery**, compare the example config for your stack:
- React + JS: `agentic-pdlc-framework/config/examples/react-javascript.yaml`
- React + TS / Next.js: `agentic-pdlc-framework/config/examples/react-typescript.yaml`
- Node Express API: `agentic-pdlc-framework/config/examples/node-express-api.yaml`

Copy the closest example and customize it instead of starting from the template.

---

## Using the Framework Day-to-Day

### Starting a New Feature

1. **Get the ticket** (or describe the task)
2. The Requirements Agent reads and analyzes it
3. The Architecture Agent designs the solution
4. The Development Agent implements skeleton → tests → code
5. The Review Agent self-reviews
6. The Deployment Agent creates the PR

**Example prompt sequence:**
```
→ "Start work on PROJ-123"
→ [Requirements Agent reads and validates]
→ "Design the solution"
→ [Architecture Agent produces design doc]
→ "Implement the solution"
→ [Development Agent: skeleton → tests → implementation]
→ "Review and create PR"
→ [Review Agent checks, Deployment Agent opens PR]
```

### Fixing a Bug

```
→ "Fix the bug reported in PROJ-456"
→ [Requirements Agent reads the bug report]
→ [Maintenance Agent triages severity]
→ "Implement the fix"
→ [Development Agent: regression test first, then fix]
→ "Create PR"
→ [Deployment Agent creates PR with [fix] prefix]
```

### Running Maintenance

```
→ "Run weekly maintenance check"
→ [Maintenance Agent: security audit, dependency check, health report]

→ "Update dependencies"
→ [Maintenance Agent: group updates by semver, create batched PRs]
```

---

## IDE Integration

### Cursor IDE
The `.cursor/rules/` files are loaded automatically. Rules with `alwaysApply: true` activate on every session. Others activate based on the `globs` patterns matching files you're editing.

### Claude Code CLI
Use agents directly:
```bash
# Run project discovery
claude "scan this project and update config/project.yaml"

# Start a feature
claude "implement PROJ-123: add user authentication"

# Review changes
claude "review all changed files and create a PR"
```

### VS Code with Claude
Invoke agents from the chat panel using the prompts in each agent's "Prompting the Agent" section.

---

## Troubleshooting

**Discovery doesn't detect my framework:**
Add it manually to `config/project.yaml > tech.framework` and file a note in the project scanning skill.

**Commands in project.yaml don't work:**
Test each command manually in your terminal. The format must be an exact shell command (no aliases).

**Jira integration fails:**
Check that `JIRA_API_TOKEN` is set in your shell. Test with:
```bash
curl -H "Authorization: Basic $(echo -n $JIRA_USER_EMAIL:$JIRA_API_TOKEN | base64)" \
  "https://your-org.atlassian.net/rest/api/3/issue/PROJ-1"
```
