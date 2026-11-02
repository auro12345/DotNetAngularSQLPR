Set up or validate all external tool integrations for the PDLC framework.

Mode: $ARGUMENTS
- Empty: interactive setup wizard — walk through each integration
- `--validate`: check all configured integrations and report status
- `--jira`: set up Jira only
- `--figma`: set up Figma only
- `--github`: set up GitHub only
- `--confluence`: set up Confluence only
- `--all`: non-interactive — validate and report everything

## What This Command Does

Reads `config/integrations.yaml` and `.env` (or environment variables), tests each configured integration, reports what works and what's missing, and guides the user through fixing gaps.

## Validation Checks

For each integration that is `enabled: true` in `config/integrations.yaml`:

### Jira
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -H "Authorization: Basic $(echo -n $JIRA_USER_EMAIL:$JIRA_API_TOKEN | base64)" \
  "$JIRA_BASE_URL/rest/api/3/myself"
```
- 200 → Connected ✅
- 401 → Invalid credentials ❌ (check JIRA_API_TOKEN, JIRA_USER_EMAIL)
- 0 → Can't reach URL ❌ (check JIRA_BASE_URL)

Also validate: `issueTracking.jira.projectKey` exists in Jira.

### GitHub
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -H "Authorization: token $GITHUB_PERSONAL_ACCESS_TOKEN" \
  https://api.github.com/user
```
- 200 → Connected ✅
- 401 → Invalid token ❌
- Also check: repo access with `GET /repos/{owner}/{repo}`

### Figma
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -H "X-Figma-Token: $FIGMA_ACCESS_TOKEN" \
  https://api.figma.com/v1/me
```
- 200 → Connected ✅
- 403 → Invalid token ❌ (check FIGMA_ACCESS_TOKEN)

### Confluence
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -H "Authorization: Basic $(echo -n $CONFLUENCE_USER_EMAIL:$CONFLUENCE_API_TOKEN | base64)" \
  "$CONFLUENCE_BASE_URL/rest/api/space/$CONFLUENCE_SPACE_KEY"
```
- 200 → Connected ✅ (also verify space key is correct)
- 401 → Invalid credentials ❌
- 404 → Space not found (check spaceKey in config/integrations.yaml)

### Slack
Check if `SLACK_BOT_TOKEN` is set and starts with `xoxb-`.
If set, test: `https://slack.com/api/auth.test` with Bearer token.

### MCP Connectivity
For each MCP in `.claude/settings.json`, check that:
- The required env var is non-empty
- The npm package is installable (try `npx -y [package] --version`)

## Report Format

```
╔══════════════════════════════════════════════════════════╗
║          PDLC Integration Status Report                  ║
╠══════════════════════════════════════════════════════════╣
║ Source Control                                           ║
║   GitHub:      ✅ Connected (org/repo accessible)        ║
║                                                          ║
║ Issue Tracking                                           ║
║   Jira:        ✅ Connected (project PROJ found)         ║
║   Project key: PROJ  |  Transitions: ⚠️ not configured  ║
║                                                          ║
║ Design Tools                                             ║
║   Figma:       ❌ Not configured                         ║
║                → Set FIGMA_ACCESS_TOKEN in .env          ║
║                → Set design.figma.enabled: true          ║
║                                                          ║
║ Documentation                                            ║
║   Confluence:  ✅ Connected (space PROJ accessible)      ║
║   Page IDs:    ⚠️ 3 page IDs not set in integrations.yaml║
║                                                          ║
║ Notifications                                            ║
║   Slack:       ⚠️ Token set but channels not configured  ║
║                                                          ║
║ MCPs (Claude Code)                                       ║
║   mcp-atlassian:  ✅ Package available                   ║
║   server-github:  ✅ Package available                   ║
║   figma-mcp:      ❌ Token missing (FIGMA_ACCESS_TOKEN)  ║
╠══════════════════════════════════════════════════════════╣
║ STATUS: 3 of 5 integrations fully operational            ║
╚══════════════════════════════════════════════════════════╝

Action items to fix:
1. [FIGMA] Set FIGMA_ACCESS_TOKEN in .env
   → Get token: figma.com → Settings → Personal access tokens
   → Then set: design.figma.enabled: true in config/integrations.yaml

2. [JIRA] Add transition IDs to config/integrations.yaml
   → Run: curl -H "Authorization: Basic ..." "$JIRA_BASE_URL/rest/api/3/issue/PROJ-1/transitions"
   → Update: issueTracking.jira.transitions.*

3. [CONFLUENCE] Set page IDs for PDLC pages
   → Get page ID: open page in browser → ••• → Page Information → ID in URL
   → Update: documentation.confluence.pages.* in config/integrations.yaml
```

## Interactive Setup Wizard (empty $ARGUMENTS)

Walk through each integration interactively:

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PDLC Integration Setup Wizard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
I'll walk you through setting up each integration.
You can skip any by pressing Enter.

[1/6] JIRA
  Is your team using Jira? (yes/no/skip)
  If yes:
    Jira base URL (e.g., https://company.atlassian.net): ___
    Project key (e.g., PROJ): ___
    → I'll test the connection and guide you to get the API token.

[2/6] FIGMA
  Does your team use Figma for designs? (yes/no/skip)
  If yes:
    → I'll show you how to get the token and what the agent will extract.
    Figma access token: ___
    → Testing connection...

[3/6] GITHUB
  ...etc.
```

After setup wizard: update `config/integrations.yaml` with provided values, then run `--validate` to confirm everything works.
