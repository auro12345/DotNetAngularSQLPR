# Agent: ErrorHandling Review Agent

## Role

The ErrorHandling Review Agent audits a single service (or all services) in `apps/` for insecure, inconsistent, or incomplete error handling. It produces a structured markdown report grouped by severity — CRITICAL, HIGH, MEDIUM, LOW — with exact file paths, line numbers, and code snippets. It makes **no code changes**.

## Activation

- When user runs `/errorHandling-review [service-name|all]`
- Example: `/errorHandling-review bleed-service`
- Example: `/errorHandling-review all`

## Inputs

- Target service name (or `all`) passed as argument
- Source files under `apps/{service}/src/` (excluding `node_modules`, `*.d.ts`, `*.test.ts`, `*.spec.ts`)

## Outputs

- Report file: `docs/error-review/{service-name}-error-review.md`
- Console summary with counts per severity

> **Note:** This report is the required gate for `/errorHandling-fix`. The fix agent reads it as its work list — only findings present in the report will be fixed. You can delete individual rows from the report before running `/errorHandling-fix` to exclude specific items from the automated fix.

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

## Review Process

### Step 1: Resolve Target Path

```bash
SERVICE_NAME="<argument>"   # e.g. bleed-service
SERVICE_PATH="apps/$SERVICE_NAME/src"

# If "all", iterate over every directory in apps/
```

### Step 2: Collect Source Files

```bash
find "$SERVICE_PATH" \
  -name "*.ts" \
  -not -path "*/node_modules/*" \
  -not -name "*.d.ts" \
  -not -name "*.test.ts" \
  -not -name "*.spec.ts"
```

### Step 3: Run Each Check

Scan every file. Record each finding with: check ID, severity, file path (relative to repo root), line number, offending code snippet (≤5 lines), and a plain-English description of why it is a problem.

---

## Check Catalogue

### ERR-001 — CRITICAL: Raw error object in response body

**Pattern to detect:**

```typescript
JSON.stringify({ message: ..., error })
JSON.stringify({ ..., error: error })
JSON.stringify({ ..., error: err })
```

**Why it is critical:** Serialising a raw `Error` or AWS SDK error object exposes stack traces, DynamoDB table names, ARNs, and potentially patient data to API clients. It is also often unusable by the UI (serialised Error objects produce `{}`).

**What to look for:**

- Any `JSON.stringify(...)` call inside a `catch` block that includes the caught variable (`error`, `err`, `e`) as a property value

---

### ERR-002 — CRITICAL: Catch block missing CORS headers

**Pattern to detect:**

```typescript
catch (error) {
    return {
        statusCode: 500,   // or any code
        body: ...,
        // NO headers property, or headers without responseHeaders(event)
    };
}
```

**Why it is critical:** API Gateway returns the Lambda response headers verbatim. Without `Access-Control-Allow-Origin`, browsers block the error response, leaving the UI with no error information — effectively a silent failure from the frontend's perspective.

**What to look for:**

- `return { statusCode: ..., body: ... }` inside a `catch` block that does NOT include `headers: responseHeaders(event)` or at minimum an `Access-Control-Allow-Origin` header.
- `return { statusCode: ..., headers: { 'Access-Control-Allow-Origin': '*' }` is acceptable but weaker (note it for ERR-008).
- **Count non-catch early returns as full findings:** Any `return { statusCode: N, body: ... }` statement _anywhere_ in a handler function (including validation early-returns in the try body) that lacks `headers: responseHeaders(event)` is an ERR-002 finding. Add these as full rows in the findings table — do NOT relegate them to footnotes or "related" notes.

---

### ERR-004 — HIGH: Full error object logged (PHI risk)

**Pattern to detect:**

```typescript
console.error("...", error); // error object as second arg
console.error("...", JSON.stringify(error));
console.log("...", error);
```

**Scope — scan ALL source files:** This check applies to every `.ts` file in the service — handlers, services, DB operations, and utilities. Do NOT limit scanning to handler files. Service and utility files (e.g., `services/s3Services/generateSignedUrl.ts`, `services/awsSecretsManagerServices/fetchSecrets.ts`, `services/tokenGenerationService/**`) commonly contain the same violation. Every `catch` block in every file is in scope, including inner `catch` blocks (e.g., a `try/catch` inside an outer `try` block). Both `console.error` and `console.log` with the raw error object as an argument qualify.

**Why it is high:** DynamoDB and AWS SDK errors include the full request payload in their `Item` property. If the payload contains Protected Health Information (PHI), logging the entire error object could violate HIPAA. Only `error.message` and `error.name` are safe to log.

**Exception — acceptable patterns:**

```typescript
console.error('...', error instanceof Error ? error.message : String(error))
console.error(JSON.stringify({ ..., errorMessage: error.message, errorName: error.name }))
```

---

### ERR-005 — MEDIUM: No correlationId in error response body

**Pattern to detect:**

- Error response bodies that do NOT include a `correlationId` or `requestId` field.
- Check `event.requestContext?.requestId` usage in catch blocks.

**Why it is medium:** Without a correlation ID, support must grep CloudWatch logs by time window. A correlation ID lets support find the exact log entry in seconds.

---

### ERR-006 — MEDIUM: Validation error returns 500 instead of 400, or service swallows error causing caller to return 200

**Pattern to detect:**

**Pattern A — Validation returns 500:**

- Input/body validation logic (missing field, invalid enum, invalid format) that returns `statusCode: 500` or calls `responseError(...)` instead of `responseBadRequest(...)`.
- Includes cases where a service function's `validateOptions()`/`validateInput()` throws validation errors that bubble up to a handler catch block returning 500 for all errors — even though those specific errors are client input failures and should be 400.

**Pattern B — Service swallows error, caller returns 200:**

```typescript
// FLAG this in service/utility files:
export const myServiceFn = async () => {
    try { ... return data; }
    catch (error) {
        console.error('...', error);
        // no throw, no explicit return in catch → implicitly returns undefined
    }
};

// The calling handler then does:
const result = await myServiceFn();   // undefined on error
return responseBodyOk(event, result ? JSON.stringify(result) : '');  // 200 with empty body
```

Look for: service/utility functions where the `catch` block only logs (no `throw`, no `return`) — the function implicitly returns `undefined` on the error path. If the calling handler uses the return value in a ternary/conditional that silently falls back to a 200 response, flag the service function's catch block as ERR-006.

**Why it is medium:** For Pattern A — 500 signals a server fault; 400 signals bad client input; misled monitoring. For Pattern B — an unhandled error surfaces as a 200 success response, hiding failures from clients and monitoring entirely.

---

### ERR-008 — LOW: Inconsistent error message format

**Pattern to detect:**

- Some handlers return `{ message: '...' }`, others `{ error: '...' }`, others `{ code: '...', message: '...' }`.
- Flag inconsistencies within the same service.

---

## Output Format

### Console Summary (printed during run)

```
Error Review: bleed-service
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  CRITICAL  2   (ERR-001: 2, ERR-002: 0)
  HIGH      1   (ERR-004: 1)
  MEDIUM    5   (ERR-005: 3, ERR-006: 2)
  LOW       1   (ERR-008: 1)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Report saved: docs/error-review/bleed-service-error-review.md
```

### Report File: `docs/error-review/{service-name}-error-review.md`

```markdown
# Error Handling Review: {service-name}

**Date:** {ISO date}
**Service path:** apps/{service-name}/src/

## Summary

| Severity  | Count |
| --------- | ----- |
| CRITICAL  | N     |
| HIGH      | N     |
| MEDIUM    | N     |
| LOW       | N     |
| **Total** | **N** |

---

## CRITICAL Findings

### [ERR-001] Raw error object in response body

| File                                                              | Line | Snippet                                                             |
| ----------------------------------------------------------------- | ---- | ------------------------------------------------------------------- |
| `apps/bleed-service/src/app/handlers/bleedEvent/addBleedEvent.ts` | 15   | `body: JSON.stringify({ message: 'Internal Server Error', error })` |

**Risk:** Exposes stack traces and AWS internals to API clients.
**Fix:** Use `buildErrorResponse(event, error, 'postBleedEvent')` from `helperFunctions.ts`.

---

## HIGH Findings

...

## MEDIUM Findings

...

## LOW Findings

...

---

## Next Step

This report is the required input for the fix agent.

- To apply **all** fixes above: run `/errorHandling-fix {service-name}`
- To skip specific items: delete those rows from the findings tables above, then run `/errorHandling-fix {service-name}`
- The fix agent will only touch files and lines listed in this report.
```

---

## Completion Criteria

The agent is done when:

1. All source files in the target service(s) have been scanned
2. Report file(s) written to `docs/error-review/`
3. Console summary printed
4. No code has been modified
