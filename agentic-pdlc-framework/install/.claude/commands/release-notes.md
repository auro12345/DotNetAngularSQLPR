Generate release notes for a version or PR and publish them to Confluence.

Input: $ARGUMENTS
- Version tag (e.g., `v1.2.3`) → generates full release notes for that version
- Ticket ID (e.g., `PROJ-123`) → generates release notes for a single PR/feature
- PR number (e.g., `#42`) → generates release notes for that specific PR

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Load from config:
   - `integrations.jira.baseUrl`, `integrations.jira.projectKey`
   - `integrations.confluence.*` (space key, parent page ID for release notes)
   - `integrations.git.provider`, `integrations.git.owner`, `integrations.git.repo`
3. Verify Confluence MCP is available (check `mcp__confluence__*` tools).

## Execution

Follow `agentic-pdlc-framework/agents/09-confluence-docs-agent.md`.

### Step 1: Gather Change Information

**If version tag (v1.2.3):**
```bash
git log [previous-tag]...[current-tag] --oneline  # all commits in release
git diff [previous-tag]...[current-tag] --stat     # files changed
```
Use Jira MCP to fetch all tickets referenced in commit messages.

**If ticket ID:**
- Fetch full ticket from Jira MCP: $ARGUMENTS
- Get all commits with this ticket ID in their message
- Get the PR (via GitHub MCP) for this ticket

**If PR number:**
- Use GitHub MCP to fetch PR details, commits, and changed files
- Extract ticket IDs from PR description and commit messages
- Fetch those tickets from Jira MCP

### Step 2: Categorize Changes
Group all changes into:
- 🚀 **New Features** — `feat:` commits or Feature-type Jira tickets
- 🐛 **Bug Fixes** — `fix:` commits or Bug-type Jira tickets
- ⚡ **Performance Improvements** — `perf:` commits
- 🔒 **Security Fixes** — security-labeled issues
- 📚 **Documentation** — `docs:` commits
- 🔧 **Maintenance** — `chore:`, dependency updates
- ⚠️ **Breaking Changes** — any commit with `Breaking-Change:` footer

### Step 3: Generate Release Notes Document

Structure:
```markdown
# Release Notes — [Version or Feature Name]
**Date:** [release date]
**Version:** [vX.Y.Z or feature name]
**Deployed to:** [environment]

## What's New
### 🚀 Features
- **[Feature Name]** ([TICKET-ID]): [user-facing description in plain English]

### 🐛 Bug Fixes
- **[Bug summary]** ([TICKET-ID]): [what was fixed, who was affected]

### ⚡ Performance
- [description]

### 🔒 Security
- [description — avoid disclosing exploit details]

### ⚠️ Breaking Changes
- [what changed, migration steps required]

## Technical Summary (for developers)
| Change | Files | PR |
|--------|-------|-----|
| [change] | [files] | [PR #link] |

## Upgrade Instructions
[If breaking changes: step-by-step migration guide]
[If none: "No action required — update is backwards-compatible"]

## Known Issues
[Any known issues or limitations introduced in this release]
```

### Step 4: Publish to Confluence
Use Confluence MCP to:
1. Navigate to the release notes space (from `config/project.yaml > integrations.confluence.releaseNotesSpace`)
2. Check if a page for this version already exists
3. If yes: update the existing page
4. If no: create a new page under the release notes parent page

Confluence page title: `Release Notes — [Version] — [YYYY-MM-DD]`

Also update the "Release History" index page with a row linking to this release.

### Step 5: Update CHANGELOG.md
Append to the top of `CHANGELOG.md` following Keep a Changelog format:
```markdown
## [vX.Y.Z] — YYYY-MM-DD
### Added
- ...
### Fixed
- ...
### Changed
- ...
```

### Step 6: Notify (if Slack configured)
If `integrations.notifications.channel` is set in config, post a Slack message:
```
🚀 Release [version] deployed to production
[2-3 bullet highlights]
Full notes: [Confluence link]
```

### Output
```
✅ Release notes published
Version:    $ARGUMENTS
Confluence: [page URL]
CHANGELOG:  Updated locally
Slack:      [sent / skipped — not configured]
```
