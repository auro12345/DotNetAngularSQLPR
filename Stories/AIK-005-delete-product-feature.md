# AIK-005: Delete Product Feature

  -----------------------------------------------------------------------
  Field                               Value
  ----------------------------------- -----------------------------------
  **Key**                             AIK-005 (proposed; not created in
                                      Jira)

  **Type**                            Story

  **Epic**                            AI Kata

  **Priority**                        TBD

  **Story points**                    TBD

  **Status**                          Draft

  **Labels**                          `product-management`, `delete`,
                                      `api`, `angular`, `database`,
                                      `git`, `pull-request`
  -----------------------------------------------------------------------

## User story

As a user managing products, I want to delete a product from the Product
Management application so that obsolete or unwanted products can be
removed from the database and no longer appear in the UI.

## Context

The existing Product Management solution uses ASP.NET Core 8, Angular
22, and SQL Server with a layered architecture. The product list already
supports viewing and editing products. Add deletion across the API,
application, domain/repository, database, and Angular UI layers,
following the existing project structure and conventions.

## Scope

**In**

-   Add a backend endpoint to delete a product by ID.
-   Implement deletion through the existing application service and
    repository layers.
-   Persist the deletion in SQL Server so the product remains deleted
    after the API restarts.
-   Add a **Delete** button beside the existing **Edit** button for
    every product row.
-   Ask the user to confirm before deleting a product.
-   After successful confirmation and API deletion, remove the product
    from the UI and refresh summary values.
-   Display useful success and error messages.
-   Create a feature branch, push it, and create a Pull Request (PR) to
    merge the changes into `main`.

**Out**

-   Bulk deletion.
-   Soft deletion, unless the existing data model already requires it.
-   Changes to unrelated product creation, reading, or editing behavior.
-   Unrequested UI redesign or unrelated refactoring.

## Git workflow

1.  Start from an up-to-date `main` branch.
2.  Create a dedicated feature branch with a suitable name, for example
    `feature/delete-product`.
3.  Make and test all backend, database, and Angular changes on that
    feature branch. Do not commit the feature directly to `main`.
4.  Commit the changes with a clear message, for example
    `Add product delete API and UI`.
5.  Push the feature branch to the remote repository.
6.  Create a Pull Request (PR) from the feature branch into `main`.
7.  Include a concise PR description covering the API endpoint, SQL
    Server persistence, Angular confirmation/UI behavior, and tests
    performed.
8.  Request a reviewer and wait for approval. Merge the PR into `main`
    only after the required review and checks pass.
9.  Do not merge the PR automatically or bypass repository
    branch-protection/review policies.

Example Git commands (run from the solution root, adjusting the remote
name if necessary):

``` bash
git switch main
git pull origin main
git switch -c feature/delete-product
# Make changes, then:
git add .
git commit -m "Add product delete API and UI"
git push -u origin feature/delete-product
```

Create the PR using the repository host's UI or the team's approved
workflow. If the branch is based on an already-current local `main`,
follow the team's normal synchronization process before creating the PR.

## Backend requirements

-   Add a `DELETE /api/Products/{id}` endpoint to `ProductsController`.
-   Use an integer route constraint for the ID, consistent with the
    existing endpoint style.
-   Return `204 No Content` when the product is deleted successfully.
-   Return `404 Not Found` when the specified product does not exist.
-   Keep the controller thin: delegate the operation to the application
    service and translate the result into the appropriate HTTP response.
-   Add a delete operation to `IProductService` and implement it in
    `ProductService`.
-   Add a delete operation to `IProductRepository` and implement it in
    the SQL Server-backed repository.
-   Ensure the deletion is saved to SQL Server through Entity Framework
    Core and is not merely removed from a temporary in-memory
    collection.
-   Register or update dependency injection only where required by the
    existing architecture.
-   Preserve the existing Clean Architecture dependency direction and
    avoid placing database logic in the API controller.
-   Use asynchronous methods if the existing repository/service/API
    conventions are asynchronous; otherwise keep the implementation
    consistent with the current project and avoid unrelated refactoring.
-   Do not change existing endpoint contracts or product fields
    unnecessarily.

## Angular UI requirements

-   Update the existing product table in `product-management.html`.
-   Add a **Delete** button immediately beside the existing **Edit**
    button in the Action column for each product row.
-   Keep the current layout, styling approach, and responsive behavior.
    Use the project's existing plain CSS/Flexbox approach; do not
    introduce a UI framework.
-   Add a delete method to the existing `ProductService` that calls
    `DELETE /api/Products/{id}`.
-   Add a delete handler to `ProductManagement` (or the current
    product-list component) that:
    1.  Prompts the user for confirmation, identifying the selected
        product by name and/or ID.
    2.  Does not call the API if the user cancels.
    3.  Calls the delete API only after confirmation.
    4.  Removes the product from the displayed list only after the API
        confirms successful deletion, or reloads the list from the API.
    5.  Updates the total product count and total inventory value.
    6.  Shows a success message after deletion.
    7.  Shows a useful error message if the API request fails and leaves
        the UI consistent with the server state.
-   Prevent repeated submissions for the same product while its delete
    request is in progress, where practical.
-   Use the existing confirmation approach or the browser's native
    `window.confirm()` if no confirmation component is already present.
    Do not add a new dependency solely for confirmation.
-   Ensure the new button is accessible, has an understandable label,
    and is styled consistently with the existing Edit button.

## Database and persistence

-   Use the existing SQL Server database and EF Core configuration.
-   Ensure the repository used by the API is the database-backed
    repository, not `InMemoryProductRepository`.
-   Do not recreate or reset the database as part of this feature.
-   Verify that deleting a product removes the corresponding database
    row and that the deletion remains effective after restarting the
    API.
-   Handle a missing product without throwing an unhandled exception.
-   If the existing SQL Server database/schema is not yet initialized,
    document the required EF Core migration/database-update commands
    rather than silently switching back to in-memory storage.

## Acceptance criteria

-   [ ] A feature branch named suitably for the delete feature is
    created from `main`.
-   [ ] The backend exposes `DELETE /api/Products/{id}`.
-   [ ] Deleting an existing product returns `204 No Content` and
    removes its row from SQL Server.
-   [ ] Deleting a non-existent product returns `404 Not Found`.
-   [ ] The Angular Products table displays a **Delete** button beside
    **Edit** for every product.
-   [ ] Clicking **Delete** prompts for confirmation and identifies the
    product being deleted.
-   [ ] Cancelling the confirmation leaves the database and UI unchanged
    and sends no delete request.
-   [ ] Confirming deletion calls the API and removes the product from
    the UI only after successful deletion.
-   [ ] The total product count and total inventory value update
    correctly after deletion.
-   [ ] A success message is displayed after successful deletion.
-   [ ] If deletion fails, the UI displays a useful error and does not
    falsely remove the product.
-   [ ] Deletion persists after stopping and restarting the API.
-   [ ] Existing list, create, and update functionality continues to
    work.
-   [ ] Automated tests cover successful deletion, missing IDs,
    repository persistence, confirmation cancellation, successful UI
    removal, and API failure handling as appropriate for the current
    test setup.
-   [ ] Changes are committed and pushed to the feature branch.
-   [ ] A Pull Request is created from the feature branch into `main`,
    with a clear description and reviewer requested.
-   [ ] The PR is merged only after required review and checks pass.

## Verification checklist

1.  Run the backend and confirm the Swagger UI lists
    `DELETE /api/Products/{id}`.
2.  Delete an existing product through Swagger and verify the response
    is `204 No Content`.
3.  Query the SQL Server `Products` table to verify the row was deleted.
4.  Refresh the Angular page and confirm the product remains absent.
5.  Restart the API and refresh again to confirm the deletion persists.
6.  In Angular, click **Delete** and cancel; verify the product remains.
7.  Click **Delete** again and confirm; verify the product disappears,
    totals update, and a success message appears.
8.  Test a missing ID and a failed API request to verify the expected
    error handling.
9.  Run the existing build and test commands for both backend and
    frontend.
10. Review the Git diff to ensure no unrelated changes are included,
    then push the feature branch and open the PR.

## Assessment focus

-   Correct layered implementation and SQL Server persistence.
-   Appropriate HTTP status codes and missing-record handling.
-   Clear confirmation UX and accurate UI state after success or
    failure.
-   Tests, maintainability, and consistency with the existing
    architecture.
-   Correct feature-branch and Pull Request workflow.

## Dependencies

-   Existing Product Management solution using ASP.NET Core 8, Angular
    22, EF Core, and SQL Server.
-   Existing product entity, DTOs, service/repository abstractions, API
    controller, and Angular product table.
-   A configured SQL Server database and database-backed repository
    registration.
-   Access to the remote Git repository and permission to push a feature
    branch and create a Pull Request.
