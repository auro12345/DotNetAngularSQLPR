# Skill: Project Scanning

## Purpose
Scans a project's file system structure to detect tech stack, frameworks, patterns, and configuration. Core skill used by Project Discovery Agent.

## Scan Operations

### 1. Root File Scan
```bash
# Find all build/config files at depth 1-2
find . -maxdepth 2 -name "package.json" -o \
  -name "pom.xml" -o -name "build.gradle" -o \
  -name "Cargo.toml" -o -name "go.mod" -o \
  -name "requirements.txt" -o -name "pyproject.toml" -o \
  -name "Gemfile" -o -name "composer.json" -o \
  -name "*.csproj" -o -name "*.sln" | grep -v node_modules

# Find CI/CD config
find . -maxdepth 3 -name "*.yml" -path "*github/workflows*" -o \
  -name ".gitlab-ci.yml" -o -name "Jenkinsfile" -o \
  -name "azure-pipelines.yml" -o -name ".circleci/config.yml"
```

### 2. Package.json Analysis
```javascript
// Parse package.json to extract:
{
  name, version, description,
  scripts: { start, build, test, dev, lint, typecheck },
  dependencies: { react, next, vue, angular, express, ... },
  devDependencies: { typescript, jest, vitest, eslint, prettier, ... }
}
```

Tech stack inference from package.json:
```
"react" in deps                    → framework: React
"next" in deps                     → framework: Next.js (React)
"@angular/core" in deps            → framework: Angular
"vue" in deps && "nuxt" in deps    → framework: Nuxt.js (Vue)
"vue" in deps                      → framework: Vue
"svelte" in deps                   → framework: Svelte
"@nestjs/core" in deps             → framework: NestJS
"express" in deps                  → framework: Express
"fastify" in deps                  → framework: Fastify
"typescript" in devDeps            → language: TypeScript
"jest" in devDeps                  → testFramework: Jest
"vitest" in devDeps                → testFramework: Vitest
"@playwright/test" in devDeps      → e2eFramework: Playwright
"cypress" in devDeps               → e2eFramework: Cypress
"eslint" in devDeps                → linter: ESLint
"prettier" in devDeps              → formatter: Prettier
"tailwindcss" in deps/devDeps      → styling: Tailwind
"@mui/material" in deps            → styling: MUI
```

### 3. Directory Structure Analysis
```bash
# Detect project organization pattern
find src -maxdepth 2 -type d | head -40

# Feature-based: src/features/auth/, src/features/cart/
# Layer-based: src/controllers/, src/services/, src/repositories/
# Domain-based: src/auth/, src/orders/, src/users/
```

Pattern detection:
- 3+ folders named after features → `featureBased`
- Folders named controllers/services/models → `layerBased`
- Mixed → `hybrid`

### 4. Test File Pattern Detection
```bash
# Find test files
find . -name "*.test.ts" -o -name "*.spec.ts" -o \
       -name "*.test.js" -o -name "*.spec.js" | \
       grep -v node_modules | head -20

# Detect: co-located vs separate __tests__ folder
```

### 5. Code Conventions Sampling
Sample 20 source files for convention detection:
```bash
# Find main source files (not test, not generated)
find src -name "*.ts" ! -name "*.test.ts" ! -name "*.spec.ts" \
  -not -path "*/node_modules/*" | shuf -n 20
```

Analyze sampled files for:
- Import style: `import X from 'y'` vs `import { X } from 'y'`
- Function style: arrow functions vs `function` keyword
- Component style: functional vs class
- File naming: kebab-case vs camelCase vs PascalCase
- Export style: named vs default

### 6. API Style Detection
```bash
# GraphQL
find . -name "*.graphql" -o -name "schema.gql" | grep -v node_modules

# gRPC
find . -name "*.proto" | grep -v node_modules

# tRPC
grep -r "createTRPCRouter\|initTRPC" src/ --include="*.ts" -l

# REST (default if none of the above)
```

### 7. Database Detection
```bash
# ORM/query builder in deps
grep -E "prisma|typeorm|sequelize|knex|drizzle|mongoose|sqlalchemy|hibernate|jooq|gorm" \
  package.json requirements.txt pyproject.toml pom.xml 2>/dev/null

# Migration files
find . -name "*.migration.ts" -o -name "*_migration.py" \
  -o -path "*/migrations/*.sql" | grep -v node_modules | head -5

# DB connections in config
grep -rE "(postgres|mysql|mongodb|redis|sqlite|cockroach)" \
  .env.example .env.development docker-compose.yml 2>/dev/null
```

## Output: Detection Report

```json
{
  "detectedAt": "2024-01-15T10:30:00Z",
  "primaryLanguage": "TypeScript",
  "framework": "Next.js",
  "packageManager": "pnpm",
  "testFramework": "Vitest",
  "e2eFramework": "Playwright",
  "linter": "ESLint",
  "formatter": "Prettier",
  "styling": "Tailwind CSS",
  "apiStyle": "REST",
  "databases": ["PostgreSQL", "Redis"],
  "orm": "Prisma",
  "ciCd": "GitHub Actions",
  "deployment": "Vercel",
  "repositoryType": "monorepo",
  "organizationPattern": "featureBased",
  "testPattern": "colocated",
  "confidence": {
    "primaryLanguage": "high",
    "framework": "high",
    "databases": "medium"
  }
}
```

Confidence levels:
- `high`: Detected from explicit config file (package.json, config file)
- `medium`: Inferred from code patterns or directory names
- `low`: Guessed from file extensions only
