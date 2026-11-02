Run the Project Discovery Agent on this project.

## What to do

1. Scan the project's root directory for all build/config/framework files following the detection logic in `agentic-pdlc-framework/agents/01-project-discovery-agent.md` and `agentic-pdlc-framework/tools/tech-stack-detector.md`.

2. Detect: primary language, framework, package manager, test runner, linter, formatter, type checker, CI/CD system, deployment target, databases, ORM, and repository type.

3. Check if `config/project.yaml` exists:
   - If it does NOT exist: create it from `agentic-pdlc-framework/config/project.template.yaml`, filling in all auto-detectable fields.
   - If it DOES exist: compare detected values against existing values. Update any `tech.*` or `commands.*` fields that have changed or are still set to `"unknown"`. Never overwrite `project.name`, `project.description`, `project.domain`, or any `integrations.*` fields.

4. Resolve all runnable commands in `commands.*` and verify they correspond to scripts or binaries available in the project. Set each to the exact shell command string.

5. Print the discovery summary in this format:
```
╔══════════════════════════════════════════════════╗
║          PROJECT DISCOVERY COMPLETE              ║
╠══════════════════════════════════════════════════╣
║ Project:     [name]                              ║
║ Domain:      [domain]                            ║
║ Language:    [primaryLanguage]                   ║
║ Framework:   [framework]                         ║
║ Tests:       [testFramework]                     ║
║ Linter:      [linter]                            ║
║ CI/CD:       [ciCd]                              ║
║ Config:      config/project.yaml [created/updated]║
╚══════════════════════════════════════════════════╝
```

6. If any fields remain `"unknown"` or still contain `"TODO:"`, list them as requiring manual input:
```
⚠️  Fields requiring manual review:
  - project.domain: set to "unknown" — please update
  - integrations.jira.projectKey: not detected — required for /start, /requirements, /fix
```

7. If the project appears to be new (empty or scaffold only), set `project.status: "new-project"` and suggest an appropriate example config from `agentic-pdlc-framework/config/examples/`.
