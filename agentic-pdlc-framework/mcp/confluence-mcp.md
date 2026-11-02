# Confluence MCP Configuration

## Package
`mcp-atlassian` — same package as Jira, covers both services.

## Setup

Same credentials as Jira (same Atlassian API token). See `mcp/jira-mcp.md` for token setup.

Additional environment variables:
```bash
export CONFLUENCE_API_TOKEN="your-api-token"   # same as JIRA_API_TOKEN usually
export CONFLUENCE_BASE_URL="https://your-org.atlassian.net/wiki"
export CONFLUENCE_USER_EMAIL="your-email@company.com"
```

## Finding Page IDs

The `config/project.yaml` needs page IDs for parent pages. Get them:
1. Open the page in Confluence
2. Click ••• (More actions) → Page Information
3. The page ID is in the URL: `/pages/viewinfo.action?pageId=123456`

Or via API:
```bash
# Find page ID by title
curl -H "Authorization: Basic $(echo -n $CONFLUENCE_USER_EMAIL:$CONFLUENCE_API_TOKEN | base64)" \
  "$CONFLUENCE_BASE_URL/rest/api/content?title=Release+Notes&spaceKey=PROJ&expand=version" | \
  jq '.results[0].id'
```

## Available MCP Tools

### Read
```
mcp__confluence__get_page(page_id)
  → Full page content and metadata

mcp__confluence__get_page_children(page_id)
  → List child pages

mcp__confluence__search(query, space_key, limit)
  → CQL search across Confluence spaces

mcp__confluence__get_space(space_key)
  → Space details and home page ID
```

### Write
```
mcp__confluence__create_page(space_key, title, content, parent_id)
  → Create new page, returns page ID and URL
  content format: Confluence Storage Format (XHTML) or Markdown

mcp__confluence__update_page(page_id, title, content, version)
  → Update page (version must be current version + 1)

mcp__confluence__add_label(page_id, labels)
  → Tag page with labels

mcp__confluence__create_comment(page_id, comment_text)
  → Add page comment
```

## Content Format

Confluence uses Storage Format (XHTML). When creating pages from Markdown:

**Headings:**
```html
<h1>Title</h1>
<h2>Section</h2>
```

**Tables:**
```html
<table>
  <tr><th>Column 1</th><th>Column 2</th></tr>
  <tr><td>Value 1</td><td>Value 2</td></tr>
</table>
```

**Code blocks:**
```html
<ac:structured-macro ac:name="code">
  <ac:parameter ac:name="language">javascript</ac:parameter>
  <ac:plain-text-body><![CDATA[const x = 1;]]></ac:plain-text-body>
</ac:structured-macro>
```

**Info panels:**
```html
<ac:structured-macro ac:name="info">
  <ac:rich-text-body><p>Important note here</p></ac:rich-text-body>
</ac:structured-macro>
```

**Mermaid diagrams** (if Mermaid macro is installed):
```html
<ac:structured-macro ac:name="mermaid">
  <ac:plain-text-body><![CDATA[
    sequenceDiagram
    Client->>Server: Request
  ]]></ac:plain-text-body>
</ac:structured-macro>
```

If Mermaid macro is not available, render as a code block or embed as an image.

## Page Structure for PDLC

### Release Notes Space
```
📁 Release Notes (parent: releaseNotesParent)
  ├── 📄 Release History (index table)
  ├── 📄 Release Notes — v1.2.3 — 2024-01-15
  ├── 📄 Release Notes — v1.2.2 — 2024-01-01
  └── ...
```

### Design Docs Space
```
📁 Technical Design Documents (parent: designDocsParent)
  ├── 📄 PROJ-123: User Authentication — Technical Design
  ├── 📄 PROJ-124: Checkout Flow — Technical Design
  └── ...
```

### API Documentation
```
📁 API Reference (parent: apiDocsParent)
  ├── 📄 API Overview
  ├── 📄 Authentication Endpoints
  ├── 📄 User Endpoints
  └── 📄 Order Endpoints
```

### ADRs (Architecture Decision Records)
```
📁 Architecture Decisions (parent: adrParent)
  ├── 📄 ADR-001: Use PostgreSQL as primary database
  ├── 📄 ADR-002: Adopt JWT for stateless auth
  └── ...
```

## Updating project.yaml for Confluence

```yaml
integrations:
  confluence:
    enabled: true
    baseUrl: "https://your-org.atlassian.net/wiki"
    spaceKey: "PROJ"
    releaseNotesParent: 123456    # get via API above
    designDocsParent: 123457
    apiDocsParent: 123458
    healthDashboardPage: 123459
    adrParent: 123460
```

## Version Tracking

When updating an existing Confluence page, always fetch the current version first:
```
1. mcp__confluence__get_page(page_id) → returns {version: {number: 5}}
2. mcp__confluence__update_page(page_id, title, content, version: 6)  ← must be +1
```
Missing the version increment causes a 409 Conflict error.
