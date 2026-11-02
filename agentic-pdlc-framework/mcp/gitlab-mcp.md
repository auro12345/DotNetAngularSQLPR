# GitLab MCP

Use this guide if your project is hosted on GitLab (gitlab.com or self-hosted).

## Setup

### Step 1: Create a Personal Access Token

1. Log in to GitLab → top-right avatar → **Preferences → Access Tokens**
2. Click **Add new token**
3. Name: `PDLC Framework`
4. Scopes: `api`, `read_repository`, `write_repository`
5. Click **Create personal access token** and copy it immediately

For self-hosted GitLab, go to `https://your-gitlab.com/-/profile/personal_access_tokens`.

### Step 2: Set Environment Variables

Add to your `.env`:
```bash
GITLAB_PERSONAL_ACCESS_TOKEN=glpat-xxxxxxxxxxxx
GITLAB_API_URL=https://gitlab.com/api/v4   # or your self-hosted URL
```

### Step 3: Enable in `config/integrations.yaml`

```yaml
sourceControl:
  provider: gitlab
  gitlab:
    enabled: true
    baseUrl: "https://gitlab.com"        # or self-hosted URL
    apiUrl: "https://gitlab.com/api/v4"
    namespace: "your-group-or-username"
    repo: "your-project"
    defaultBranch: main
    mergeStrategy: merge_commit           # merge_commit | squash | rebase
```

---

## Available MCP Tools

The `mcp-gitlab` package provides:

| Tool | Description |
|------|-------------|
| `mcp__gitlab__list_merge_requests` | List open MRs for the project |
| `mcp__gitlab__get_merge_request` | Fetch full MR details, description, diff |
| `mcp__gitlab__create_merge_request` | Open a new MR from a branch |
| `mcp__gitlab__update_merge_request` | Update title, description, labels, assignees |
| `mcp__gitlab__list_issues` | List issues by project, label, status |
| `mcp__gitlab__get_issue` | Fetch full issue with comments |
| `mcp__gitlab__create_issue` | Create a new issue |
| `mcp__gitlab__list_pipelines` | List CI/CD pipelines for a ref |
| `mcp__gitlab__get_pipeline` | Get pipeline status and job details |
| `mcp__gitlab__list_branches` | List branches |

---

## How the Framework Uses GitLab

**Deployment Agent** — creates MRs via `mcp__gitlab__create_merge_request`:
- Source branch: `feat/TICKET-ID-description`
- Target: `main` (or configured `defaultBranch`)
- Title, description, labels populated automatically

**Review Agent** — reads existing MR diffs via `mcp__gitlab__get_merge_request`

**Maintenance Agent** — checks pipeline status via `mcp__gitlab__list_pipelines` when monitoring health

---

## GitLab vs GitHub Feature Map

| Feature | GitHub MCP | GitLab MCP |
|---------|-----------|-----------|
| Open PR/MR | `create_pull_request` | `create_merge_request` |
| Review | `create_pull_request_review` | MR notes (comments) |
| Pipelines | GitHub Actions (via gh CLI) | `list_pipelines` / `get_pipeline` |
| Issue tracking | `create_issue`, `get_issue` | `create_issue`, `get_issue` |

---

## Troubleshooting

**401 Unauthorized:**
- Verify `GITLAB_PERSONAL_ACCESS_TOKEN` is set and not expired
- Check token has `api` scope (not just `read_repository`)

**404 Not Found:**
- Verify `GITLAB_API_URL` — for self-hosted, it must include `/api/v4` suffix
- Confirm `namespace/repo` path is correct (use the full path from your GitLab URL)

**Pipeline not found:**
- The MCP queries by project ID or namespace/path — confirm `integrations.yaml` has the correct `repo` field
