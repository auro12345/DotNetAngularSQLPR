Run the Architecture & Design phase for the given ticket.

Ticket: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. **Check requirements doc exists**: look for `docs/designs/$ARGUMENTS-requirements.md` on disk.
   - If **NOT found**: STOP. Print: "No requirements doc found for $ARGUMENTS. Run `/requirements $ARGUMENTS` first, then re-run `/design $ARGUMENTS`." Do NOT proceed.
   - If **found**: load it and verify it contains at least one `$ARGUMENTS-AC-N` ID before continuing.
3. Load `config/learnings.yaml` for any `agentHints.architecture` entries that apply to this design.

## Execution

Follow `agentic-pdlc-framework/agents/03-architecture-agent.md` fully.

### Step 1: Load Context
- Read `config/project.yaml` for: tech stack, patterns, apiStyle, databases, codingStandards
- Scan the relevant source files in the codebase to understand existing patterns
- Identify files that will be created, modified, or deleted

### Step 2: Solution Design

#### Approach
Present 1–2 viable approaches with pros/cons if meaningful tradeoffs exist. State the selected approach and rationale.

#### File Impact Map
```
Files to CREATE:
  [path/file.ext]  — [purpose]

Files to MODIFY:
  [path/file.ext]  — [what changes]

Database migrations:
  [migration description, if any]

New dependencies:
  [package@version] — [why needed]
```

#### Diagrams
Produce relevant Mermaid diagrams:
- **Sequence diagram** for multi-step flows or async interactions
- **Component diagram** for new service relationships
- **Flowchart** for complex branching logic

#### API Contract (if new endpoints)
Document each new endpoint: method, path, request/response shapes, status codes.

### Step 3: Design Validation
- [ ] Follows patterns listed in `config/project.yaml > codingStandards.patterns`
- [ ] No unnecessary new dependencies
- [ ] All acceptance criteria from requirements are traceable to design decisions
- [ ] Security considerations noted
- [ ] Performance implications noted

### Step 4: Save Design Document
Save the complete design to: `docs/designs/$ARGUMENTS-[description].md`

### Output
```
✅ Design complete for $ARGUMENTS
Design saved: docs/designs/$ARGUMENTS-[description].md
Files to create: [N] | Files to modify: [N]
Next step: /implement $ARGUMENTS
```
