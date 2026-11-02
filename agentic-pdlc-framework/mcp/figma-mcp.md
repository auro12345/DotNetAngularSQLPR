# Figma MCP Configuration

## Purpose
Allows Requirements Agent to read Figma design files linked in Jira tickets — extracting components, text content, color tokens, typography, spacing, prototype flows, and designer annotations.

## Package
`figma-mcp` — community MCP server for Figma API access.

Alternative: `@modelcontextprotocol/server-figma` (if/when official package is published).

## Setup

### 1. Get Figma Personal Access Token
1. Go to https://www.figma.com/settings (or figma.com → click avatar → Settings)
2. Scroll to "Personal access tokens"
3. Click "Generate new token"
4. Give it a descriptive name: "PDLC Framework"
5. Select scopes:
   - **File content** — read (required)
   - **Dev resources** — read (for code snippets if Code Connect is set up)
   - **Comments** — read (for designer annotations)
6. Copy the token (shown only once)

### 2. Set Environment Variable
```bash
export FIGMA_ACCESS_TOKEN="figd_xxxxxxxxxxxxxxxxxxxx"
```

### 3. MCP Config (add to .claude/settings.json)
```json
"figma": {
  "command": "npx",
  "args": ["-y", "figma-mcp"],
  "env": {
    "FIGMA_ACCESS_TOKEN": "${FIGMA_ACCESS_TOKEN}"
  }
}
```

### 4. Update config/integrations.yaml
```yaml
design:
  figma:
    enabled: true
    teamId: null        # Optional: your Figma team ID
```

## Figma URL Formats

The Requirements Agent detects and parses these Figma URL patterns:

| URL Pattern | Type | Key Location |
|------------|------|--------------|
| `figma.com/file/{KEY}/...` | Design file | `{KEY}` |
| `figma.com/design/{KEY}/...` | Design file (new URL) | `{KEY}` |
| `figma.com/proto/{KEY}/...` | Prototype | `{KEY}` |
| `figma.com/board/{KEY}/...` | FigJam board | `{KEY}` |

With optional node selection:
- `?node-id=123:456` → specific frame or component
- `?node-id=123-456` → same (hyphen or colon format both work)

**Parsing logic:**
```
URL: https://www.figma.com/design/ABC123def456/Feature-Login?node-id=10:24

fileKey  = "ABC123def456"
nodeId   = "10:24"    (convert hyphen to colon if needed)
title    = "Feature-Login"
```

## Available MCP Tools

### Read File Structure
```
mcp__figma__get_file(file_key, node_id?)
  → Complete file structure: pages, frames, components
  → If node_id provided: returns only that subtree

mcp__figma__get_file_nodes(file_key, node_ids[])
  → Specific nodes by ID — use to get individual components
```

### Extract Design Tokens
```
mcp__figma__get_file_styles(file_key)
  → All defined styles: colors, text, effects, grids
  → Returns name, description, and key for each style

mcp__figma__get_style(style_key)
  → Details of a specific style (color value, font properties)
```

### Export Frames as Images
```
mcp__figma__get_image(file_key, node_ids[], scale?, format?)
  → Returns image URLs for specified nodes
  → format: "png" | "jpg" | "svg" | "pdf"
  → scale: 1 | 2 | 3 (for retina)
  → Use these URLs to download and embed in requirements doc
```

### Comments (Designer Annotations)
```
mcp__figma__get_comments(file_key)
  → All comments on the file
  → Filter by: client_meta.node_id to get comments on specific frames
  → Each comment: message, author, created_at, resolved
```

### Components
```
mcp__figma__get_file_components(file_key)
  → All components with name, description, key, containing_frame
  → Tells you what reusable components are available/expected
```

### Prototype Flows
```
mcp__figma__get_file_component_sets(file_key)
  → Component variants (states, sizes, themes)

# Prototype interactions are in the node data:
# node.interactions[].trigger.type  = "ON_CLICK" | "ON_HOVER" | "AFTER_DELAY"
# node.interactions[].action.type   = "NAVIGATE" | "SCROLL_TO" | "OPEN_OVERLAY"
```

## What Requirements Agent Extracts

When a Figma link is found in a ticket, the agent extracts and structures:

### 1. Visual Overview
Downloads and embeds the frame as an image. Shows designers' intent at a glance.

### 2. UI Components Inventory
```markdown
## Components in this design
| Component | Figma Name | State/Variant | Notes |
|-----------|------------|---------------|-------|
| Primary Button | Button/Primary | Default, Hover, Disabled, Loading | CTA actions |
| Text Input | Input/Text | Default, Focus, Error, Filled | Form fields |
| Error Banner | Alert/Error | - | Shown on login failure |
```

### 3. Text Strings
```markdown
## UI Text Content (from design)
| Element | Text | Notes |
|---------|------|-------|
| Page title | "Sign in to your account" | H1 |
| Email label | "Email address" | - |
| Email placeholder | "you@example.com" | - |
| Password label | "Password" | - |
| Submit button | "Sign in" | - |
| Error message | "Invalid email or password" | Shown on failed login |
| Forgot link | "Forgot your password?" | Links to /reset-password |
```

### 4. Design Tokens Used
```markdown
## Colors (from design)
- Primary action: #0066FF (brand.blue.600)
- Error state: #DC2626 (semantic.error)
- Background: #FFFFFF
- Border: #E5E7EB (gray.200)

## Typography
- Heading: Inter 24px/32px Bold
- Label: Inter 14px/20px Medium
- Input text: Inter 16px/24px Regular
- Error text: Inter 14px/20px Regular

## Spacing
- Form padding: 24px
- Field gap: 16px
- Button height: 48px
```

### 5. Interaction Flows
```markdown
## Prototype Interactions
1. Click "Sign in" → validates form → shows loading state → navigate to /dashboard
2. Click "Sign in" with invalid creds → shows error banner (stays on page)
3. Click "Forgot your password?" → navigate to /reset-password
4. Form field focus → shows focus ring (border color changes to #0066FF)
5. Hover on submit button → background darkens (#0052CC)
```

### 6. Designer Notes/Annotations
All comments on the frame that haven't been resolved.

### 7. Responsive/State Variants
```markdown
## States/Variants
| State | What changes |
|-------|-------------|
| Default | Empty form, no errors |
| Loading | Submit button shows spinner, disabled |
| Error | Red border on failed fields, error banner visible |
| Mobile (375px) | Single column, full-width button |
```

## Integration in Requirements Document

The extracted Figma data is added as a section in the requirements document:

```markdown
## Design Specification
**Figma Link**: [Feature Login](https://figma.com/design/ABC123/Feature-Login?node-id=10:24)
**Frame**: Login Screen — Desktop (1440px) + Mobile (375px)

### Visual Reference
[embedded frame export image]

### UI Components Required
[components table]

### UI Text Strings
[text table — use these exact strings in implementation]

### Interaction Flows
[prototype flows]

### Design Notes from Designer
> "The error message should appear below the form, not replace it. Keep both visible."
> "Loading state must disable the button to prevent double-submission." — @designer
```

## Fallback: No Figma MCP Available

If the Figma MCP is not configured but a Figma URL is present in the ticket:

1. **Record the URL** in the requirements doc as a link
2. **Flag it as a gap**: `⚠️ DESIGN: Figma link found but Figma MCP not configured — review manually`
3. **Extract file key from URL** and include direct API URL for manual access
4. **Ask the user**: "A Figma design is linked ([url]). Should I proceed without design specs, or can you share key design decisions?"
