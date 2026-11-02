# Skill: Jira Integration

## Purpose
Fetch, create, and update Jira tickets. Used by Requirements Agent and Deployment Agent.

## Configuration (from project.yaml)
```yaml
integrations:
  jira:
    baseUrl: https://your-org.atlassian.net
    projectKey: PROJ
    # Auth via JIRA_API_TOKEN env variable
```

## Operations

### Fetch Ticket
```
Input:  ticket_id (e.g., "PROJ-123")
Output: Full ticket object including:
  - summary
  - description (full text)
  - acceptance_criteria (from custom field or description)
  - status, priority, assignee
  - linked issues (blocks, is blocked by, relates to, epic)
  - attachments (wireframes, specs)
  - comments
  - labels, components
```

**Implementation (REST API):**
```
GET {baseUrl}/rest/api/3/issue/{ticket_id}
Authorization: Basic {base64(email:JIRA_API_TOKEN)}
```

**Parse acceptance criteria:**
- Check `customfield_10016` (common AC field)
- Or extract from description between `## Acceptance Criteria` and next heading
- Or extract from checklist items in description

### Update Ticket Status
```
Input:  ticket_id, new_status ("In Progress", "In Review", "Done")
Output: Updated ticket

POST {baseUrl}/rest/api/3/issue/{ticket_id}/transitions
Body: { "transition": { "id": "[transition_id]" } }
```

### Add Comment
```
Input:  ticket_id, comment_text
Output: Comment ID

POST {baseUrl}/rest/api/3/issue/{ticket_id}/comment
Body: { "body": { "type": "doc", "version": 1, "content": [...] } }
```

### Fetch Epic
```
Input:  epic_id
Output: Epic with all child stories
GET {baseUrl}/rest/api/3/issue/{epic_id}?fields=subtasks,issuetype
```

### Search Issues
```
Input:  JQL query string
Output: List of matching issues

POST {baseUrl}/rest/api/3/issue/search
Body: { "jql": "project = PROJ AND sprint in openSprints()", "maxResults": 50 }
```

## Common JQL Queries
```
# All open stories in current sprint
project = {projectKey} AND sprint in openSprints() AND issuetype = Story

# Bugs in backlog
project = {projectKey} AND issuetype = Bug AND status = Backlog ORDER BY priority

# Blocked tickets
project = {projectKey} AND issuelinksType = "is blocked by"

# Epic children
"Epic Link" = {epic_id}
```

## Error Handling
- 401: Invalid/expired API token → prompt user to update `JIRA_API_TOKEN` env var
- 404: Ticket not found → check project key and ticket ID
- 429: Rate limited → wait 1 second and retry (max 3 retries)

## Environment Variables Required
```
JIRA_API_TOKEN=your-atlassian-api-token
JIRA_USER_EMAIL=your-email@company.com
```
