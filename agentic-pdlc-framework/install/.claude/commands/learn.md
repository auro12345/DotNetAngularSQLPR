Capture a team learning and encode it into config/learnings.yaml so all agents benefit from it going forward.

Learning: $ARGUMENTS

## Precheck

1. Check if `config/project.yaml` exists. If missing, run project discovery first. Load it for project context (tech stack, standards) used to classify the learning accurately.
2. If $ARGUMENTS is empty, ask: "What did the team learn? Describe the pattern, insight, or rule."

## Execution

Follow `agentic-pdlc-framework/agents/10-framework-evolution-agent.md` — the `/learn` flow.

### Step 1: Load current state
Read `config/learnings.yaml` to understand what's already captured.

### Step 2: Classify the learning

Analyze $ARGUMENTS and determine which section(s) of `config/learnings.yaml` it belongs to:

| If the learning is about... | Section |
|-----------------------------|---------|
| A coding pattern to follow | `patterns.preferred` |
| A pattern/antipattern to avoid | `patterns.avoid` |
| Something requirements agent should know | `agentHints.requirements` |
| Something dev agent should know | `agentHints.development` |
| Something review agent should flag | `agentHints.review` + possibly `patterns.avoid` |
| Something testing agent should know | `agentHints.testing` |
| Adjusting a quality threshold | `reviewOverrides` |
| A workflow change | `process` |
| A known codebase problem | `techDebt` |
| A production incident lesson | `postmortems` |

### Step 3: Propose the entry

Show exactly what will be written before applying:

```
I'll add the following to config/learnings.yaml:

SECTION: [section name]
  [yaml entry, indented and formatted]

EFFECT: [one sentence explaining how agents will behave differently]

Apply? (yes / adjust / cancel)
```

### Step 4: Apply and confirm

On confirmation:
1. Write the entry to `config/learnings.yaml`
2. Update `lastUpdated` and `updatedBy` fields at the top
3. Print:

```
✅ Learning captured
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Section: [section]
Entry: [brief summary]

Effect going forward:
  [Which agent is affected and how it will behave differently]

Run /health to see all active learnings.
```

## Examples

```
/learn "Never call Prisma directly from controllers — use the service layer"
/learn "The payments module needs an explicit security review comment on every PR"  
/learn "Our Jira tickets always have incomplete ACs — also check linked Confluence pages"
/learn "Always run db:seed before integration tests"
/learn "The notifications module has no tests and it's getting worse — track as tech debt"
/learn "After the JWT incident, any auth change needs cross-service impact check"
```
