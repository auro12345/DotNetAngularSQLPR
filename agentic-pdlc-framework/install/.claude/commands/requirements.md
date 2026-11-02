Run the Requirements phase for the given ticket. Also used to resume after the team has answered clarification questions in Jira.

Arguments: $ARGUMENTS
- `PROJ-123` — fresh run or resume (auto-detected)
- `PROJ-123 --autonomous` — skip clarification loop, make assumptions instead

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first.
2. If no ticket ID in $ARGUMENTS, ask: "Which ticket? (e.g., PROJ-123)"
3. Check if `docs/designs/$TICKET-clarification.md` exists:
   - **Yes** → **Resume mode**: jump to Step 4 (fetch new Jira comments)
   - **No** → **Fresh mode**: run full analysis from Step 1

---

## Fresh Mode — Full Requirements Analysis

Follow `agentic-pdlc-framework/agents/02-requirements-agent.md` Steps 1–7 fully.

### Step 1: Load integration config
Read `config/integrations.yaml` for Jira settings and external source routing.

### Step 2: Fetch the ticket
```
mcp__jira__get_issue($TICKET)
→ description, ACs, remoteLinks, attachments, comments, linked issues
```

### Step 3: Analyze all sources
- Figma links → extract design spec (if `design.figma.enabled: true`)
- Confluence links → fetch page content
- Attachments → describe images, extract PDF text
- Linked Jira tickets → fetch as dependency context
- Other URLs → HTTP fetch

### Step 4 (Fresh): Structure requirements document
Save to `docs/designs/$TICKET-requirements.md` with:
- Summary, Functional Requirements, Non-Functional Requirements
- Acceptance Criteria (each with `$TICKET-AC-N` ID)
- Empty Traceability Matrix
- Ambiguities section

### Step 5: Clarification — post to Jira or proceed

**If NO ambiguities** → skip to Readiness Check below.

**If ambiguities exist AND NOT --autonomous**:
1. Post all questions as a single Jira comment (format: `[Q1]`, `[Q2]`... grouped by category)
2. Save `docs/designs/$TICKET-clarification.md` with question list and timestamp
3. Print pause message and **stop**:

```
⏸  REQUIREMENTS PAUSED — $TICKET
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
[N] questions posted to Jira. Waiting for team responses.

Questions:
  [Q1] [question]
  [Q2] [question]

→ Answer in Jira, then re-run: /requirements $TICKET
```

**If --autonomous**:
- Document each as `ASSUMPTION: [what] — [reason]`
- Post Jira comment listing all assumptions
- Proceed to Readiness Check

---

## Resume Mode — Pick Up Jira Responses

Triggered when `docs/designs/$TICKET-clarification.md` already exists.

### Step R1: Load clarification state
Read `docs/designs/$TICKET-clarification.md`:
- Pending question IDs
- Timestamp of last Jira comment posted

### Step R2: Fetch new Jira comments
```
mcp__jira__get_issue($TICKET)
→ filter comments created after saved timestamp
→ also check if ticket description was updated
```

### Step R3: Match answers to questions
For each pending `[Qn]`:
- Scan new comments for explicit or implicit answers
- Mark ✅ Answered with extracted answer text and commenter
- Keep ⏳ Pending if not addressed

Update `docs/designs/$TICKET-clarification.md` with answers.

### Step R4: Decide next action

**All questions answered**:
- Incorporate answers into `docs/designs/$TICKET-requirements.md`
- Move `## Ambiguities` to `## Resolved Clarifications` with answers recorded
- Continue to Readiness Check

**Some questions still pending**:
```
⏸  STILL WAITING — $TICKET
━━━━━━━━━━━━━━━━━━━━━━━━━━━
Received answers for [N]/[total] questions. Still pending:
  [Q2] [question text]
  [Q5] [question text]

→ Re-run: /requirements $TICKET after those are answered in Jira.
```
Stop.

**Answers introduce new ambiguities**:
- Collect follow-up questions
- Post Round N+1 Jira comment with new `[Qn]` IDs
- Update clarification state file
- Stop and print pause message

---

## Readiness Check

Before finishing (runs after all ambiguities are resolved):
- [ ] All ACs are testable and have `$TICKET-AC-N` IDs
- [ ] Traceability Matrix is present (test columns = Pending)
- [ ] All dependencies identified with status
- [ ] Non-functional requirements captured
- [ ] No open `[Qn]` questions

## Output

```
✅ Requirements complete for $TICKET
Clarification rounds: [N] (all resolved via Jira)
Ready for design: YES
Next step: /design $TICKET
```

or if still blocked:

```
⏸  Requirements paused for $TICKET
Waiting on: [N] unanswered questions in Jira
Re-run this command after the team responds.
```
