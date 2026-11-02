# Skill: Test Execution

## Purpose
Run tests, parse results, generate coverage reports, and analyze failures. Used by Testing Agent and Development Agent.

## Test Runner Detection
Read `config/project.yaml > tech.testFramework` and `config/project.yaml > commands.test`.
Fall back to detection from config files:

| Config File | Framework | Default Command |
|-------------|-----------|----------------|
| `jest.config.*` | Jest | `npx jest` |
| `vitest.config.*` | Vitest | `npx vitest run` |
| `pytest.ini` / `pyproject.toml [tool.pytest]` | Pytest | `pytest` |
| `build.gradle` with `testImplementation` | JUnit | `./gradlew test` |
| `pom.xml` with `junit` | JUnit | `mvn test` |
| `go.mod` | Go test | `go test ./...` |
| `*.test.rs` | Rust/Cargo | `cargo test` |
| `*_test.go` | Go | `go test ./...` |
| `spec/` dir + Gemfile | RSpec | `bundle exec rspec` |

## Execution Modes

### Run All Tests
```bash
# JS/TS
npx jest --passWithNoTests
npx vitest run

# Python
pytest -v

# Java
./gradlew test
mvn test

# Go
go test ./... -v

# Rust
cargo test
```

### Run with Coverage
```bash
# Jest
npx jest --coverage --coverageReporters=text,lcov

# Vitest
npx vitest run --coverage

# Pytest
pytest --cov=src --cov-report=term-missing --cov-report=html

# Java/JaCoCo
./gradlew test jacocoTestReport

# Go
go test ./... -coverprofile=coverage.out
go tool cover -html=coverage.out
```

### Run Single File / Pattern
```bash
# Jest
npx jest src/auth/auth.service.test.ts
npx jest --testPathPattern=auth

# Pytest
pytest tests/test_auth.py
pytest -k "test_login"

# Go
go test ./auth/... -run TestLogin
```

### Watch Mode (development)
```bash
# Jest
npx jest --watch

# Vitest
npx vitest

# Pytest-watch
ptw
```

## Result Parsing

Parse test runner output into structured result:
```json
{
  "status": "passed" | "failed" | "error",
  "summary": {
    "total": 47,
    "passed": 45,
    "failed": 2,
    "skipped": 0,
    "duration_ms": 4230
  },
  "failures": [
    {
      "file": "src/auth/auth.service.test.ts",
      "suite": "AuthService > login",
      "test": "returns token for valid credentials",
      "error": "Expected value to be defined, received undefined",
      "stack": "...",
      "line": 34
    }
  ],
  "coverage": {
    "statements": 87.5,
    "branches": 82.1,
    "functions": 91.2,
    "lines": 88.3,
    "uncovered": [
      "src/auth/auth.service.ts: 45-47"
    ]
  }
}
```

## Coverage Threshold Enforcement

Read thresholds from `config/project.yaml > testing.coverageThresholds`:
```yaml
testing:
  coverageThresholds:
    statements: 80
    branches: 75
    functions: 85
    lines: 80
```

If any threshold not met, report as BLOCKER:
```
⛔ Coverage threshold not met:
  branches: 72% (required: 75%) — FAIL
  functions: 87% (required: 85%) — PASS
```

## Failure Analysis Workflow

1. Identify failing tests from parsed results
2. For each failure:
   a. Read the test code to understand what it expects
   b. Read the implementation code at the error point
   c. Determine: test bug vs code bug
   d. If code bug: report to Development Agent with exact location and fix
   e. If test bug: fix the test (with explanation)
3. Re-run after fixes to confirm resolution

## Skipped Tests Policy

Skipped tests require justification:
- `it.skip` / `pytest.mark.skip` must have a comment with ticket reference
- Count of skipped tests reported in summary
- Alert if skipped count increases (possible test hiding)
