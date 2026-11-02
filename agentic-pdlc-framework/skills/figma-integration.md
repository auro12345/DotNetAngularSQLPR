# Skill: Figma Integration

## Purpose
Extracts design specifications, component inventories, text strings, tokens, interactions, and designer annotations from Figma files. Called by Requirements Agent when a Figma link is detected in a ticket.

## Activation Condition
This skill activates when ANY of the following contain a Figma URL:
- Jira ticket description
- Jira ticket comments
- Jira remote links (Web Links section)
- Jira attachments with `.fig` extension
- User's direct input

## URL Detection and Parsing

### Detect Figma URLs
```python
import re

FIGMA_URL_PATTERN = re.compile(
    r'https?://(?:www\.)?figma\.com/'
    r'(?:file|design|proto|board)/'
    r'([a-zA-Z0-9]+)'   # file key
    r'(?:/[^?#]*)?'     # optional title slug
    r'(?:\?[^#]*)?'     # optional query string
    r'(?:#.*)?'         # optional fragment
)

def parse_figma_url(url):
    match = FIGMA_URL_PATTERN.match(url)
    if not match:
        return None

    file_key = match.group(1)

    # Extract node_id from query string
    node_id = None
    if 'node-id=' in url:
        node_part = url.split('node-id=')[1].split('&')[0].split('#')[0]
        # Normalize: hyphens and colons are both used
        node_id = node_part.replace('-', ':')

    return {
        'fileKey': file_key,
        'nodeId': node_id,
        'isPrototype': '/proto/' in url,
        'isFigJam': '/board/' in url,
        'originalUrl': url
    }
```

### Detect URLs in Jira Ticket Fields
Check these locations (in order):
1. `description` — inline URLs and embedded links
2. `remoteLinks[]` — the "Web Links" section (external links added explicitly)
3. `comments[]` — URLs in comments
4. `attachments[]` — `.fig` files
5. Custom field values (check all string fields for figma.com pattern)

## Extraction Operations

### Step 1: Fetch File or Node
```
IF nodeId is present:
  → mcp__figma__get_file_nodes(fileKey, [nodeId])
  → Returns only the subtree rooted at that node (faster, more focused)

ELSE:
  → mcp__figma__get_file(fileKey)
  → Returns entire file — use first page's first frame if no node specified
```

### Step 2: Export Frame as Image
```
mcp__figma__get_image(fileKey, [nodeId or firstFrameId], scale=2, format="png")
→ Returns { [nodeId]: "https://figma-alpha-api.s3.us-west-2.amazonaws.com/..." }
→ Download this image and include it in the requirements document
→ If multiple viewports exist (mobile + desktop): export both
```

### Step 3: Extract Design Tokens
```
mcp__figma__get_file_styles(fileKey)
→ Returns all named styles used in the file

For each style referenced in the target frame:
  mcp__figma__get_style(styleKey)
→ Returns resolved values (hex colors, font stacks, etc.)
```

### Step 4: Extract Text Strings
Traverse the node tree looking for nodes with `type = "TEXT"`:
```json
{
  "type": "TEXT",
  "name": "Error message",
  "characters": "Invalid email or password",
  "style": { "fontFamily": "Inter", "fontSize": 14, "fontWeight": 400 }
}
```
Collect all unique text strings. Tag them by context (heading, label, placeholder, button, error, helper text).

### Step 5: Extract Component List
```
mcp__figma__get_file_components(fileKey)
→ All components; filter to those referenced in the target frame

For each component instance in the frame:
  - name: the component name
  - description: designer's notes
  - variants: list from componentSet if available
```

### Step 6: Extract Prototype Interactions
Traverse nodes looking for `interactions` array:
```json
{
  "interactions": [
    {
      "trigger": { "type": "ON_CLICK" },
      "action": {
        "type": "NAVIGATE",
        "destinationId": "frame-id-123",
        "transition": { "type": "DISSOLVE", "duration": 0.3 }
      }
    }
  ]
}
```

Map interaction types:
- `ON_CLICK` → user taps/clicks
- `ON_HOVER` → mouse hover
- `AFTER_DELAY` → auto-advance (e.g., splash screens)
- `ON_DRAG` → drag gesture
- `KEY_DOWN` → keyboard shortcut

Map action types:
- `NAVIGATE` → go to another frame (note destination frame name)
- `SCROLL_TO` → scroll to position
- `OPEN_OVERLAY` → modal/drawer opens
- `CLOSE_OVERLAY` → modal/drawer closes
- `SWAP_OVERLAY` → replace overlay with another

### Step 7: Extract Designer Comments
```
mcp__figma__get_comments(fileKey)
→ All comments; filter to:
   - Unresolved comments (resolved = false)
   - Comments whose client_meta.node_id matches our target frame

For each relevant comment:
  - message: the annotation text
  - user.name: designer's name
  - created_at: timestamp
```

## Output Structure

```markdown
## Design Specification
**Source**: Figma — [frame name]
**Link**: [original Figma URL]
**Extracted**: [timestamp]

### Visual Reference
![Design screenshot]([image URL])

### UI Components Required
| Component | Figma Name | Variants/States | Implementation Notes |
|-----------|------------|-----------------|---------------------|
| [name] | [figma name] | [Default, Hover, Error, Disabled] | [notes] |

### Text Strings (use exactly as shown)
| Location | String | Notes |
|----------|--------|-------|
| Page title | "[text]" | H1 element |
| [field] label | "[text]" | - |
| [field] placeholder | "[text]" | - |
| Error message | "[text]" | Shown when [condition] |
| Button | "[text]" | Primary CTA |

### Design Tokens
**Colors:**
| Usage | Value | Token Name |
|-------|-------|------------|
| Primary action | #XXXXXX | [token name] |
| Error | #XXXXXX | [token name] |

**Typography:**
| Element | Font | Size | Weight | Line Height |
|---------|------|------|--------|-------------|
| Heading | [font] | [px] | [weight] | [px] |
| Body | [font] | [px] | [weight] | [px] |

**Spacing:** [key measurements]

### Interaction Flows
1. [Trigger] → [Action] → [Destination/Effect]
2. ...

### Responsive Variants
| Breakpoint | Differences from desktop |
|------------|--------------------------|
| Mobile (375px) | [layout changes] |
| Tablet (768px) | [layout changes] |

### Designer Notes (unresolved annotations)
> "[comment text]" — [designer name]
> "[comment text]" — [designer name]
```

## Implications for Requirements

After extraction, the agent adds these to the functional requirements:
- Each text string → a string literal to implement (no paraphrasing)
- Each component variant → an AC (e.g., "Submit button shows spinner in loading state")
- Each interaction → an AC (e.g., "Invalid credentials shows error banner without page reload")
- Each designer note → a constraint or clarification on existing ACs

## Error Handling

| Error | Action |
|-------|--------|
| 403 Forbidden | Token lacks file access. Check: is the file shared with your Figma account? |
| 404 Not Found | File deleted or moved. Note in requirements, ask user. |
| Rate limit (429) | Wait 30s, retry once. If fails again, flag and continue. |
| MCP not configured | Flag as `⚠️ DESIGN GAP`, include URL in requirements for manual review. |
| Node ID not found | Fall back to fetching the entire file's first page. |
| No interactions found | Frame may be static mockup only — note in requirements. |
