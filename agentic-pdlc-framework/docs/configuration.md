# Configuration Reference

Complete reference for `config/project.yaml` — the single source of truth for all project settings.

## File Location

```
your-project/
└── config/
    └── project.yaml    ← active config
```

The config file lives in `config/project.yaml` relative to your project root (the directory that contains your source code, package.json, etc.).

## Field Reference

### `project.*` — Project Identity

| Field | Type | Auto-updated | Description |
|-------|------|--------------|-------------|
| `name` | string | NO | Project name (e.g., "my-app") |
| `description` | string | NO | 1-2 sentence description |
| `domain` | string | NO | Business domain (e.g., "e-commerce", "fintech") |
| `team` | string | NO | Team or squad name |
| `owner` | string | NO | Product owner or tech lead |
| `status` | enum | NO | `active` \| `maintenance` \| `deprecated` \| `archived` |
| `detectedAt` | timestamp | YES | Last time discovery agent scanned |

> **Protected:** `name`, `description`, `domain` will never be overwritten by the discovery agent.

---

### `tech.*` — Technology Stack

All `tech.*` fields are **auto-detected** by the Project Discovery Agent. You can override any field manually.

| Field | Auto-detected From | Examples |
|-------|-------------------|---------|
| `primaryLanguage` | File extensions, build files | `TypeScript`, `Python`, `Java`, `Go` |
| `languageVersion` | `.nvmrc`, `pyproject.toml`, `pom.xml` | `"5.4"`, `"3.12"` |
| `runtimeVersion` | `.nvmrc`, `Dockerfile` | `"Node 20.x"`, `"JDK 21"` |
| `framework` | Dependencies in build file | `React`, `Next.js`, `FastAPI`, `Spring Boot` |
| `frameworkVersion` | Package version | `"14.x"`, `"3.x"` |
| `frameworkVariant` | Config files, folder structure | `"App Router"`, `"REST"` |
| `packageManager` | Lock file presence | `npm`, `pnpm`, `yarn`, `pip`, `maven` |
| `testFramework` | Test config files | `Jest`, `Vitest`, `Pytest`, `JUnit` |
| `e2eFramework` | devDependencies | `Playwright`, `Cypress`, `none` |
| `linter` | Config files | `ESLint`, `Pylint`, `Ruff`, `none` |
| `linterConfig` | Detected file path | `.eslintrc.json`, `pyproject.toml` |
| `formatter` | Config files | `Prettier`, `Black`, `none` |
| `typeChecker` | Config files | `tsc`, `mypy`, `none` |
| `styling` | Dependencies | `Tailwind CSS`, `MUI`, `CSS Modules`, `none` |
| `stateManagement` | Dependencies | `Redux`, `Zustand`, `TanStack Query`, `none` |
| `apiStyle` | Schema files, routes | `REST`, `GraphQL`, `gRPC`, `tRPC` |
| `databases` | Dependencies, env vars, docker-compose | `[PostgreSQL, Redis]` |
| `orm` | Dependencies | `Prisma`, `TypeORM`, `SQLAlchemy`, `none` |
| `ciCd` | CI config files | `GitHub Actions`, `Jenkins`, `GitLab CI` |
| `containerization` | Dockerfile presence | `Docker`, `none` |
| `deployTarget` | Config files, hosting hints | `Vercel`, `AWS`, `GCP`, `self-hosted` |
| `repositoryType` | Workspace config | `single`, `monorepo`, `multi-repo` |
| `additionalLanguages` | Other file extensions | `["SQL", "HTML"]` |

---

### `commands.*` — Executable Commands

These must be exact shell commands runnable from your project root.

| Field | Purpose | JS/TS Example | Python Example |
|-------|---------|---------------|----------------|
| `install` | Install dependencies | `npm install` | `pip install -r requirements.txt` |
| `dev` | Start dev server | `npm run dev` | `uvicorn main:app --reload` |
| `build` | Production build | `npm run build` | `python -m build` |
| `start` | Run production server | `node dist/index.js` | `uvicorn main:app` |
| `test` | Run test suite | `npx vitest run` | `pytest` |
| `testWatch` | Run tests in watch mode | `npx vitest` | `ptw` |
| `testCoverage` | Run with coverage | `npx vitest run --coverage` | `pytest --cov=src` |
| `lint` | Run linter (check only) | `npx eslint src/ --ext .ts` | `ruff check src/` |
| `lintFix` | Run linter with auto-fix | `npx eslint src/ --fix` | `ruff check src/ --fix` |
| `format` | Check formatting | `npx prettier --check src/` | `black --check src/` |
| `formatFix` | Apply formatting | `npx prettier --write src/` | `black src/` |
| `typecheck` | Type check (no emit) | `npx tsc --noEmit` | `mypy src/` |
| `audit` | Security audit | `npm audit --audit-level=moderate` | `pip-audit` |
| `migrate` | Run DB migrations | `npx prisma migrate dev` | `alembic upgrade head` |
| `clean` | Clean build artifacts | `rm -rf dist .next` | `find . -name __pycache__ -delete` |

---

### `codingStandards.*` — Quality Thresholds

| Field | Default | Description |
|-------|---------|-------------|
| `maxFunctionLines` | `10` | Max lines per function (not counting blanks/comments) |
| `maxCyclomaticComplexity` | `8` | Max independent paths through a function |
| `maxFileLines` | `300` | Max lines per source file |
| `maxClassMethods` | `10` | Max methods per class |

#### `codingStandards.conventions.*`

| Field | Options | Description |
|-------|---------|-------------|
| `componentStyle` | `functional` \| `class` | React component style |
| `importStyle` | `named` \| `default` \| `mixed` | Import preference |
| `namingConvention` | `camelCase` \| `snake_case` \| `PascalCase` | Variables/functions |
| `fileNaming` | `kebab-case` \| `camelCase` \| `PascalCase` | Source files |
| `testStyle` | `colocated` \| `separate-folder` | Test file location |
| `organizationPattern` | `featureBased` \| `layerBased` \| `domainBased` | Folder structure |

#### `codingStandards.patterns`
List of architectural patterns agents must follow. These are applied when generating new code.

Example:
```yaml
patterns:
  - "Repository pattern for all data access"
  - "Constructor injection for dependencies"
  - "DTOs for all API request/response shapes"
```

#### `codingStandards.prohibitedPatterns`
Patterns agents must never use. Review Agent flags violations.

---

### `testing.*` — Test Configuration

#### `testing.coverageThresholds`
Minimum coverage percentages. Build fails if any threshold is not met.

```yaml
coverageThresholds:
  statements: 80   # % of statements executed
  branches: 75     # % of branches taken
  functions: 85    # % of functions called
  lines: 80        # % of lines executed
```

#### `testing.e2e`
```yaml
e2e:
  enabled: true
  baseUrl: "http://localhost:3000"
  headless: true  # false to see browser during local runs
```

---

### `integrations.*`

#### Jira
```yaml
jira:
  enabled: true
  baseUrl: "https://your-org.atlassian.net"
  projectKey: "PROJ"
  # Set env vars: JIRA_API_TOKEN, JIRA_USER_EMAIL
```

#### Git
```yaml
git:
  provider: github   # github | gitlab | bitbucket | azure-devops
  owner: "your-org"
  repo: "your-repo"
  defaultBranch: main
  mergeStrategy: squash   # squash | rebase | merge
  requireLinearHistory: true
  requireSignedCommits: false
```

#### Monitoring
```yaml
monitoring:
  enabled: true
  provider: sentry  # sentry | datadog | newrelic | grafana | cloudwatch
  dashboardUrl: "https://..."
  alertsSlackChannel: "#alerts"
```

---

### `deployment.*`

```yaml
deployment:
  environments:
    development:
      url: "http://localhost:3000"
    staging:
      url: "https://staging.example.com"
      autoDeployOnMerge: true
      branch: main
    production:
      url: "https://example.com"
      autoDeployOnMerge: false
      requireApproval: true

  releaseStrategy: continuous   # continuous | tagged | manual
  versioningScheme: semver      # semver | calver | none
  changelogFormat: keepachangelog
```

---

### `maintenance.*`

```yaml
maintenance:
  dependencyUpdateSchedule: monthly  # weekly | monthly | quarterly
  securityAuditSchedule: weekly
  techDebtReviewSchedule: monthly

  licensing:
    allowed: [MIT, Apache-2.0, BSD-2-Clause, ISC]
    restricted: [GPL-2.0, GPL-3.0]
    blocked: [UNLICENSED, UNKNOWN]
```

---

## Config Inheritance and Overrides

The config hierarchy is flat — one `project.yaml` per project. For monorepos, create a `project.yaml` per package:

```
packages/
├── web-app/
│   └── config/project.yaml    ← React frontend config
├── api/
│   └── config/project.yaml    ← Express API config
└── shared/
    └── config/project.yaml    ← Shared lib config
```

## Validation

Run the discovery agent to validate your config:
```
"Validate my project.yaml config"
```

It checks:
- All required fields are present
- Commands are executable
- No placeholder "TODO" values remain in required fields
- Integration URLs are reachable (if integrations.*.enabled is true)

---

## External Tool Integrations — `config/integrations.yaml`

`project.yaml` holds project metadata and tech stack settings. **External tool connections** (Jira, GitHub, Figma, Confluence, Slack) live in a separate file: `config/integrations.yaml`.

### Why two files?

| File | Contains | Updated by |
|------|----------|------------|
| `config/project.yaml` | Tech stack, commands, coding standards | Discovery Agent + you |
| `config/integrations.yaml` | Tool URLs, project keys, feature flags | You at project setup |

### Structure overview

```yaml
sourceControl:
  provider: github            # github | gitlab | bitbucket
  github:
    owner: "your-org"
    repo: "your-repo"
    defaultBranch: main

issueTracking:
  provider: jira              # jira | github | linear
  jira:
    baseUrl: "https://your-org.atlassian.net"
    projectKey: "PROJ"
    transitions:
      toDo: 11
      inProgress: 21
      inReview: 31
      done: 41

design:
  figma:
    enabled: false            # set true to activate design extraction in requirements
    teamId: null

documentation:
  confluence:
    baseUrl: "https://your-org.atlassian.net/wiki"
    spaceKey: "PROJ"
    pages:
      releaseNotesParent: null   # right-click page → Page Information → ID in URL
      designDocsParent: null

externalSources:
  fetchLinkedUrls: true        # fetch URLs found in Jira ticket fields
  attachments:
    images: true
    pdfs: true
    maxSizeMb: 10

notifications:
  slack:
    channels:
      deployments: "#deployments"
      alerts: "#alerts"
      releases: "#releases"
```

### Credentials

Never put tokens or passwords in `integrations.yaml`. Those go in `.env` (which is in `.gitignore`). Copy `.env.example` to `.env` and fill in your credentials.

Run `/setup --validate` to test that all configured integrations are reachable with the current credentials.

---

## Team Learnings — `config/learnings.yaml`

This file stores accumulated team knowledge that makes all agents smarter over time. Unlike `project.yaml` (auto-detected) and `integrations.yaml` (set once at setup), `learnings.yaml` grows continuously via `/learn` and `/retro`.

### Why a separate file?

| File | Contains | Updated by |
|------|----------|------------|
| `config/project.yaml` | Tech stack, commands, standards | Discovery Agent (auto) |
| `config/integrations.yaml` | External tool connections | You at project setup |
| `config/learnings.yaml` | Team knowledge, patterns, overrides, postmortems | `/learn`, `/retro`, you |

### How agents use it

Every agent loads `learnings.yaml` after `project.yaml`. Each section targets a specific agent:

| Section | Read by | Effect |
|---------|---------|--------|
| `patterns.avoid` (blocker) | Review Agent | Violations are flagged as BLOCKERs in every PR |
| `patterns.avoid` (warning) | Review Agent | Violations are flagged as WARNINGs |
| `patterns.preferred` | Dev + Review Agent | Dev Agent follows them; Review Agent suggests them |
| `agentHints.requirements` | Requirements Agent | Extra context injected into requirements analysis |
| `agentHints.development` | Development Agent | Extra context injected when writing code |
| `agentHints.review` | Review Agent | Extra checks added to every review |
| `agentHints.testing` | Testing Agent | Extra testing guidance injected |
| `reviewOverrides` | Review Agent | Overrides thresholds from `project.yaml` |
| `process.*` | All agents | Changes workflow (skip steps, auto modes, path exclusions) |
| `techDebt` | Maintenance Agent | Surfaced in `/health` reports |
| `postmortems` | Maintenance + Review Agent | Context when touching incident-related code |

### Key sections reference

```yaml
patterns:
  avoid:
    - pattern: "Don't import PrismaClient in controllers"
      reason: "Bypasses service layer auth checks"
      severity: blocker       # blocker | warning | suggestion
      source: "retro-2025-Q1"

agentHints:
  requirements:
    - "Our Jira ACs are often incomplete — always check linked Confluence pages"
  review:
    - "Any change to src/payments/** needs an explicit PCI scope note"

reviewOverrides:
  maxFunctionLines: 15          # override project.yaml default of 10
  extraSecurityPaths:
    - "src/auth/**"

process:
  skipDesignDocForTypes: ["bug"]       # skip design doc for bug-fix tickets
  autonomousOnLabels: ["hotfix", "p0"] # skip Jira clarification loop for these labels
  requireHumanReviewForPaths:
    - "src/payments/**"

techDebt:
  - id: "TD-001"
    description: "UserService is 400 lines — needs splitting"
    file: "src/users/user.service.ts"
    severity: warning
    ticketRef: "PROJ-789"

postmortems:
  - id: "PM-001"
    incident: "Auth failure after JWT rotation"
    learning: "Cross-service JWT consumer check required before auth deploys"
    rulesAdded:
      - "review: flag JWT secret changes for cross-service impact check"
```

### How to add entries

```bash
# Single learning (agent classifies and routes it automatically):
/learn "Never call Prisma directly from controllers"

# Sprint retrospective (analyzes recent PRs and proposes batch updates):
/retro

# Or edit config/learnings.yaml directly — every entry must have a source field
```

See `agentic-pdlc-framework/agents/10-framework-evolution-agent.md` for the full classification logic.
