# Skill: PR Management

## Purpose
Create, update, and manage pull requests across Git providers. Used by Deployment Agent.

## Provider Detection
Read from `config/project.yaml > integrations.git.provider`.
Supported: `github`, `gitlab`, `bitbucket`, `azure-devops`

## PR Creation

### GitHub (via gh CLI)
```bash
gh pr create \
  --title "feat(auth): add JWT authentication [PROJ-123]" \
  --body "$(cat /tmp/pr-body.md)" \
  --base main \
  --head feat/PROJ-123-auth \
  --label "feature,needs-review"

# Draft PR (not ready for review)
gh pr create ... --draft

# Add reviewers
gh pr create ... --reviewer username1,username2

# Assign to project
gh pr create ... --project "Sprint 42"
```

### GitLab (via glab CLI)
```bash
glab mr create \
  --title "feat(auth): add JWT authentication" \
  --description "$(cat /tmp/pr-body.md)" \
  --target-branch main \
  --source-branch feat/PROJ-123-auth \
  --label "feature" \
  --assignee username
```

### Bitbucket (via REST API)
```bash
curl -X POST \
  "https://api.bitbucket.org/2.0/repositories/{workspace}/{repo}/pullrequests" \
  -H "Authorization: Bearer $BITBUCKET_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "feat(auth): add JWT authentication",
    "description": "...",
    "source": {"branch": {"name": "feat/PROJ-123-auth"}},
    "destination": {"branch": {"name": "main"}},
    "reviewers": [{"uuid": "{reviewer-uuid}"}],
    "close_source_branch": true
  }'
```

## PR Body Generator

Auto-generates PR body from:
1. Commit messages on the branch
2. Requirements document
3. Test results
4. Changed file list

Template variables:
- `{TICKET_ID}` — from branch name
- `{TICKET_URL}` — from project.yaml jira.baseUrl
- `{SUMMARY_BULLETS}` — from commit messages
- `{CHANGED_FILES}` — from git diff --name-only
- `{TEST_RESULTS}` — from last test run
- `{COVERAGE}` — from coverage report

## PR Status Management

### Check CI status
```bash
# GitHub
gh pr checks [PR_NUMBER]
gh pr view [PR_NUMBER] --json statusCheckRollup

# Poll until all checks pass (max 30 minutes)
timeout 1800 bash -c '
  while ! gh pr checks [PR_NUMBER] | grep -q "All checks passed"; do
    sleep 30
  done
'
```

### Merge PR
```bash
# Squash merge (recommended for feature branches)
gh pr merge [PR_NUMBER] --squash --delete-branch --auto

# Rebase merge
gh pr merge [PR_NUMBER] --rebase --delete-branch

# Regular merge commit
gh pr merge [PR_NUMBER] --merge --delete-branch
```

Merge strategy from `config/project.yaml > integrations.git.mergeStrategy`:
- `squash` (default for feature branches)
- `rebase` (for linear history projects)
- `merge` (for release branches that need merge commits)

## PR Review Operations

### Request Review
```bash
gh pr edit [PR_NUMBER] --add-reviewer username1,username2
```

### Check Review Status
```bash
gh pr view [PR_NUMBER] --json reviews
```

### Respond to Review Comments
When review comments come in:
1. Read each comment carefully
2. Address BLOCKER comments with code changes
3. Respond to SUGGESTION comments with reasoning if not implementing
4. Re-request review after addressing all comments

## PR Cleanup

After merge:
```bash
# Delete remote branch
git push origin --delete feat/PROJ-123-auth

# Delete local branch
git branch -d feat/PROJ-123-auth

# Update local default branch
git checkout main && git pull origin main
```
