# Skill: Code Analysis

## Purpose
Run static analysis, linting, type checking, and complexity analysis on the codebase. Used by Development Agent and Review Agent.

## Operations by Language

### JavaScript / TypeScript

**Linting:**
```bash
# ESLint
npx eslint src/ --ext .ts,.tsx,.js,.jsx
npx eslint src/ --ext .ts,.tsx --fix  # auto-fix

# Biome (modern alternative)
npx biome check src/
```

**Type Checking:**
```bash
npx tsc --noEmit
```

**Formatting:**
```bash
npx prettier --check src/
npx prettier --write src/  # auto-format
```

**Complexity Analysis:**
```bash
# Check cyclomatic complexity
npx eslint --rule '{"complexity": ["error", 8]}' src/

# Find long functions (>10 lines)
# Custom scan: count lines between function start and end
```

**Dependency Security:**
```bash
npm audit --audit-level=moderate
npm audit fix  # auto-fix where safe
```

**Bundle Analysis (frontend):**
```bash
# Next.js
ANALYZE=true npm run build

# Vite
npx vite-bundle-visualizer
```

### Python

**Linting:**
```bash
# Ruff (fast, modern)
ruff check src/
ruff check src/ --fix

# Pylint (comprehensive)
pylint src/

# Flake8 (classic)
flake8 src/
```

**Type Checking:**
```bash
mypy src/ --strict
```

**Formatting:**
```bash
black --check src/
black src/  # auto-format
isort --check-only src/
isort src/  # auto-sort imports
```

**Security:**
```bash
bandit -r src/
pip-audit
```

**Complexity:**
```bash
radon cc src/ -a -nb  # cyclomatic complexity, show average
radon mi src/  # maintainability index
```

### Java

**Linting / Style:**
```bash
# Checkstyle
mvn checkstyle:check

# SpotBugs
mvn spotbugs:check

# PMD
mvn pmd:check
```

**Type Checking:** (built into javac)
```bash
mvn compile
```

**Formatting:**
```bash
# Google Java Format
java -jar google-java-format.jar --check src/**/*.java
java -jar google-java-format.jar --replace src/**/*.java
```

**Security:**
```bash
mvn org.owasp:dependency-check-maven:check
```

### Go

**Linting:**
```bash
golangci-lint run ./...
go vet ./...
```

**Formatting:**
```bash
gofmt -l .
gofmt -w .  # auto-format
```

**Security:**
```bash
govulncheck ./...
```

## Analysis Result Parser

After running analysis tools, parse output into structured format:
```json
{
  "tool": "eslint",
  "status": "failed",
  "errors": [
    {
      "file": "src/auth/auth.service.ts",
      "line": 34,
      "rule": "no-unused-vars",
      "severity": "error",
      "message": "'token' is defined but never used"
    }
  ],
  "warnings": [...],
  "summary": {
    "errors": 1,
    "warnings": 3,
    "fixable": 2
  }
}
```

## Thresholds (from project.yaml)

```yaml
codingStandards:
  maxFunctionLines: 10
  maxCyclomaticComplexity: 8
  maxFileLines: 300
  maxClassMethods: 10
```

Violations above these thresholds are reported as WARNINGS in the review report.

## Running All Checks
The analysis skill runs tools in this order (all results aggregated):
1. Formatter (check mode)
2. Linter
3. Type checker
4. Security audit
5. Complexity check

Report: pass/fail per tool + combined issue list.
