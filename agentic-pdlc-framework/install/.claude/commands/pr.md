Create a commit and pull request for the current changes.

Ticket: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. **Verify branch**: run `git branch --show-current`. The current branch MUST contain the ticket ID (`$ARGUMENTS`). If you are on the default branch (main/master/develop), STOP: "You are on the default branch. The feature branch should have been created at the start of implementation. Check out your feature branch first: `git checkout feat/$ARGUMENTS-...`"
3. **Confirm review passed**: check if `/review` has been run and produced 0 BLOCKERs. If not, run `/review $ARGUMENTS` now. Do NOT create a PR if BLOCKERs exist.
4. Confirm all tests pass: `[commands.test from project.yaml]`
5. Load git config from `config/project.yaml > integrations.git`: provider, defaultBranch, mergeStrategy.

## Execution

Follow `agentic-pdlc-framework/agents/07-deployment-agent.md`.

### Step 1: Determine Commit Type
Infer commit type from the nature of the changes:
- New feature → `feat`
- Bug fix → `fix`
- Dependency update → `chore`
- Documentation → `docs`
- Refactor → `refactor`
- Performance → `perf`
- Tests → `test`

### Step 2: Stage Files
Stage ONLY the files relevant to this change. Never use `git add -A` or `git add .`.
Scan staged diff for secrets before committing: check for patterns matching `api_key`, `secret`, `password`, `token`.

### Step 3: Write Commit Message (Conventional Commits)
```
[type]([scope]): [imperative subject, max 72 chars]

[body: what changed and why, max 100 chars per line]

Implements: $ARGUMENTS
Acceptance: $ARGUMENTS-AC-1, $ARGUMENTS-AC-2
Tests: [test-file]:[AC-ID], [test-file]:[AC-ID]
Breaking-Change: none
```

### Step 4: Create Pull Request
Use `mcp__github__create_pull_request` (GitHub) or `mcp__gitlab__create_merge_request` (GitLab), or `gh pr create` / `glab mr create`.

PR title: same as commit subject
PR body must include:
```markdown
## Summary
[2-3 bullet points]

## Ticket
Implements: [$ARGUMENTS]([jira-link from config])

## AC Traceability
| AC ID | Summary | Test | Status |
|-------|---------|------|--------|
| $ARGUMENTS-AC-1 | [summary] | [file:test-name] | ✅ |
| $ARGUMENTS-AC-2 | [summary] | [file:test-name] | ✅ |

## Changes
[list changed files with descriptions]

## Test Plan
- [ ] All ACs have at least one test (see table above)
- [ ] All tests pass locally
- [ ] Coverage meets thresholds from config/project.yaml
- [ ] Edge cases covered: [list]

## Checklist
- [ ] Tests pass
- [ ] AC Traceability complete (no ❌ rows)
- [ ] No linting errors
- [ ] No type errors
- [ ] Docs updated (if needed)
- [ ] No secrets in code
```

### Step 5: Update Jira Ticket
Use `mcp__jira__transition_issue` to move $ARGUMENTS to "In Review".
Add a comment with the PR link via `mcp__jira__add_comment`.

### Output
```
✅ PR Created for $ARGUMENTS
Branch:   [branch-name]
PR URL:   [url]
Commit:   [hash] — [subject]
Ticket:   $ARGUMENTS → In Review
Next step: /release-notes $ARGUMENTS (after merge)
```
