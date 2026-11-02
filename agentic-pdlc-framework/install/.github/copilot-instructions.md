# GitHub Copilot Instructions — PDLC Framework

These instructions are loaded automatically by GitHub Copilot in VS Code and JetBrains IDEs.

## Project Context

This is a PDLC (Product Development Life Cycle) AI framework. It contains agent definitions, rules, skills, tools, and configuration templates for automating software development workflows. See `agentic-pdlc-framework/docs/README.md` for full context.

**When working in a project that uses this framework**, always:
1. Check `config/project.yaml` for the project's tech stack, patterns, and coding standards
2. Follow the conventions in `config/project.yaml > codingStandards`
3. Apply the rules in `.cursor/rules/` when generating code

## Code Generation Rules

### Function Size
- **Maximum 10 lines** per function (or the project threshold from `config/project.yaml > codingStandards.maxFunctionLines`)
- Extract sub-functions if a function would grow beyond the limit
- Each function does exactly ONE thing

### No Comments by Default
- Do not add comments unless the WHY is non-obvious
- Never write comments explaining WHAT the code does (the code does that)
- No TODO comments without a ticket reference

### Type Safety
- TypeScript: no `any` type — use `unknown` + type narrowing
- Python: type hints on all public functions
- Java: avoid raw types

### Security
- Never hard-code API keys, tokens, or passwords
- SQL queries: always parameterized (never string concatenation)
- User input: validate at entry points only

### Naming
- Use intention-revealing names
- Boolean functions/variables: `is`, `has`, `can`, `should` prefixes
- No abbreviations except universally known: `id`, `url`, `api`, `ctx`

## Skeleton-First Pattern

When generating new classes or modules, generate skeleton first:
```typescript
class ExampleService {
  /**
   * What this method does and why.
   * @throws SomeError when [condition]
   */
  async methodName(param: ParamType): Promise<ReturnType> {
    // TODO: implement
    throw new NotImplementedError();
  }
}
```

## Test Generation

When asked to generate tests:
1. Use AAA (Arrange-Act-Assert) pattern
2. Test name format: `[unit]_[scenario]_[expectedOutcome]`
3. Always test: happy path, invalid input, error cases, edge cases
4. Mock only external dependencies (never internal business logic)

## Commit Messages

When suggesting commit messages, use Conventional Commits:
```
feat(scope): imperative description

Body explaining what and why.

Implements: TICKET-ID
```

## Tech Stack Conventions

Read `config/project.yaml` for the active stack. Default conventions:

**React/TypeScript:**
- Functional components only (never class components)
- Named exports (not default exports except Next.js pages)
- Props typed as interface above the component

**Python:**
- Pydantic models for data structures
- `async/await` for I/O operations
- Type hints on all functions

**Java/Spring:**
- Constructor injection (not field injection)
- Repository pattern for data access
- DTOs for API request/response shapes

## Documentation Reference
- Architecture: `architecture.drawio`
- Agents: `agentic-pdlc-framework/agents/` directory
- Config schema: `agentic-pdlc-framework/config/project.template.yaml`
- Tech conventions: `agentic-pdlc-framework/docs/tech-stack-guide.md`
