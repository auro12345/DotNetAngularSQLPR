# Agent: ErrorHandling Fix Agent

## Role

The ErrorHandling Fix Agent applies systematic, safe corrections to error handling in a single service (or all services) under `apps/`. It **requires a review report to exist first** — the report is both the gate and the work list. It adds `buildErrorResponse()` and `logError()` to each service's existing `helperFunctions.ts`, then rewrites only the handler catch blocks, CORS omissions, and logging issues that appear in the review report — without touching anything else.

## Activation

- When user runs `/errorHandling-fix [service-name|all]`
- Example: `/errorHandling-fix bleed-service`
- Example: `/errorHandling-fix all`

## Prerequisite Gate — Review Report Must Exist

**Before making any file change**, check:

```
docs/error-review/{service-name}-error-review.md
```

If the file **does not exist**, do not make any code changes. Instead, ask the user:

```
No review report found for {service-name}.
Expected: docs/error-review/{service-name}-error-review.md

Would you like me to run /errorHandling-review {service-name} now?
  [Y] Yes — run the review, show the report, then ask whether to proceed with fixes
  [N] No  — stop here; I will run /errorHandling-review manually first
```

- If the user answers **Y**: invoke the Error Review Agent (agent 11) for the same service, display the report summary, then ask: _"Review complete. Proceed with /errorHandling-fix {service-name}?"_ — only continue with fixes if the user confirms.
- If the user answers **N**: stop and remind them to run `/errorHandling-review {service-name}` when ready.

If the file **exists**, load it. The findings table in the report is the authoritative work list — only fix what is listed there. If a developer has manually deleted a finding from the report before running `/errorHandling-fix`, that item must be skipped.

For `all` services: check each service's report individually. For any service whose report is missing, ask the user the same Y/N question above before proceeding with that service. Do not assume the answer from a previous service applies to the next one.

## Inputs

- Target service name (or `all`) passed as argument
- **Required**: `docs/error-review/{service-name}-error-review.md` — the review report (gate + work list)
- Source files under `apps/{service}/src/` (excluding `node_modules`, `*.d.ts`, tests)

## Outputs

- Modified `helperFunctions.ts` (error helper functions appended)
- Modified handler files (catch blocks updated)
- Modified service/DB layer files (structured logging, DynamoDB classification)
- Git-ready changes (no commit — user commits)

---

## Services Reference

Valid service names:

- `bleed-service`
- `comp-reminder-service`
- `accessMangement-service`
- `pharmacy-service`
- `patient-service`
- `shared-service`
- `formMetadata-service`
- `athena-service`
- `comp-llm-async-processing-job`
- `comp-report-service`
- `surgery-service`
- `lookup-service`

---

## Standard Error Response Contract

Every error response body must match this shape — never deviate:

```json
{
  "code": "INTERNAL_SERVER_ERROR",
  "message": "An unexpected error occurred. Please try again or contact support.",
  "errorMessage": "original error message string",
  "errorType": "ErrorClassName",
  "correlationId": "abc-123"
}
```

- `code` — stable, SCREAMING_SNAKE_CASE string. UI keys off this.
- `message` — safe human-readable copy. No stack traces, no AWS internals, no table names.
- `errorMessage` — `error instanceof Error ? error.message : String(error)`. Safe string, not the raw object.
- `errorType` — `error instanceof Error ? error.constructor?.name ?? error.name : typeof error`. Safe string.
- `correlationId` — `event.requestContext?.requestId ?? 'unknown'`
- `details` — optional `string[]` for validation errors only (400s)

**What must never appear in a response body:** raw `error` objects, `error.stack`, AWS ARNs, table names, environment variables.

---

## Fix Process

### Step 0: Gate Check + Load Work List

```bash
REPORT="docs/error-review/$SERVICE_NAME-error-review.md"

# Check report exists before touching any source file
if [ ! -f "$REPORT" ]; then
  # Prompt user — see instructions below
fi
```

Read the report and extract the findings table. Build a work list: a mapping of `{ checkId, severity, filePath, lineNumber }` for each row. This is the only list of items that will be touched — nothing outside it.

### Step 1: Resolve Target

```bash
SERVICE_NAME="<argument>"
SERVICE_PATH="apps/$SERVICE_NAME/src"
HELPERS_FILE="$SERVICE_PATH/app/util/helperFunctions.ts"
```

If `all`, repeat Steps 0–7 for each service in sequence (skip services with no report).

### Step 2: Extend `helperFunctions.ts`

Read the existing `helperFunctions.ts`. Check if `buildErrorResponse` already exists — if so, skip adding it (idempotent). Otherwise, append the following block to the end of the file.

**Also check existing imports** — only add the import lines if `APIGatewayProxyEvent`, `APIGatewayProxyResult`, and `responseHeaders` are not already imported.

```typescript
// ─── Error Handling Utilities ────────────────────────────────────────────────

import { APIGatewayProxyEvent, APIGatewayProxyResult } from 'aws-lambda';
import { responseHeaders } from '../response/headers/corsHeaders';

export interface ApiError {
    code: string;
    message: string;
    correlationId: string;
    details?: string[];
}

export const logError = (operation: string, error: unknown, context: Record<string, unknown> = {}): void => {
    console.error(
        JSON.stringify({
            operation,
            errorMessage: error instanceof Error ? error.message : String(error),
            errorName: error instanceof Error ? error.name : typeof error,
            ...context,
        }),
    );
};

export const buildErrorResponse = (
    event: APIGatewayProxyEvent,
    error: unknown,
    operation: string,
    context: Record<string, unknown> = {},
): APIGatewayProxyResult => {
    const correlationId = event.requestContext?.requestId ?? 'unknown';

    logError(operation, error, { correlationId, ...context });

    return {
        statusCode: 500,
        headers: responseHeaders(event),
        body: JSON.stringify({
            code: 'INTERNAL_SERVER_ERROR',
            message: 'An unexpected error occurred. Please try again or contact support.',
            errorMessage: error instanceof Error ? error.message : String(error),
            errorType: error instanceof Error ? error.constructor?.name ?? error.name : typeof error,
            correlationId,
        } as ApiError),
    };
};

export const buildValidationErrorBody = (event: APIGatewayProxyEvent, message: string, details?: string[]): string => {
    const correlationId = event.requestContext?.requestId ?? 'unknown';
    return JSON.stringify({
        code: 'VALIDATION_ERROR',
        message,
        correlationId,
        ...(details ? { details } : {}),
    } as ApiError);
};
```

**Special case — `formMetadata-service`:** This service already has a `handleError(error, context?)` at the bottom of `helperFunctions.ts`. Replace it with `buildErrorResponse` above. Update any callers of `handleError` to use `buildErrorResponse` instead (they will need `event` as the first argument).

### Step 3: Fix Handler Catch Blocks

Scan all files under `{service}/src/app/handlers/` (`.ts`, not `.d.ts`, not test files).

For each `catch` block:

**BEFORE (pattern to replace):**

```typescript
} catch (error) {
    console.error('Error : handlerName ', error);
    return {
        statusCode: 500,
        body: JSON.stringify({ message: 'Internal Server Error', error }),
    };
}
```

**AFTER:**

```typescript
} catch (error) {
    return buildErrorResponse(event, error, 'handlerName', { /* relevant IDs from the handler scope */ });
}
```

Rules:

1. Add `import { buildErrorResponse } from '../../util/helperFunctions';` (adjust relative path as needed) if not already imported.
2. Derive `operation` from the handler function name (e.g. `postBleedEvent`, `deletePharmacyOrder`).
3. Include relevant IDs in the `context` object (e.g. `{ patientId, bleedEventId }`) if those variables are in scope — but never include PHI values, only IDs.
4. Remove any existing `console.error('Error ...')` line that was logging the full error (the structured log inside `buildErrorResponse` replaces it).
5. Do not change any code above or below the catch block.

**Variant — handlers using `responseError()` with hardcoded message:**

```typescript
// BEFORE
return responseError(
  event,
  JSON.stringify({ message: "Internal Server Error" }),
);

// AFTER
return buildErrorResponse(event, error, "handlerName");
```

**Variant — handlers returning bare object (missing CORS):**

```typescript
// BEFORE
return { statusCode: 500, body: JSON.stringify({ message: "error" }) };

// AFTER
return buildErrorResponse(event, error, "handlerName");
```

### Step 4: Fix Validation Error HTTP Codes

Scan handler and service files for validation checks that return `statusCode: 500` or call `responseError(...)` for input validation failures.

**BEFORE:**

```typescript
if (!patientId) {
  return responseError(event, JSON.stringify({ message: "Missing patientId" }));
}
```

**AFTER:**

```typescript
if (!patientId) {
  return responseBadRequest(
    event,
    JSON.stringify({
      code: "VALIDATION_ERROR",
      message: "Missing required field: patientId",
      correlationId: event.requestContext?.requestId ?? "unknown",
    }),
  );
}
```

### Step 5: Fix Structured Logging in Service/DB Layers

Scan files under `**/services/` and `**/db/` for catch blocks that log the full error object.

Use the `logError` helper added in Step 2. Import it from the service's `helperFunctions.ts` — extend any existing import or add a standalone one (adjust relative path depth as needed).

**BEFORE:**

```typescript
} catch (error) {
    console.error('Service Error : createBleedEvent ', error);
    throw error;
}
```

**AFTER:**

```typescript
} catch (error) {
    logError('createBleedEvent', error);
    throw error;
}
```

Do not remove the `throw error` — service layers must still propagate errors to handlers.

---

## What This Agent Does NOT Change

- Business logic inside try blocks
- Validation logic (only the response code for validation failures)
- SQS/SNS handler catch blocks that re-throw for DLQ retry — those are intentional `throw error` patterns
- Test files
- Any handler that already uses `buildErrorResponse` correctly

---

## Idempotency

Running this agent twice on the same service must produce no further changes (the second run detects that `buildErrorResponse` already exists and all catch blocks already use it).

---

## Output

After completing each service, print:

```
✓ bleed-service — error handling standardised
  Modified: 12 handler files, 3 service files, 1 helperFunctions.ts

  Validation codes corrected: 2
```

---

## Completion Criteria

The agent is done when:

1. `buildErrorResponse` and `logError` exist in the service's `helperFunctions.ts`
2. Zero handler catch blocks return `{ statusCode, body }` without CORS headers
3. Zero instances of `JSON.stringify({...error})` or `JSON.stringify({message, error})` in handler files
4. All validation failures return 400 via `responseBadRequest()`
5. Service/DB layer catch blocks log only `error.message` and `error.name`
