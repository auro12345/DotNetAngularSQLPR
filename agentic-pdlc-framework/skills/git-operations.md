# Skill: Git Operations

## Purpose
Provides standardized git operations for branching, committing, pushing, and PR management. Used by Development Agent and Deployment Agent.

## Configuration (from project.yaml)
```yaml
integrations:
  git:
    provider: github  # github | gitlab | bitbucket | azure-devops
    owner: your-org
    repo: your-repo
    defaultBranch: main  # main | master | develop
    requireLinearHistory: true
    requireSignedCommits: false
```

## Operations

### Branch Management
```bash
# Create feature branch from default branch
git checkout [defaultBranch] && git pull origin [defaultBranch]
git checkout -b feat/TICKET-ID-description

# Create hotfix branch from production tag
git checkout -b hotfix/TICKET-ID-description vX.Y.Z

# List branches matching pattern
git branch --list "feat/*"

# Delete local branch after PR merge
git branch -d feat/TICKET-ID-description
```

### Staging Changes (safe)
```bash
# ALWAYS stage specific files — never git add -A
git add src/feature/newfile.ts src/feature/modified.ts

# Review what's staged before committing
git diff --staged

# Check for accidentally staged secrets
git diff --staged | grep -iE "(api_key|secret|password|token|credential)" --color
```

### Commit
```bash
# Conventional commit format
git commit -m "$(cat <<'EOF'
feat(scope): subject line

Body explaining what and why.

Implements: TICKET-ID
Tests: path/to/test.ts — coverage summary
Breaking-Change: none
EOF
)"
```

### Push
```bash
git push origin feat/TICKET-ID-description
# First push: -u to set upstream
git push -u origin feat/TICKET-ID-description
```

### Pull Request (GitHub CLI)
```bash
gh pr create \
  --title "feat(auth): add JWT authentication" \
  --body "$(cat docs/pr-body.md)" \
  --base main \
  --head feat/PROJ-123-auth \
  --draft  # open as draft while in review
```

### PR Status Check
```bash
# Check CI status
gh pr checks [PR_NUMBER]

# View PR details
gh pr view [PR_NUMBER]

# Merge PR
gh pr merge [PR_NUMBER] --squash --delete-branch
```

### Git History Queries
```bash
# Find commits that touched a file
git log --all --oneline -- src/auth/auth.service.ts

# Find when a bug was introduced (bisect)
git bisect start
git bisect bad HEAD
git bisect good v1.2.0
# Follow prompts...

# Show changes in last N commits
git log -n 10 --oneline

# Show diff between two branches
git diff main...feat/PROJ-123-auth

# Check if branch is up to date with main
git fetch origin main
git log HEAD..origin/main --oneline
```

### Tagging Releases
```bash
git tag -a v1.2.3 -m "Release v1.2.3

Features:
- PROJ-123: User authentication
- PROJ-124: Password reset

Bug fixes:
- PROJ-456: Fixed cart quantity validation"

git push origin v1.2.3
```

## Provider-Specific PR Creation

**GitHub (gh CLI):**
```bash
gh pr create --title "[title]" --body "[body]" --base main
```

**GitLab (glab CLI):**
```bash
glab mr create --title "[title]" --description "[body]" --target-branch main
```

**Bitbucket (bb CLI or curl):**
```bash
curl -X POST https://api.bitbucket.org/2.0/repositories/{owner}/{repo}/pullrequests \
  -H "Authorization: Bearer $BITBUCKET_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"[title]","source":{"branch":{"name":"feat/xxx"}},"destination":{"branch":{"name":"main"}}}'
```

## Environment Variables Required
```
# For GitHub
GITHUB_TOKEN=ghp_...
# or: use gh auth login

# For GitLab
GITLAB_TOKEN=glpat-...

# For Bitbucket
BITBUCKET_TOKEN=...
```
