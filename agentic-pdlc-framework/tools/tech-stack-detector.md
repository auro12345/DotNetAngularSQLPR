# Tool: Tech Stack Detector

## Purpose
Determines the complete technology stack of a project from its files. Core tool used by Project Scanning Skill and Project Discovery Agent.

## Detection Rules

### Language Detection

| Signal | Language | Confidence |
|--------|----------|------------|
| `package.json` + no `.ts` files | JavaScript | High |
| `package.json` + `tsconfig.json` | TypeScript | High |
| `requirements.txt` or `pyproject.toml` | Python | High |
| `pom.xml` + `src/main/java/` | Java | High |
| `build.gradle.kts` + `*.kt` | Kotlin | High |
| `go.mod` | Go | High |
| `Cargo.toml` | Rust | High |
| `*.csproj` or `*.sln` | C# | High |
| `Gemfile` + `*.rb` | Ruby | High |
| `composer.json` + `*.php` | PHP | High |
| Only `*.ts` files, no package.json | TypeScript (Deno?) | Medium |

**Multi-language projects:**
```
If multiple languages detected → list all, set primary = one with most source files
Store others in tech.additionalLanguages
```

### Runtime Version Detection
```bash
# Node.js version
cat .nvmrc 2>/dev/null || node --version 2>/dev/null || cat package.json | jq '.engines.node'

# Python version
cat .python-version 2>/dev/null || python --version 2>/dev/null || cat pyproject.toml | grep 'python_requires'

# Java version
cat .java-version 2>/dev/null || java -version 2>/dev/null || cat pom.xml | grep '<java.version>'
```

### Framework Detection Decision Tree

```
JavaScript/TypeScript project?
├── Has "next" in deps → Next.js
│   ├── Has "app" directory → App Router (Next 13+)
│   └── Has "pages" directory → Pages Router
├── Has "nuxt" in deps → Nuxt.js
├── Has "@angular/core" in deps → Angular
├── Has "svelte" in deps
│   ├── Has "@sveltejs/kit" → SvelteKit
│   └── Otherwise → Svelte
├── Has "astro" in deps → Astro
├── Has "remix" in deps → Remix
├── Has "react" in deps → React
│   ├── Also has "vite" → React + Vite
│   ├── Also has "webpack" → React + Webpack (CRA or custom)
│   └── No bundler config → Minimal React
├── Has "@nestjs/core" in deps → NestJS (backend)
├── Has "fastify" in deps → Fastify (backend)
├── Has "express" in deps → Express (backend)
├── Has "hono" in deps → Hono (backend)
└── No recognizable framework → Vanilla JS/TS

Python project?
├── Has "django" → Django
│   ├── Has "rest_framework" → Django REST Framework
│   └── Has "channels" → Django + WebSockets
├── Has "fastapi" → FastAPI
├── Has "flask" → Flask
├── Has "starlette" → Starlette
├── Has "tornado" → Tornado
└── No framework → Pure Python

Java project?
├── Has "spring-boot" in pom → Spring Boot
│   ├── Has "spring-security" → + Security
│   ├── Has "spring-data" → + Data
│   └── Has "spring-web" → + MVC/REST
├── Has "quarkus" → Quarkus
├── Has "micronaut" → Micronaut
└── No framework → Plain Java
```

### Styling System Detection (Frontend)

```bash
grep -E "tailwindcss|@mui/material|styled-components|@emotion|antd|chakra-ui|shadcn|radix-ui|bootstrap" \
  package.json 2>/dev/null
```

| Package | System |
|---------|--------|
| `tailwindcss` | Tailwind CSS |
| `@mui/material` | Material UI |
| `styled-components` | Styled Components |
| `@emotion/styled` | Emotion |
| `antd` | Ant Design |
| `@chakra-ui/react` | Chakra UI |
| `bootstrap` | Bootstrap |
| None above + `*.module.css` files | CSS Modules |
| None above | Plain CSS |

### State Management Detection (Frontend)

```bash
grep -E "redux|zustand|recoil|jotai|mobx|@tanstack/react-query|swr|apollo-client" \
  package.json 2>/dev/null
```

### Monorepo Detection

```bash
# Check for workspaces
cat package.json | jq '.workspaces' 2>/dev/null
ls pnpm-workspace.yaml 2>/dev/null
ls lerna.json 2>/dev/null
ls nx.json 2>/dev/null
ls turbo.json 2>/dev/null
ls rush.json 2>/dev/null
```

If workspaces detected:
```yaml
repositoryType: monorepo
monorepoTool: [turborepo | nx | lerna | pnpm-workspaces | yarn-workspaces]
packages:
  - name: [package-name]
    path: packages/[name]
    type: [app | lib | service]
    framework: [detected per-package]
```

## Output Schema

```yaml
tech:
  primaryLanguage: TypeScript
  languageVersion: "5.4"
  runtimeVersion: "Node 20.x"
  framework: Next.js
  frameworkVersion: "14.x"
  frameworkVariant: "App Router"
  packageManager: pnpm
  packageManagerVersion: "8.x"
  testFramework: Vitest
  e2eFramework: Playwright
  linter: ESLint
  linterConfig: .eslintrc.json
  formatter: Prettier
  formatterConfig: .prettierrc
  typeChecker: tsc
  styling: "Tailwind CSS"
  stateManagement: "TanStack Query"
  apiStyle: REST
  databases:
    - PostgreSQL
    - Redis
  orm: Prisma
  ciCd: "GitHub Actions"
  containerization: Docker
  deployTarget: Vercel
  repositoryType: single
  additionalLanguages: []
```
