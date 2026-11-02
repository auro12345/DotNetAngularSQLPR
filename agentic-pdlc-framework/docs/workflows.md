# Workflows

Step-by-step walkthroughs for the most common PDLC scenarios.

## Workflow 1: Full Feature Development (Ticket to PR)

**Scenario:** You have a Jira ticket PROJ-123 for a new feature.

```
Prompt: "Start work on PROJ-123"
```

### What Happens

**Step 1 — Discovery** (automatic)
```
Agent: Project Discovery
Output: Loads config/project.yaml, confirms tech stack
        "Project: my-app | Stack: TypeScript/Next.js | Tests: Vitest | CI: GitHub Actions"
```

**Step 2 — Requirements**
```
Agent: Requirements
Output: 
  Requirement: User Login with Email + Password
  Source: PROJ-123
  Functional: [FR1, FR2, FR3]
  AC: [Given/When/Then statements]
  Clarifications needed: 2 questions
```

**Step 3 — Clarification (if needed)**
```
You: Answer the questions
Agent: Requirements agent incorporates answers
```

**Step 4 — Design**
```
Prompt: "Design the solution"
Agent: Architecture
Output: 
  - docs/designs/PROJ-123-user-login.md created
  - Sequence diagram
  - File impact map
  - API contract
```

**Step 5 — Implementation**
```
Prompt: "Implement the solution"
Agent: Development
Phase A: Skeleton created (all files, no logic yet)
Phase B: Tests written (all failing — expected)
Phase C: Implementation (one method at a time)
Phase D: Static analysis (lint + typecheck)
Phase E: All tests passing
```

**Step 6 — Review**
```
Prompt: "Review the changes"
Agent: Review
Output:
  - 0 BLOCKERs
  - 1 WARNING (function 12 lines, threshold 10)
  - Coverage: 91%
  - Verdict: READY FOR PR (after fixing warning)
```

**Step 7 — Fix Warning + PR**
```
[Development Agent fixes the 12-line function]
Prompt: "Create the PR"
Agent: Deployment
Output:
  - Branch: feat/PROJ-123-user-login
  - Commit: "feat(auth): add email+password user login"
  - PR created: https://github.com/org/repo/pull/42
```

---

## Workflow 2: Bug Fix

**Scenario:** Production bug where login fails for email addresses with uppercase letters.

```
Prompt: "Fix the bug in PROJ-456: login fails for uppercase email addresses"
```

### What Happens

**Step 1 — Requirements**
```
Agent: Requirements
Output:
  Bug: Login fails when email contains uppercase letters
  Root symptom: POST /auth/login with "User@example.com" returns 401
  Environment: All environments
  Severity: P2 (workaround: tell users to type lowercase)
  AC: Email comparison should be case-insensitive
```

**Step 2 — Root Cause**
```
Agent: Maintenance (triage)
Output:
  Root cause: UserRepository.findByEmail() uses exact match SQL
  File: src/users/user.repository.ts:34
  Introduced: commit abc1234 on 2024-01-01
  Fix: Normalize email to lowercase before comparison (in service layer)
```

**Step 3 — Regression Test First**
```
Agent: Testing
Creates: login_withUppercaseEmail_succeeds → FAILS (proves bug exists)
```

**Step 4 — Fix**
```
Agent: Development
Fix: AuthService.login() normalizes email to lowercase before lookup
Confirms: regression test now passes
Confirms: all existing tests still pass
```

**Step 5 — PR**
```
Agent: Deployment
Branch: fix/PROJ-456-case-insensitive-email
Commit: "fix(auth): normalize email to lowercase before lookup"
PR: https://github.com/org/repo/pull/43
```

---

## Workflow 3: Dependency Security Patch

**Scenario:** `npm audit` reports a high severity CVE in `express`.

```
Prompt: "There's a security vulnerability in express — handle it"
```

### What Happens

**Step 1 — Audit**
```
Agent: Maintenance
Output:
  express@4.18.2 — CVE-2024-12345 — CVSS 8.1 (HIGH)
  Description: Path traversal in static file serving
  Fixed in: express@4.19.1
  Affected code: Static file middleware (if used)
```

**Step 2 — Impact Assessment**
```
Agent: Maintenance
Analysis:
  - Does project use express.static()? → YES, in app.ts:15
  - Is it serving user-controlled paths? → NO, fixed path
  - Risk in our context: LOW (but still patch)
```

**Step 3 — Patch**
```
Agent: Development
Command: npm install express@4.19.1
Verify: npm audit shows 0 HIGH/CRITICAL remaining
Run: npm test → all passing
```

**Step 4 — PR**
```
Agent: Deployment
Branch: chore/PROJ-SEC-001-patch-express-cve
Commit: "chore(deps): patch express CVE-2024-12345

Upgrade express 4.18.2 → 4.19.1.
Fixes path traversal vulnerability in static file serving.
Our usage (fixed paths only) had low exploitability but patching
as standard practice.

Resolves: PROJ-SEC-001"
```

---

## Workflow 4: Monthly Dependency Update

```
Prompt: "Run the monthly dependency update"
```

### What Happens

**Step 1 — Discovery**
```
Agent: Maintenance
Output:
  Patch updates: 18 packages (safe to batch)
  Minor updates: 6 packages (check changelogs)
  Major updates: 2 packages (react 18→19, typescript 5.1→5.4)
  Security issues: 1 (see Workflow 3)
```

**Step 2 — Batch Patch Updates**
```
Agent: Development
Command: npm update (within package.json ranges)
         or: npx npm-check-updates -u --target patch && npm install
Tests: All passing after patch updates
```

**Step 3 — PR 1: Patch Updates**
```
PR: "chore(deps): batch patch dependency updates (January 2024)"
Body: Lists all updated packages with version ranges
```

**Step 4 — Minor Updates (after patch PR merges)**
```
Agent: Maintenance + Development
For each minor update:
  - Read changelog for breaking changes
  - Apply update
  - Run tests
  - Create PR
```

**Step 5 — Major Updates (separate tickets)**
```
For react 18→19:
  - Read React 19 migration guide
  - Identify breaking changes affecting this project
  - Create separate ticket PROJ-XX for migration
  - Do NOT rush into major updates — plan properly
```

---

## Workflow 5: Project Onboarding (New Developer Joins)

```
Prompt: "A new developer joined. Give them a project overview."
```

### What Happens

**Step 1 — Discovery Agent** summarizes config/project.yaml:
```
Project: my-app
Stack: TypeScript + Next.js 14 (App Router)
Domain: E-commerce SaaS
Tests: Vitest + Playwright | CI: GitHub Actions | Deploy: Vercel
Team patterns: Feature-based, Repository pattern, TanStack Query for data
```

**Step 2 — Architecture overview** from design docs:
```
Key areas: src/features/auth/, src/features/products/, src/features/orders/
API style: REST | DB: PostgreSQL + Redis | ORM: Prisma
```

**Step 3 — Setup instructions** from docs/getting-started.md

**Step 4 — First task** recommended via requirements agent

---

## Workflow 6: Performance Investigation

```
Prompt: "The /api/orders endpoint is slow (p95 > 2 seconds). Investigate."
```

### What Happens

**Step 1 — Triage**
```
Agent: Maintenance
Questions: Is this new (regression) or always been slow?
→ Check monitoring for when p95 latency increased
→ Check git log for recent changes to orders endpoint
```

**Step 2 — Profiling**
```
Agent: Development
Analysis: Check orders.repository.ts for N+1 queries
Found: Each order fetches items in a loop (N+1)
Fix: Use eager loading / join query
```

**Step 3 — Fix + Test**
```
Agent: Development + Testing
Fix: Rewrite query to use JOIN
Add: Performance test asserting p95 < 500ms
```

**Step 4 — PR**
```
perf(orders): fix N+1 query on order item loading

Replaced individual item fetches in loop with single JOIN query.
Reduces DB roundtrips from O(n) to O(1) per order list page.

Resolves: PROJ-789
```

---

## Workflow 7: End-of-Sprint Retrospective

**Scenario:** The sprint just finished. Two PRs had the same BLOCKER, and the team wants the agent to catch it automatically next time.

```
Prompt: "Run the sprint retro"
```

### What Happens

**Step 1 — Analysis**
```
Agent: Framework Evolution
Analyzing: 8 PRs from the last 2 weeks

Found patterns:
  [BLOCKER x3] Direct Prisma usage in controllers (PR #41, #44, #47)
  [Repeated Q ] "Does this need a migration?" asked in 3 tickets
  [Coverage gap] src/notifications/ at 12% for 4th consecutive sprint
```

**Step 2 — Proposals**
```
PROPOSED UPDATES:

[1] patterns.avoid (blocker)
    "Don't import PrismaClient in controllers — use service layer"
    → Review Agent will flag this as BLOCKER in all future PRs
    Apply? y

[2] agentHints.requirements
    "Always check if ticket touches DB schema — if yes, confirm migration
    strategy before writing ACs"
    → Requirements Agent will check this proactively
    Apply? y

[3] techDebt (critical)
    "src/notifications/ has 12% coverage and no improvement trend"
    Apply? y
```

**Step 3 — Applied**
```
✅ 3 learnings added to config/learnings.yaml

Next sprint: the BLOCKER pattern is enforced automatically.
No human needs to catch it in review.
```

---

## Workflow 8: Capturing a Postmortem

**Scenario:** A production incident just happened. JWT secret was rotated without updating dependent services.

```
Prompt: "/learn 'After the JWT incident: any auth change must include a cross-service impact check. Specifically, verify which services consume our JWT before deploying'"
```

### What Happens

```
Agent: Framework Evolution

I'll add the following to config/learnings.yaml:

SECTION: agentHints.review
  "Flag any change to JWT secret handling or auth token format.
  Before merging, verify all services that consume JWT are
  updated in the same deployment window."

SECTION: postmortems
  id: PM-001
  incident: "Auth failure after JWT secret rotation"
  learning: "Cross-service JWT consumer check required before auth deploys"

SECTION: process.requireHumanReviewForPaths
  "src/auth/**"

Apply? y

✅ Learning captured.

Effect:
  Review Agent will flag auth changes with a cross-service impact reminder
  Maintenance Agent will reference this incident when triaging auth bugs
  src/auth/** changes will require human review alongside AI review
```

---

## Tips for Effective Use

### Do
- Provide ticket IDs when available — agents fetch full context
- Let requirements agent finish before moving to design
- Review the design doc before saying "implement"
- Trust the skeleton phase — don't skip it

### Don't
- Don't ask agents to skip phases (e.g., "skip tests and just implement")
- Don't override `config/project.yaml` tech fields manually (let discovery handle it)
- Don't create PRs until Review Agent gives approval
- Don't batch major version updates with routine patch updates
