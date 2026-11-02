# Tech Stack Guide

Language and framework-specific conventions applied by PDLC agents. All conventions are also configurable via `config/project.yaml > codingStandards.conventions`.

## React + JavaScript

**Best for:** SPAs, internal dashboards, marketing sites

### Component Structure
```jsx
// Named export, no React.FC, props at top
const propTypes = {
  userId: PropTypes.string.isRequired,
  onUpdate: PropTypes.func.isRequired,
};

export function UserProfile({ userId, onUpdate }) {
  const { data: user } = useUser(userId);
  return <div>{user?.name}</div>;
}
```

### File Conventions
- Components: `PascalCase.jsx` (e.g., `UserProfile.jsx`)
- Hooks: `use[Name].js` (e.g., `useUser.js`)
- Utils: `camelCase.js` (e.g., `formatDate.js`)
- Tests: `ComponentName.test.jsx` (co-located)

### State Management Hierarchy
1. **Local state**: `useState` for component-local UI state
2. **Server state**: TanStack Query for anything from an API
3. **Shared UI state**: React Context for theme, locale, auth
4. **Complex client state**: Zustand (avoid Redux unless already in project)

### Testing
```javascript
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

test('shows user name after loading', async () => {
  render(<UserProfile userId="123" />);
  await screen.findByText('Alice Johnson');
});
```

### ESLint Config (recommended)
```json
{
  "extends": ["eslint:recommended", "plugin:react/recommended", "plugin:react-hooks/recommended"]
}
```

---

## React + TypeScript (Next.js)

**Best for:** Full-stack apps, e-commerce, SaaS products

### TypeScript Conventions
```typescript
// Interface for props (above component)
interface UserCardProps {
  user: User;
  onDelete: (id: string) => void;
}

// Named export, type props inline
export function UserCard({ user, onDelete }: UserCardProps) {
  return <div onClick={() => onDelete(user.id)}>{user.name}</div>;
}

// Never: React.FC, export default (except Next.js pages)
// Never: any type
// Use: unknown + type narrowing instead of any
```

### Next.js App Router Conventions
```typescript
// app/users/[id]/page.tsx — Server Component (default)
export default async function UserPage({ params }: { params: { id: string } }) {
  const user = await getUserById(params.id);  // Direct DB call OK in Server Component
  return <UserProfile user={user} />;
}

// 'use client' only when: useState, useEffect, event handlers, browser APIs
'use client';
export function UserForm() { ... }
```

### Data Access Pattern (Server Components)
```
Server Component → Service Layer → Repository → Prisma → Database
Never: Component → Prisma (skip service/repository layers)
```

### Error Handling
```typescript
// app/users/[id]/error.tsx — catches errors from page.tsx
'use client';
export default function Error({ error, reset }: { error: Error; reset: () => void }) {
  return <button onClick={reset}>Try again: {error.message}</button>;
}
```

---

## Node.js + Express (REST API)

**Best for:** Backend APIs, microservices

### Controller → Service → Repository Pattern
```typescript
// controller: only orchestrates, no logic
export class UserController {
  constructor(private readonly userService: UserService) {}

  async createUser(req: Request, res: Response): Promise<void> {
    const dto = await validateDto(CreateUserDto, req.body);
    const user = await this.userService.createUser(dto);
    res.status(201).json(user);
  }
}

// service: business logic
export class UserService {
  async createUser(dto: CreateUserDto): Promise<User> {
    const existing = await this.userRepo.findByEmail(dto.email);
    if (existing) throw new ConflictError('Email already registered');
    return this.userRepo.create(dto);
  }
}

// repository: only data access
export class UserRepository {
  async findByEmail(email: string): Promise<User | null> {
    return this.db.query('SELECT * FROM users WHERE email = $1', [email]);
  }
}
```

### Error Classes
```typescript
export class AppError extends Error {
  constructor(
    message: string,
    public readonly statusCode: number,
    public readonly code: string,
  ) { super(message); }
}

export class NotFoundError extends AppError {
  constructor(resource: string) {
    super(`${resource} not found`, 404, 'NOT_FOUND');
  }
}
```

### Middleware Order
```typescript
app.use(helmet());          // Security headers
app.use(cors(corsOptions)); // CORS
app.use(express.json());    // Body parsing
app.use(requestLogger);     // Logging
app.use('/api', router);    // Routes
app.use(errorHandler);      // Error handling (LAST)
```

---

## Python + FastAPI

**Best for:** Data APIs, ML model serving, async services

### Route Handlers (thin, like controllers)
```python
@router.post("/users", response_model=UserResponse, status_code=201)
async def create_user(
    dto: CreateUserRequest,
    user_service: UserService = Depends(get_user_service),
) -> UserResponse:
    return await user_service.create_user(dto)
```

### Type Hints and Pydantic
```python
from pydantic import BaseModel, EmailStr, Field

class CreateUserRequest(BaseModel):
    email: EmailStr
    name: str = Field(..., min_length=1, max_length=100)
    role: Literal["admin", "user"] = "user"

class UserResponse(BaseModel):
    id: str
    email: str
    name: str
    created_at: datetime

    class Config:
        from_attributes = True
```

### Dependency Injection
```python
# Dependencies declared, not instantiated in handlers
def get_user_service(
    db: AsyncSession = Depends(get_db),
    cache: Redis = Depends(get_cache),
) -> UserService:
    return UserService(UserRepository(db), cache)
```

### Testing with Pytest
```python
@pytest.mark.asyncio
async def test_create_user_returns_201(client: AsyncClient):
    response = await client.post("/users", json={
        "email": "test@example.com",
        "name": "Test User"
    })
    assert response.status_code == 201
    assert response.json()["email"] == "test@example.com"
```

---

## Java + Spring Boot

**Best for:** Enterprise applications, complex business domains

### Constructor Injection (not field injection)
```java
@Service
public class UserService {
    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;

    // Constructor injection — testable, immutable
    public UserService(UserRepository userRepository, PasswordEncoder passwordEncoder) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
    }
}
```

### Controller → Service → Repository
```java
@RestController
@RequestMapping("/api/users")
@RequiredArgsConstructor
public class UserController {
    private final UserService userService;

    @PostMapping
    @ResponseStatus(HttpStatus.CREATED)
    public UserResponse createUser(@Valid @RequestBody CreateUserRequest request) {
        return userService.createUser(request);
    }
}
```

### Exception Handling
```java
@ControllerAdvice
public class GlobalExceptionHandler {
    @ExceptionHandler(UserNotFoundException.class)
    @ResponseStatus(HttpStatus.NOT_FOUND)
    public ErrorResponse handleUserNotFound(UserNotFoundException ex) {
        return new ErrorResponse("USER_NOT_FOUND", ex.getMessage());
    }
}
```

### Testing
```java
@Test
@DisplayName("createUser throws ConflictException when email already exists")
void createUser_withExistingEmail_throwsConflictException() {
    // Arrange
    when(userRepository.existsByEmail("existing@test.com")).thenReturn(true);

    // Act + Assert
    assertThrows(ConflictException.class,
        () -> userService.createUser(new CreateUserRequest("existing@test.com", "Alice")));
}
```

---

## Go

**Best for:** CLI tools, high-performance services, infrastructure

### Package Structure
```
cmd/
  server/main.go        # Entry point
internal/
  user/
    handler.go          # HTTP handlers
    service.go          # Business logic
    repository.go       # Data access
    model.go            # Types
pkg/
  errors/               # Shared error types
  middleware/           # HTTP middleware
```

### Error Handling
```go
// Wrap errors with context, always
user, err := r.userRepo.FindByEmail(ctx, email)
if err != nil {
    return nil, fmt.Errorf("userService.Login: find user by email: %w", err)
}

// Typed sentinel errors for known cases
var ErrUserNotFound = errors.New("user not found")

if errors.Is(err, ErrUserNotFound) {
    return nil, ErrUnauthorized  // Don't leak "not found" to client
}
```

### Testing
```go
func TestLogin_ValidCredentials_ReturnsToken(t *testing.T) {
    repo := &mockUserRepo{user: testUser}
    svc := NewAuthService(repo, testJWTKey)

    token, err := svc.Login(context.Background(), "user@test.com", "password")

    require.NoError(t, err)
    assert.NotEmpty(t, token)
}
```

---

## Adding a New Tech Stack

1. Create a new example config: `config/examples/[tech-stack].yaml`
2. Add detection rules to `agentic-pdlc-framework/tools/tech-stack-detector.md`
3. Add language-specific commands to `agentic-pdlc-framework/skills/code-analysis.md` and `agentic-pdlc-framework/skills/test-execution.md`
4. Add conventions section to this document
5. Update `agentic-pdlc-framework/config/project.template.yaml` if new fields are needed
