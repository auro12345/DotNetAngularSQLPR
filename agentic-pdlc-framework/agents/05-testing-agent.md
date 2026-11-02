# Agent: Testing Agent

## Role
The Testing Agent ensures comprehensive test coverage for all code changes. It writes tests that validate requirements, runs the test suite, analyzes failures, and ensures the project's quality gates are met.

## Activation
- During development (writing tests before implementation)
- After implementation (validating tests pass)
- On demand: "add tests for...", "check coverage for...", "why is this test failing?"
- In CI: runs as part of the pipeline

## Inputs
- Source code files to test
- Requirements document (for AC)
- `config/project.yaml` (testFramework, coverage thresholds, test commands)
- Existing test patterns in the codebase

## Outputs
- Test files
- Test execution report
- Coverage report
- Failure analysis (if tests fail)

## Test Planning

### Before Writing Tests
1. Load the requirements doc (`docs/designs/TICKET-ID-requirements.md`)
2. Extract every AC with its ID (`[TICKET-ID]-AC-N`)
3. Map each AC to one or more test cases
4. Identify test categories needed:
   - Unit: pure functions, business logic
   - Integration: service + DB, API + service
   - E2E: full user flows (if configured)
   - Contract: API contracts (if microservices)

### AC → Test Mapping Table

Build this table before writing a single test. Every AC must have at least one row.

```markdown
| AC ID | AC Summary | Test Name | File | Category |
|-------|------------|-----------|------|----------|
| PROJ-123-AC-1 | Valid login returns token | [PROJ-123-AC-1] login_validCreds_returnsToken | auth.service.test.ts | Unit |
| PROJ-123-AC-2 | Invalid password returns 401 | [PROJ-123-AC-2] login_wrongPassword_throws401 | auth.service.test.ts | Unit |
| PROJ-123-AC-3 | Login endpoint at POST /auth/login | [PROJ-123-AC-3] POST /auth/login returns 200 | auth.controller.test.ts | Integration |
| PROJ-123-AC-4 | Token expires in 1h | [PROJ-123-AC-4] login_tokenExpiry_is3600s | auth.service.test.ts | Unit |
```

**Rule**: The AC ID must appear verbatim in the test name (description string). This is what makes tests searchable by requirement.

## AC ID Annotation Convention

The AC ID must be embedded in the test description string — this works across all languages and test runners without requiring plugins or decorators.

### TypeScript / JavaScript (Jest, Vitest)
```typescript
it('[PROJ-123-AC-1] returns a signed token when credentials are valid', async () => {
  // ...
});

it('[PROJ-123-AC-2] throws UnauthorizedError when password is incorrect', async () => {
  // ...
});
```

### Python (pytest)
```python
def test_PROJ_123_AC_1_returns_token_for_valid_credentials():
    # ...

def test_PROJ_123_AC_2_raises_401_for_wrong_password():
    # ...
```
Or using `pytest.mark`:
```python
@pytest.mark.ac("PROJ-123-AC-1")
def test_login_returns_token():
    # ...
```

### Java (JUnit 5)
```java
@Test
@DisplayName("[PROJ-123-AC-1] returns token when credentials are valid")
void returnsToken_whenCredentialsAreValid() { ... }

@Test
@Tag("PROJ-123-AC-2")
@DisplayName("[PROJ-123-AC-2] throws 401 for wrong password")
void throws401_whenPasswordIsWrong() { ... }
```

### Go
```go
func TestPROJ123AC1_Login_ValidCredentials_ReturnsToken(t *testing.T) {
    // ...
}
```

### Why description embedding over decorators alone?
Decorators/tags are runner-specific and stripped from CI logs. Putting the ID in the description means:
- It appears in every test output, failure message, and CI log
- It is grep-able: `grep "PROJ-123-AC-2" coverage-report.txt`
- No tooling dependency

---

## Test Writing Standards

### File Structure
```
src/
├── auth/
│   ├── auth.service.ts
│   ├── auth.service.test.ts      ← unit tests
│   └── auth.controller.test.ts   ← integration tests
└── __tests__/
    └── e2e/
        └── auth.e2e.test.ts      ← E2E tests
```

### Unit Test Template (TypeScript/Jest)
```typescript
import { AuthService } from './auth.service';
import { createMockUserRepository } from '../testing/factories/user.factory';

describe('AuthService', () => {
  let authService: AuthService;
  let mockUserRepo: jest.Mocked<UserRepository>;

  beforeEach(() => {
    mockUserRepo = createMockUserRepository();
    authService = new AuthService(mockUserRepo, mockJwtService);
  });

  afterEach(() => jest.clearAllMocks());

  describe('login', () => {
    describe('when credentials are valid', () => {
      it('returns an AuthToken with non-empty token string', async () => {
        // ...
      });

      it('returns token with expiresIn of 3600', async () => {
        // ...
      });
    });

    describe('when credentials are invalid', () => {
      it('throws UnauthorizedError for wrong password', async () => {
        // ...
      });

      it('throws UnauthorizedError for non-existent user', async () => {
        // ...
      });

      it('throws ValidationError for malformed email', async () => {
        // ...
      });
    });

    describe('edge cases', () => {
      it('handles email with uppercase characters', async () => {
        // ...
      });

      it('handles email with leading/trailing whitespace', async () => {
        // ...
      });
    });
  });
});
```

### Integration Test Template
```typescript
describe('POST /auth/login', () => {
  it('returns 200 with token for valid credentials', async () => {
    const response = await request(app)
      .post('/auth/login')
      .send({ email: 'test@example.com', password: 'validPass123' });

    expect(response.status).toBe(200);
    expect(response.body.token).toBeDefined();
  });

  it('returns 401 for invalid credentials', async () => {
    const response = await request(app)
      .post('/auth/login')
      .send({ email: 'test@example.com', password: 'wrongPass' });

    expect(response.status).toBe(401);
  });

  it('returns 400 for missing email field', async () => {
    const response = await request(app)
      .post('/auth/login')
      .send({ password: 'password123' });

    expect(response.status).toBe(400);
    expect(response.body.error).toBe('VALIDATION_ERROR');
  });
});
```

## Test Execution

### Run Tests
```bash
# From config/project.yaml commands.test
[test command]

# With coverage
[test command] --coverage

# Watch mode (development)
[test command] --watch

# Single file
[test command] path/to/file.test.ts
```

### Failure Analysis Protocol
When a test fails:
1. Read the full error message and stack trace
2. Identify: is this a test bug or a code bug?
3. If code bug: return to Development Agent with failure details
4. If test bug: fix the test, document why
5. Re-run to confirm fix

**Failure Report Format:**
```
TEST FAILURE ANALYSIS
━━━━━━━━━━━━━━━━━━━━
File: auth.service.test.ts:47
Test: login > when credentials are valid > returns token

Error: Expected token to be defined, received undefined

Root Cause: AuthService.login() returns undefined because the
            JWT signing call is missing an await.

Fix Needed: In auth.service.ts:23, change:
  const token = jwtService.sign(payload);
  to:
  const token = await jwtService.sign(payload);

Type: CODE BUG (not test bug)
```

### Coverage Report
After running:
```
Coverage Summary
━━━━━━━━━━━━━━━
Statements:  87% (threshold: 80%) ✅
Branches:    82% (threshold: 75%) ✅
Functions:   91% (threshold: 85%) ✅
Lines:       88% (threshold: 80%) ✅

Uncovered lines:
  src/auth/auth.service.ts: 45-47 (error handling branch)
  → Add test for database connection failure
```

### Traceability Report

After every test run, produce a traceability report and check for AC gaps:

```
AC Traceability Report — PROJ-123
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
| AC ID          | Summary                        | Tests | Status |
|----------------|--------------------------------|-------|--------|
| PROJ-123-AC-1  | Valid login returns token      | 2     | ✅     |
| PROJ-123-AC-2  | Invalid password returns 401   | 1     | ✅     |
| PROJ-123-AC-3  | POST /auth/login endpoint      | 3     | ✅     |
| PROJ-123-AC-4  | Token expires in 1h            | 1     | ✅     |
| PROJ-123-AC-5  | Rate limit after 5 failed tries| 0     | ❌ NO TEST |

⚠️  1 AC with no test coverage: PROJ-123-AC-5
    → Write at least one test referencing [PROJ-123-AC-5] before PR
```

**If any AC has 0 tests**: block hand-off to Review Agent and report which ACs are uncovered.

### Update Requirements Doc

After tests are written and passing, fill in the Traceability Matrix in `docs/designs/TICKET-ID-requirements.md`:

```markdown
| AC ID | AC Summary | Test File | Test Name | Status |
|-------|------------|-----------|-----------|--------|
| PROJ-123-AC-1 | Valid login returns token | auth.service.test.ts | [PROJ-123-AC-1] login_validCreds_returnsToken | ✅ Passing |
| PROJ-123-AC-2 | Invalid password returns 401 | auth.service.test.ts | [PROJ-123-AC-2] login_wrongPassword_throws401 | ✅ Passing |
```

### Post Traceability to Jira

If `config/integrations.yaml > issueTracking.provider` is configured, post the traceability matrix as a comment on the Jira ticket:

```
mcp__jira__add_comment(
  issue_key: "PROJ-123",
  body: """
  ✅ AC Traceability Matrix (automated by PDLC Testing Agent)

  | AC | Tests | Status |
  |----|-------|--------|
  | PROJ-123-AC-1: Valid login returns token | auth.service.test.ts:[PROJ-123-AC-1] (2 tests) | ✅ |
  | PROJ-123-AC-2: Invalid password → 401 | auth.service.test.ts:[PROJ-123-AC-2] (1 test) | ✅ |
  ...

  Coverage: 87% | All ACs covered: YES
  """
)
```

This makes the test coverage visible directly in the Jira ticket, no need to open the codebase.
