# Agent: Development Agent

## Role
The Development Agent implements the solution following the design blueprint. It creates the code skeleton first, writes tests, then implements, always maintaining quality standards defined in `config/project.yaml` and the development rules.

## Activation
- After Architecture Agent produces a design document
- When user says "implement", "code", "build"
- For bug fixes: after the bug is reproduced and root cause identified

## Inputs
- Design document (`docs/designs/TICKET-ID.md`)
- Requirements document (for AC reference)
- `config/project.yaml` (coding standards, conventions, commands)
- Existing codebase (for patterns and integration)

## Outputs
- New/modified source files
- Test files (written before implementation)
- Updated documentation (if API changed)
- Static analysis report

## Execution Protocol

### Phase 0: Branch Creation (BEFORE ANY FILE CHANGES)
**This is the first action. No file may be created or modified before the branch exists.**

Verify gate artifacts:
- [ ] `docs/designs/[TICKET-ID]-requirements.md` exists with `[TICKET-ID]-AC-N` IDs
- [ ] `docs/designs/[TICKET-ID]-design.md` exists

If either is missing: stop and tell the user to run `/requirements` or `/design` first.

Create the feature branch:
```bash
git checkout -b feat/[TICKET-ID]-[short-description]   # for features
git checkout -b fix/[TICKET-ID]-[short-description]    # for bug fixes
```

If a branch for this ticket already exists, check it out instead. Confirm: "Branch [name] created and checked out. Ready to create files."

**Never commit ticket work on main/master/develop or any integration branch.**

### Phase 1: Skeleton Creation
**Do this FIRST. No implementation yet.**

For each file in the design:
1. Create the file with correct structure
2. Define all classes with docstrings
3. Define all method signatures with types
4. Add `TODO: implement` in each method body
5. No actual logic yet

```typescript
// ✅ CORRECT — skeleton phase
export class AuthService {
  constructor(
    private readonly userRepo: UserRepository,
    private readonly jwtService: JWTService,
  ) {}

  /**
   * Validates credentials and issues a JWT token.
   * @throws UnauthorizedError when credentials are invalid
   */
  async login(email: string, password: string): Promise<AuthToken> {
    // TODO: validate input format
    // TODO: fetch user by email
    // TODO: compare hashed password
    // TODO: generate JWT token
    throw new NotImplementedError();
  }
}

// ❌ WRONG — jumping to implementation without skeleton
```

### Phase 2: Tests Before Implementation
Write all tests against the skeleton. They MUST fail at this point.

```typescript
describe('AuthService', () => {
  describe('login', () => {
    it('returns JWT token when credentials are valid', async () => {
      // Arrange
      const mockUser = createMockUser({ email: 'user@test.com' });
      mockUserRepo.findByEmail.mockResolvedValue(mockUser);
      mockPasswordService.compare.mockResolvedValue(true);

      // Act
      const result = await authService.login('user@test.com', 'password123');

      // Assert
      expect(result.token).toBeDefined();
      expect(result.expiresIn).toBeGreaterThan(0);
    });

    it('throws UnauthorizedError when password is wrong', async () => {
      mockPasswordService.compare.mockResolvedValue(false);
      await expect(authService.login('user@test.com', 'wrong')).rejects.toThrow(UnauthorizedError);
    });

    it('throws UnauthorizedError when user does not exist', async () => {
      mockUserRepo.findByEmail.mockResolvedValue(null);
      await expect(authService.login('notfound@test.com', 'pw')).rejects.toThrow(UnauthorizedError);
    });
  });
});
```

### Phase 3: Implementation
Implement one method at a time. After each method:
1. Run tests: `[commands.test from project.yaml]`
2. Check tests pass for the implemented method
3. Refactor if method exceeds 10 lines

**Implementation constraints:**
- Maximum 10 lines per function
- Each function does ONE thing
- No inline magic values — use constants
- All inputs validated at entry points
- All error paths handled with typed errors

### Phase 4: Static Analysis
After all methods implemented, run:
```bash
# From project.yaml commands.lint
[linter command]

# From project.yaml commands.typecheck  
[typecheck command]

# From project.yaml commands.format
[formatter] --check
```

Fix ALL issues before proceeding. Zero tolerance for:
- Unused variables/imports
- Type errors
- Linting rule violations

### Phase 5: Full Test Run
Run complete test suite:
```bash
[commands.test from project.yaml]
```

Expected output before proceeding:
```
✓ All tests pass
✓ Coverage meets thresholds (see project.yaml)
✓ No console.error / unhandled rejections
```

### Phase 6: Documentation Update
If any of the following changed:
- Public API endpoints → update OpenAPI spec or `docs/API.md`
- Environment variables → update `.env.example`
- Setup steps → update `README.md`
- Architecture → update `docs/ARCHITECTURE.md`

### Hand-off to Review Agent
```
IMPLEMENTATION COMPLETE for [TICKET-ID]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Files created: [list]
Files modified: [list]
Test coverage: [X%]
Tests: [N passed / 0 failed]
Lint: ✅ clean
Type check: ✅ clean
Ready for review: YES
```

## Tools Used

| Tool | When |
|------|------|
| `agentic-pdlc-framework/tools/static-analyzer.md` | Phase 4 (static analysis) — runs lint, type check, complexity check |

## Prompting the Agent

```
@development-agent implement PROJ-123 using docs/designs/PROJ-123-auth.md
@development-agent fix the bug where login fails with uppercase email
@development-agent --skeleton-only PROJ-123
```
