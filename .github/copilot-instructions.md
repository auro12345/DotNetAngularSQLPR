# Workspace Framework

Load and follow `config/project.yaml` before implementation, review, testing, or architecture work. Load `config/integrations.yaml` for external tool settings and `config/learnings.yaml` for team-specific review and development rules.

Use the agents under `agentic-pdlc-framework/agents/` for the matching PDLC phase. The active repository is a layered ASP.NET Core 8 API with SQL Server/Entity Framework Core and an Angular 22 client under `product-management-ui/`.

Use `ProductManagement.sln` for backend builds and tests, and `npm --prefix product-management-ui` for frontend commands. Preserve the Domain, Application, Infrastructure, and API boundaries; keep controllers thin and do not expose EF entities as API contracts.

GitHub integration points to `auro12345/DotNetAngularSQLPR` on `main`. Jira, Confluence, Figma, Slack, and monitoring integrations are disabled until their account-specific settings are supplied.

# Azure

- @azure Rule - Use Azure Tools - When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Best Practices - When handling requests related to Azure, always invoke your `azmcp_bestpractices_get` tool first.
- @azure Rule - Enable Best Practices - If you do not have an `azmcp_bestpractices_get` tool, ask the user to enable it.
