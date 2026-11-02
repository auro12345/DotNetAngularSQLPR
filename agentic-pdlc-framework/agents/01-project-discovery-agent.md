# Agent: Project Discovery Agent

## Role
The Project Discovery Agent runs **automatically at the start of every session** and whenever a new project is opened. Its job is to understand the current state of the project — tech stack, structure, domain, and conventions — and populate or update `config/project.yaml`.

This agent is the foundation. All other agents depend on the context it produces.

## Activation
- **Automatic**: Triggers when `config/project.yaml` is missing or older than 7 days
- **Manual**: Triggered by user saying "scan project", "update config", or "re-discover project"
- **On demand**: Other agents call this agent when they detect config is stale

## Inputs
- Project root file system
- Existing `config/project.yaml` (if any)
- `agentic-pdlc-framework/config/project.template.yaml` (schema reference)

## Outputs
- Created/updated `config/project.yaml`
- Discovery summary printed to terminal

## Discovery Steps

### 1. File System Scan
```
Scan for:
├── Build/package files: package.json, pom.xml, build.gradle, Cargo.toml,
│   requirements.txt, pyproject.toml, go.mod, *.csproj, Gemfile, composer.json
├── Framework configs: next.config.*, vite.config.*, angular.json, nuxt.config.*,
│   vue.config.*, webpack.config.*, svelte.config.*, astro.config.*
├── Test configs: jest.config.*, vitest.config.*, pytest.ini, .pytest.ini,
│   karma.conf.*, cypress.json, playwright.config.*
├── Lint/format: .eslintrc*, .prettierrc*, pylintrc, .flake8, ruff.toml,
│   .stylelintrc*, checkstyle.xml, .rubocop.yml
├── CI/CD: .github/workflows/*.yml, .gitlab-ci.yml, Jenkinsfile,
│   .circleci/config.yml, azure-pipelines.yml, bitbucket-pipelines.yml
├── Container/Deploy: Dockerfile, docker-compose.yml, k8s/, helm/, *.tf (Terraform),
│   serverless.yml, vercel.json, netlify.toml
├── Database: migrations/, alembic/, flyway/, db/schema.rb
└── Docs: README.md, CONTRIBUTING.md, ARCHITECTURE.md, docs/, ADR/
```

### 2. Tech Stack Detection Logic

**Language detection** (in order of confidence):
1. If `package.json` with TypeScript in deps/devDeps → TypeScript
2. If `package.json` without TypeScript → JavaScript
3. If `pyproject.toml` or `requirements.txt` → Python
4. If `pom.xml` or `build.gradle` → Java/Kotlin
5. If `go.mod` → Go
6. If `Cargo.toml` → Rust
7. If `*.csproj` → C#
8. If `Gemfile` → Ruby

**Framework detection:**
- React: `"react"` in package.json dependencies
- Next.js: `"next"` in deps + `next.config.*`
- Vue: `"vue"` in deps
- Angular: `@angular/core` in deps
- Svelte: `"svelte"` in deps
- FastAPI: `fastapi` in Python requirements
- Django: `django` in Python requirements
- Spring Boot: `spring-boot-starter` in Java pom/gradle
- Express: `"express"` in Node deps
- NestJS: `"@nestjs/core"` in deps

**Package manager detection:**
- `package-lock.json` → npm
- `yarn.lock` → Yarn
- `pnpm-lock.yaml` → pnpm
- `bun.lock` / `bun.lockb` → Bun

### 3. Code Pattern Analysis
Sample up to 20 source files to detect:
- Naming conventions (camelCase, snake_case, PascalCase for different constructs)
- Import style (default vs named imports)
- File organization (feature-based vs layer-based)
- State management patterns (Redux, Zustand, Context API, etc.)
- Testing patterns (test file colocation vs separate folder)

### 4. Domain Inference
Look for clues in:
- `README.md` (first paragraph, title)
- API route names and model names
- Database table/schema names
- Folder names (`/orders`, `/patients`, `/products`, etc.)

### 5. Commands Resolution
Detect runnable commands:
```yaml
commands:
  install: [detected from package manager]
  build: [from scripts in package.json / Makefile / build.gradle]
  test: [from test config files]
  lint: [from eslint/pylint/etc config]
  format: [from prettier/black/etc config]
  typecheck: [tsc --noEmit / mypy / etc]
  dev: [dev server command]
  start: [production start command]
```

## Output Format

After discovery, print:
```
╔══════════════════════════════════════════════════╗
║          PROJECT DISCOVERY COMPLETE              ║
╠══════════════════════════════════════════════════╣
║ Project:     [name]                              ║
║ Domain:      [domain or "inferred: e-commerce"]  ║
║ Language:    [primaryLanguage]                   ║
║ Framework:   [framework]                         ║
║ Tests:       [testFramework]                     ║
║ Linter:      [linter]                            ║
║ CI/CD:       [ciCd]                              ║
║ Config:      config/project.yaml [created/updated]║
╚══════════════════════════════════════════════════╝

⚠️  Fields requiring manual review:
  - project.domain: set to "unknown" — please update
  - integrations.jira.projectKey: not detected
```

## Tools Used

| Tool | When |
|------|------|
| `agentic-pdlc-framework/tools/tech-stack-detector.md` | File system scan — maps project files to language/framework/test runner |

## Error Handling
- If project root is empty: create minimal config with `status: "new-project"` and prompt user
- If conflicting signals (e.g., both Python and Node files): list both, set `primaryLanguage` to the dominant one, note secondary in `tech.additionalLanguages`
- If no recognizable structure: set all tech fields to "unknown" and create blank config with TODOs
