# Jira MCP Configuration

## Package
`mcp-atlassian` — covers both Jira and Confluence with a single package.

## Setup

### 1. Get Atlassian API Token
1. Go to https://id.atlassian.com/manage-profile/security/api-tokens
2. Click "Create API token"
3. Name it "PDLC Framework"
4. Copy the token immediately (shown only once)

### 2. Set Environment Variables
```bash
export JIRA_API_TOKEN="your-api-token-here"
export JIRA_BASE_URL="https://your-org.atlassian.net"
export JIRA_USER_EMAIL="your-email@company.com"
```

### 3. MCP Config (already in .claude/settings.json)
```json
"jira": {
  "command": "npx",
  "args": ["-y", "mcp-atlassian"],
  "env": {
    "JIRA_API_TOKEN": "${JIRA_API_TOKEN}",
    "JIRA_BASE_URL": "${JIRA_BASE_URL}",
    "JIRA_USER_EMAIL": "${JIRA_USER_EMAIL}"
  }
}
```

## Available MCP Tools

### Read Operations
```
mcp__jira__get_issue(issue_key)
  → Full issue: summary, description, status, assignee, AC, labels, links, attachments

mcp__jira__search_issues(jql, max_results)
  → Search with JQL query

mcp__jira__get_issue_comments(issue_key)
  → All comments on an issue

mcp__jira__get_transitions(issue_key)
  → Available status transitions
```

### Write Operations
```
mcp__jira__create_issue(project_key, summary, description, issue_type, priority)
  → Create new ticket, returns issue key

mcp__jira__update_issue(issue_key, fields)
  → Update fields (description, labels, components, etc.)

mcp__jira__transition_issue(issue_key, transition_id)
  → Change status (e.g., "In Progress" → "In Review" → "Done")

mcp__jira__add_comment(issue_key, comment_text)
  → Add comment to issue

mcp__jira__add_remote_link(issue_key, url, title)
  → Add external link (e.g., PR link, Confluence page)
```

## Common JQL Queries Used by Agents

```jql
# Requirements Agent: fetch the ticket
project = PROJ AND key = PROJ-123

# Maintenance Agent: open bugs in backlog
project = PROJ AND issuetype = Bug AND status = Backlog ORDER BY priority ASC

# Health check: current sprint
project = PROJ AND sprint in openSprints() ORDER BY status ASC

# Release notes: tickets in a version
project = PROJ AND fixVersion = "v1.2.3"

# Find all tickets in a PR's commits  
project = PROJ AND key in (PROJ-123, PROJ-124, PROJ-125)
```

## Status Transition Workflow

Typical Jira workflow transitions used by agents:
```
Backlog → In Progress → In Review → Done
                     ↘ Blocked ↗
```

Common transition IDs (vary by project config):
- `11`: To Do
- `21`: In Progress  
- `31`: In Review
- `41`: Done
- `51`: Blocked

Fetch your project's actual transition IDs:
```bash
curl -H "Authorization: Basic $(echo -n $JIRA_USER_EMAIL:$JIRA_API_TOKEN | base64)" \
  "$JIRA_BASE_URL/rest/api/3/issue/PROJ-1/transitions" | jq '.transitions[] | {id, name}'
```

## Acceptance Criteria Extraction

The Jira MCP fetches raw issue content. The Requirements Agent parses ACs from:
1. Custom field `customfield_10016` (Story Points AC field if configured)
2. Description text between heading `## Acceptance Criteria` and next heading
3. Checklist items (`[ ]`) in the description body

## Updating project.yaml from Jira

After first Jira connection, update these fields in `config/project.yaml`:
```yaml
integrations:
  jira:
    enabled: true
    baseUrl: "https://your-org.atlassian.net"
    projectKey: "PROJ"
    # Add transition IDs after running the curl command above:
    transitions:
      inProgress: "21"
      inReview: "31"
      done: "41"
```
