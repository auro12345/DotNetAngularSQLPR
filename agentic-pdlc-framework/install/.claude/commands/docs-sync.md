Sync documentation to Confluence for a ticket or PR — update technical docs, API references, and ADRs.

Input: $ARGUMENTS
- Ticket ID (e.g., PROJ-123) — sync docs for this feature
- PR number (e.g., #42) — sync docs for this PR
- "all" — sync all local docs that are out of date with Confluence

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. Verify `integrations.confluence` fields are set in config.
3. Verify Confluence MCP is available.

## Execution

Follow `agentic-pdlc-framework/agents/09-confluence-docs-agent.md`.

### Step 1: Identify What Changed
```bash
# For a ticket/PR: get changed docs files
git diff main...HEAD --name-only -- "*.md" "docs/**" "openapi*" "swagger*"
```

Categorize changed documentation:
- **API docs** (`openapi.yaml`, `swagger.json`, `docs/API.md`)
- **Architecture docs** (`docs/ARCHITECTURE.md`, `docs/designs/*.md`)
- **Runbook / ops docs** (`docs/runbooks/**`)
- **ADRs** (`docs/adr/**`, `docs/decisions/**`)
- **User guides** (`docs/guides/**`)

### Step 2: Sync Each Doc Type

**API Documentation:**
- Read the updated OpenAPI/Swagger spec or API.md
- Find the corresponding Confluence page (search by title or stored ID)
- Update the page content with the new API documentation
- If page doesn't exist: create it under `integrations.confluence.apiDocsParent`

**Architecture Decision Records (ADRs):**
- Each ADR file → corresponding Confluence page
- Title format: `ADR-[NNN]: [Decision Title]`

**Design Documents:**
- Each `docs/designs/TICKET-ID-*.md` → Confluence page in design docs space
- Link back to the Jira ticket

**General Docs:**
- Map each `docs/*.md` file to its Confluence equivalent
- Sync content (Markdown → Confluence wiki format)

### Step 3: Verify Links
After syncing:
- Check all internal doc links are valid
- Update cross-references if page URLs changed
- Verify diagrams rendered correctly

### Step 4: Update Doc Index
Update the main documentation index page in Confluence to reflect any new pages created.

### Output
```
✅ Docs synced for $ARGUMENTS
Pages updated: [N]
Pages created: [N]
Pages skipped (unchanged): [N]

Updated:
  - [Confluence page title] → [URL]
  - ...
```
