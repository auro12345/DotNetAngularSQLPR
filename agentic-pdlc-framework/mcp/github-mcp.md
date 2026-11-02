# GitHub MCP Configuration

## Package
`@modelcontextprotocol/server-github` — official GitHub MCP from Anthropic/MCP.

## Setup

### 1. Create Personal Access Token
1. Go to https://github.com/settings/tokens
2. Click "Generate new token (classic)" or use Fine-grained tokens
3. Required scopes:
   - `repo` (full repository access)
   - `read:org` (read org membership)
   - `workflow` (to trigger/read Actions)
4. Copy the token

### 2. Set Environment Variable
```bash
export GITHUB_PERSONAL_ACCESS_TOKEN="ghp_xxxxxxxxxxxx"
```

### 3. MCP Config (already in .claude/settings.json)
```json
"github": {
  "command": "npx",
  "args": ["-y", "@modelcontextprotocol/server-github"],
  "env": {
    "GITHUB_PERSONAL_ACCESS_TOKEN": "${GITHUB_PERSONAL_ACCESS_TOKEN}"
  }
}
```

## Available MCP Tools

### Repository Operations
```
mcp__github__get_file_contents(owner, repo, path)
  → Read file from repo

mcp__github__search_code(query)
  → Search across repo code

mcp__github__list_commits(owner, repo, branch, since, until)
  → Commit history with messages and diffs
```

### Pull Request Operations
```
mcp__github__create_pull_request(owner, repo, title, body, head, base, draft)
  → Create PR, returns PR URL and number

mcp__github__get_pull_request(owner, repo, pull_number)
  → Full PR details: title, body, commits, status checks, reviews

mcp__github__list_pull_requests(owner, repo, state, base)
  → List PRs filtered by state/branch

mcp__github__merge_pull_request(owner, repo, pull_number, merge_method)
  → Merge PR (squash, rebase, or merge)

mcp__github__add_pull_request_review_comment(owner, repo, pull_number, body, path, line)
  → Add inline review comment

mcp__github__create_review(owner, repo, pull_number, event, body, comments)
  → Submit PR review (APPROVE, REQUEST_CHANGES, COMMENT)
```

### Issue Operations
```
mcp__github__create_issue(owner, repo, title, body, labels, assignees)
  → Create issue

mcp__github__update_issue(owner, repo, issue_number, state, labels)
  → Update issue state/labels
```

### Branch and Commit
```
mcp__github__create_branch(owner, repo, branch, sha)
  → Create branch from commit

mcp__github__create_or_update_file(owner, repo, path, message, content, branch)
  → Create/update a file in the repo

mcp__github__push_files(owner, repo, branch, files, commit_message)
  → Push multiple files in one commit
```

### GitHub Actions / CI
```
mcp__github__list_workflow_runs(owner, repo, workflow_id, branch, status)
  → Get CI run status

mcp__github__get_workflow_run(owner, repo, run_id)
  → Detailed run info including logs URL

mcp__github__trigger_workflow(owner, repo, workflow_id, ref, inputs)
  → Manually trigger a workflow
```

## How Agents Use GitHub MCP

### Deployment Agent (creating PR)
```
1. mcp__github__create_pull_request → opens PR with full template
2. Result includes PR number and URL → reported to user
3. mcp__github__list_workflow_runs → monitors CI status
```

### Review Agent (checking CI on existing PR)
```
1. mcp__github__get_pull_request → fetch PR details
2. Check PR.statusCheckRollup → all checks pass?
3. mcp__github__list_workflow_runs → get detailed CI status
```

### Confluence Docs Agent (release notes)
```
1. mcp__github__list_commits → get all commits since last tag
2. mcp__github__get_pull_request → get PR description, labels
3. Extract ticket IDs from commit messages
```

### Maintenance Agent (health check)
```
1. mcp__github__list_pull_requests(state="open") → count open PRs
2. Check PR ages for stale PRs
3. mcp__github__list_workflow_runs(status="failure") → failing CI
```

## GitLab Alternative

For GitLab projects, replace the GitHub MCP with:

```json
"gitlab": {
  "command": "npx",
  "args": ["-y", "mcp-gitlab"],
  "env": {
    "GITLAB_PERSONAL_ACCESS_TOKEN": "${GITLAB_PERSONAL_ACCESS_TOKEN}",
    "GITLAB_API_URL": "${GITLAB_API_URL}"
  }
}
```

MCP tool names use `mcp__gitlab__*` prefix with equivalent operations.

## Updating project.yaml

```yaml
integrations:
  git:
    provider: github          # github | gitlab | bitbucket
    owner: "your-org"
    repo: "your-repo"
    defaultBranch: main
    mergeStrategy: squash     # squash | rebase | merge
    requireLinearHistory: true
```
