# Skill: External Sources Fetcher

## Purpose
Fetches and analyzes all external content linked or attached in a Jira ticket (or any requirements source). Handles URLs, attachments, images, documents, and embedded links. Called by Requirements Agent after the ticket is fetched.

## What Counts as an External Source

From a Jira ticket:
1. **Description** — inline hyperlinks (`[text](url)`), bare URLs, image embeds
2. **Remote Links** (Web Links section) — explicitly added external links
3. **Attachments** — files uploaded to the ticket
4. **Comments** — URLs mentioned in discussion
5. **Linked issues** → fetch those tickets too (epic, blockers, relates-to)
6. **Custom fields** — any field containing a URL

## Source Detection Logic

### Step 1: Extract All URLs from Ticket
```python
import re

URL_PATTERN = re.compile(
    r'https?://[^\s\]\)\'"<>]+',
    re.IGNORECASE
)

def extract_urls_from_ticket(ticket):
    sources = []

    # From description (Atlassian Document Format — extract from text nodes)
    sources += extract_urls_from_adf(ticket.description)

    # From remote links
    for link in ticket.remoteLinks:
        sources.append({'url': link.url, 'title': link.title, 'source': 'web-link'})

    # From comments
    for comment in ticket.comments:
        for url in URL_PATTERN.findall(comment.body_text):
            sources.append({'url': url, 'source': f'comment by {comment.author}'})

    return deduplicate(sources)
```

### Step 2: Route Each URL to the Right Handler

Based on `config/integrations.yaml > externalSources.urlRouting`:

```
figma.com                    → Figma Integration skill
atlassian.net/wiki           → Confluence MCP
atlassian.net/jira           → Jira MCP (linked tickets)
github.com/*/issues/*        → GitHub MCP
github.com/*/pull/*          → GitHub MCP
github.com                   → GitHub MCP
gitlab.com                   → GitLab MCP
docs.google.com              → HTTP fetch (public) or flag if private
notion.so                    → HTTP fetch (public) or flag if private
Everything else              → HTTP fetch
```

## Handlers

### Handler A: Figma Links
Delegate to `skills/figma-integration.md`.
Returns: visual spec, components, text strings, interactions.

### Handler B: Confluence Pages
```
mcp__confluence__get_page(page_id extracted from URL)
→ Returns full page content and title
→ Summarize relevant sections for the requirements context
```

Extract Confluence page ID from URL:
- `/pages/{pageId}/` or `/pages/viewpage.action?pageId={pageId}`
- `/display/{spaceKey}/{pageTitle}` → search by title to get ID

### Handler C: GitHub Issues / PRs
```
# Issue
mcp__github__get_issue(owner, repo, issue_number)

# PR
mcp__github__get_pull_request(owner, repo, pull_number)
```
Summarize: what was the linked issue/PR about, what was its resolution?

### Handler D: General HTTP Fetch
For URLs not covered by an MCP (Google Docs with public access, specification websites, etc.):
```bash
curl -sL --max-time 10 --max-filesize 2097152 "[url]" \
  -H "User-Agent: Mozilla/5.0 (compatible; PDLC-Framework/1.0)" \
  -o /tmp/fetched-content.html
```
Then extract: title, main text content (strip HTML), any structured data.

For PDF URLs: note the URL, attempt to fetch and extract text via pdftotext or similar.

**Rate limiting**: max 1 request/second, max 10 external fetches per ticket.

### Handler E: Linked Jira Tickets
For `issuelinks[]` in the ticket:
```
For each linked issue (blockers, parent epics, related):
  mcp__jira__get_issue(linked_issue_key)
  → Extract: summary, description, status, acceptance criteria
  → Include as "Dependency Context" in requirements
```

## Attachment Handling

### Images (screenshots, wireframes, mockups)
For each image attachment (jpg, png, gif, webp, svg):
1. Download via Jira attachment API:
   ```
   GET {jira_base_url}/rest/api/3/attachment/content/{attachment_id}
   Authorization: Basic {credentials}
   ```
2. Describe the image visually: what UI is shown, what state, what data
3. Note as: "Attachment: [filename] — [visual description]"

Image interpretation guidance:
- Identify: is this a wireframe, mockup, screenshot, diagram, or flowchart?
- Wireframe → note the layout and component positions
- Mockup (high-fidelity) → treat similarly to Figma (extract components, text)
- Screenshot (existing UI) → note what the current state looks like (for "before")
- Diagram/flowchart → describe the flow it depicts

### PDF Attachments
Extract text content. Focus on:
- Requirements or specification sections
- Acceptance criteria tables
- API contracts or data models

### Text/Markdown/Word Attachments
Read content directly. Summarize relevant sections.

### `.fig` Files (Figma)
Note: "Figma file attached directly". Open via Figma desktop or import.
Cannot be parsed via API without uploading — flag for manual review.

## Output Structure

Add to requirements document:

```markdown
## External Sources Analyzed

### Figma Designs
[see Design Specification section above]

### Linked Confluence Pages
| Page | Summary | Relevant to |
|------|---------|-------------|
| [title](url) | [1-2 sentence summary] | [which FR/AC] |

### Linked GitHub Issues/PRs
| Link | Status | Summary |
|------|--------|---------|
| [#123](url) | Closed | [what was resolved] |

### Linked Tickets
| Ticket | Relationship | Status | Key Info |
|--------|-------------|--------|----------|
| [PROJ-100](url) | Blocks this | In Progress | [summary] |
| [PROJ-99](url) | Parent epic | Active | [epic context] |

### Attachments
| File | Type | Key Findings |
|------|------|-------------|
| [filename] | Screenshot | [description of UI state shown] |
| [filename] | PDF | [key requirements found] |

### Other External Links
| URL | Title | Relevant Content |
|-----|-------|-----------------|
| [url] | [page title] | [1-line summary] |

### ⚠️ Inaccessible Sources
| URL/File | Reason | Action Needed |
|---------|--------|---------------|
| [url] | Private — login required | Request access or share key content |
| [file.fig] | Figma MCP not configured | Configure in config/integrations.yaml |
```

## Access Error Handling

| Situation | What to Do |
|-----------|-----------|
| URL returns 401/403 | Mark as `⚠️ Private — needs access`. Ask user if they can share content. |
| URL returns 404 | Mark as `⚠️ Dead link`. Note in requirements, ask if there's a replacement. |
| URL times out | Skip and note as `⚠️ Unreachable`. |
| MCP not configured | Flag as gap, include URL for manual review. |
| Image description fails | Note filename and size, skip description. |
| Too many links (>10) | Process top 5 by priority (Figma first, Confluence second, GitHub third). |

## Priority Order

When multiple sources exist, process in this order:
1. Figma links → most direct design guidance for implementation
2. Confluence pages → functional specifications
3. Linked Jira tickets → dependency context
4. GitHub issues/PRs → technical context
5. Image attachments → visual context
6. PDF/document attachments → written specs
7. Other URLs → supplementary context
