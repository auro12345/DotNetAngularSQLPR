# Requirements: Delete Product Feature

**Ticket**: AIK-005 — Draft
**Analyzed**: 2026-11-02

## Summary

Add deletion of an existing product across the ASP.NET Core API, application service, SQL Server repository, and Angular product table. Deletion must be confirmed in the UI, persist in the database, and report success or failure without leaving the UI out of sync with the server.

## External Sources Analyzed

- Local story: `Stories/AIK-005-delete-product-feature.md`
- Jira and Figma integrations are disabled.
- No linked web sources are present in the story.

## Functional Requirements

- FR1: Expose `DELETE /api/Products/{id}` for integer product IDs.
- FR2: Return `204 No Content` when deletion succeeds and `404 Not Found` when the product does not exist.
- FR3: Delete the product through `IProductService` and `IProductRepository`, persisting through EF Core.
- FR4: Display a Delete button beside Edit for each product row.
- FR5: Confirm deletion using the browser native confirmation dialog and cancel without an API call.
- FR6: Remove the row and refresh totals only after successful API deletion.
- FR7: Show success after deletion and an error after failure without false UI removal.
- FR8: Prevent repeated delete requests for the same product while in progress.

## Non-Functional Requirements

- Keep controllers thin and database operations below the application/API boundaries.
- Do not expose domain entities as API contracts.
- Preserve SQL Server persistence and use the existing database-backed repository registration.
- Keep the Angular implementation type-safe and consistent with the existing plain CSS style.

## Acceptance Criteria

- [ ] **AIK-005-AC-1**: Given an existing product ID, when DELETE is called, then the API returns 204 and the SQL Server row is removed.
  - Source: story backend and database requirements
- [ ] **AIK-005-AC-2**: Given an unknown product ID, when DELETE is called, then the API returns 404 without an unhandled exception.
  - Source: story backend requirements
- [ ] **AIK-005-AC-3**: Given the product list, then each row displays Delete immediately beside Edit and the button is accessible.
  - Source: story Angular UI requirements
- [ ] **AIK-005-AC-4**: Given the user cancels confirmation, then no DELETE request is sent and UI/database state is unchanged.
  - Source: story confirmation behavior
- [ ] **AIK-005-AC-5**: Given confirmation succeeds, then the API deletion completes before the product is removed locally and totals update.
  - Source: story confirmation/UI requirements
- [ ] **AIK-005-AC-6**: Given a successful deletion, then a success message is shown.
  - Source: story UI requirements
- [ ] **AIK-005-AC-7**: Given an API deletion failure, then an error is shown and the product remains in UI state.
  - Source: story API failure handling
- [ ] **AIK-005-AC-8**: Given a confirmed deletion request, then repeated submissions for that product are blocked while it is pending.
  - Source: story UI requirements
- [ ] **AIK-005-AC-9**: Given a deletion in SQL Server, then the row remains deleted after API restart.
  - Source: story persistence requirements
- [ ] **AIK-005-AC-10**: Given existing list/create/update behavior, then deletion changes do not break those flows.
  - Source: story regression acceptance criterion

## Traceability Matrix

| AC ID | AC Summary | Test File | Test Name | Status |
|---|---|---|---|---|
| AIK-005-AC-1 | Existing product deletion returns 204 and persists | ProductManagement.Tests | AIK_005_AC_1_Delete_existingProduct_returnsNoContentAndRepositoryDeletion | Covered |
| AIK-005-AC-2 | Missing product returns 404 | ProductManagement.Tests | AIK_005_AC_2_Delete_missingProduct_returnsNotFound | Covered |
| AIK-005-AC-3 | Delete control is accessible beside Edit | — | Template inspection | Pending |
| AIK-005-AC-4 | Cancel sends no API request | product-management.spec.ts | AIK-005-AC-4 cancellation sends no delete request | Covered |
| AIK-005-AC-5 | Successful response removes row and updates totals | product-management.spec.ts | AIK-005-AC-5 successful deletion reloads products | Covered |
| AIK-005-AC-6 | Success message is shown | — | Component implementation inspection | Pending |
| AIK-005-AC-7 | Failure leaves UI unchanged | product-management.spec.ts | AIK-005-AC-7 API failure leaves the displayed product unchanged | Covered |
| AIK-005-AC-8 | In-progress delete is prevented | — | Component implementation inspection | Pending |
| AIK-005-AC-9 | SQL deletion persists | ProductManagement.Tests | AIK_005_AC_1_existingProduct_isRemovedFromSqlRepositoryContext | Covered |
| AIK-005-AC-10 | Existing flows still work | — | Solution build and Angular build | Pending |

## Out of Scope

- Bulk deletion
- Soft deletion
- Unrelated list/create/update redesign
- Jira issue creation and automatic merge

## Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| SQL Server connection | Technical | Available/configured | Verify through existing app configuration |
| GitHub remote | Technical | Available | Push and PR creation require credentials/permissions |
| Backend test framework | Technical | Not configured | Add focused project only if implementation warrants it |
| Angular test runner | Technical | Configured by dependencies | Existing app spec may be expanded |
