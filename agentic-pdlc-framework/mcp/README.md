# MCP (Model Context Protocol) Setup

The PDLC framework uses MCPs to connect Claude Code to external services. This guide covers setup for all supported integrations.

## What MCPs Are Used For

| Integration | MCP Package | Used By |
|-------------|------------|---------|
| Jira | `mcp-atlassian` | Requirements, fix, maintenance agents |
| Confluence | `mcp-atlassian` | Confluence docs agent, release notes |
| GitHub | `@modelcontextprotocol/server-github` | Deployment, PR management |
| GitLab | `mcp-gitlab` | Deployment (GitLab projects) |
| Figma | `figma-mcp` | Requirements agent — design spec extraction |
| Slack | `@modelcontextprotocol/server-slack` | Deployment notifications, health alerts |
| Filesystem | `@modelcontextprotocol/server-filesystem` | Local file operations |

## Quick Setup

### Step 1: Configure MCPs in `.claude/settings.json`

The MCP server config is already in `.claude/settings.json`. You only need to provide the environment variables.

### Step 2: Set Environment Variables

Copy the template and fill in your credentials:
```bash
cp .env.example .env
```

Key variables to set:

```bash
# Atlassian (Jira + Confluence — same token)
JIRA_API_TOKEN=your-atlassian-api-token
JIRA_BASE_URL=https://your-org.atlassian.net
JIRA_USER_EMAIL=you@company.com

CONFLUENCE_API_TOKEN=your-atlassian-api-token  # usually same as JIRA_API_TOKEN
CONFLUENCE_BASE_URL=https://your-org.atlassian.net/wiki
CONFLUENCE_USER_EMAIL=you@company.com

# GitHub
GITHUB_PERSONAL_ACCESS_TOKEN=ghp_xxxxxxxxxxxx

# GitLab (if using GitLab)
GITLAB_PERSONAL_ACCESS_TOKEN=glpat-xxxxxxxxxxxx
GITLAB_API_URL=https://gitlab.com/api/v4  # or self-hosted URL

# Figma (if design specs are needed in requirements)
FIGMA_ACCESS_TOKEN=figd_xxxxxxxxxxxx

# Slack (if using notifications)
SLACK_BOT_TOKEN=xoxb-xxxxxxxxxxxx
SLACK_TEAM_ID=T0XXXXXXXXX
```

See `.env.example` for the full list of available variables.

Load into your shell (add to `~/.zshrc` or `~/.bashrc` for persistence):
```bash
export $(cat .env | grep -v '^#' | xargs)
```

### Step 3: Install MCP packages (first time only)

MCPs are installed automatically by `npx -y` on first use. To pre-install:
```bash
npx -y mcp-atlassian --help
npx -y @modelcontextprotocol/server-github --help
```

### Step 4: Test Connections

```bash
# Test Jira connection
curl -H "Authorization: Basic $(echo -n $JIRA_USER_EMAIL:$JIRA_API_TOKEN | base64)" \
  "$JIRA_BASE_URL/rest/api/3/myself" | jq '.displayName'

# Test GitHub
curl -H "Authorization: token $GITHUB_PERSONAL_ACCESS_TOKEN" \
  https://api.github.com/user | jq '.login'

# Test Confluence
curl -H "Authorization: Basic $(echo -n $CONFLUENCE_USER_EMAIL:$CONFLUENCE_API_TOKEN | base64)" \
  "$CONFLUENCE_BASE_URL/rest/api/space" | jq '.results[0].name'
```

---

## Individual MCP Guides

- [Jira & Confluence MCP](jira-mcp.md) — Atlassian suite setup
- [GitHub MCP](github-mcp.md) — GitHub integration
- [GitLab MCP](gitlab-mcp.md) — GitLab MR and pipeline integration
- [Confluence MCP Details](confluence-mcp.md) — Detailed Confluence operations
- [Figma MCP](figma-mcp.md) — Design spec extraction
- [Slack MCP](slack-mcp.md) — Notification setup

---

## Integration Configuration

Non-secret settings (URLs, project keys, space keys, feature flags) live in `config/integrations.yaml` — **not** in environment variables. Edit that file once at project setup:

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
    enabled: true   # set to true to activate design extraction in requirements phase
    teamId: "123456789"

documentation:
  confluence:
    baseUrl: "https://your-org.atlassian.net/wiki"
    spaceKey: "PROJ"
    pages:
      releaseNotesParent: 123456   # right-click page → Page Information → ID in URL
      designDocsParent: 123457
```

See the full schema in `config/integrations.yaml`. Credentials (tokens, passwords) always go in `.env`, never in this file.

---

## Rate Limits and Retry Behavior

External APIs enforce rate limits. Here are the defaults and how the framework handles them:

| Service | Rate Limit | Strategy |
|---------|-----------|----------|
| Jira Cloud | 10 req/s per token | Automatic retry with 1s backoff on 429 |
| GitHub API | 5000 req/hr (authenticated) | Check `X-RateLimit-Remaining` header; pause if < 100 |
| Figma API | 60 req/min per token | Batch frame exports; cache responses within session |
| Confluence Cloud | Same token as Jira | Share rate limit budget with Jira calls |
| Slack | 1 req/s per method | Queue messages; don't burst on deployments |

**What to do when you hit a rate limit:**
```
- Wait for the retry-after window before re-invoking the agent
- For large Jira projects with many linked tickets: use /requirements with a single ticket ID,
  not bulk fetch — agents self-limit to max 10 external fetches per ticket
- For Figma: re-running /requirements will use cached node data if the session is still active
```

**Offline / degraded mode:**
If an MCP is unavailable (token expired, API down, network issue):
1. The agent reports which integrations are unavailable
2. Proceeds with available sources only
3. Flags gaps in the output (`⚠️ Jira unavailable — requirements based on user description only`)

---

## Troubleshooting

**MCP not starting:**
```bash
# Check if the MCP process starts cleanly
npx -y mcp-atlassian 2>&1 | head -20
```

**Jira 401 errors:**
- Verify `JIRA_API_TOKEN` is a valid API token (not your password)
- Generate one at: https://id.atlassian.com/manage-profile/security/api-tokens
- Token must be Base64-encoded with `email:token` format

**GitHub 401 errors:**
- Verify your PAT has the required scopes: `repo`, `read:org`
- Check expiry: PATs can be set to expire

**Confluence page not found:**
- Verify the space key matches your Confluence space
- Page IDs change if pages are deleted and recreated — update config after page moves

**MCP tools not appearing in Claude Code:**
- Restart Claude Code after changing `.claude/settings.json`
- Check the Claude Code MCP panel (Settings → MCP Servers) for error messages
