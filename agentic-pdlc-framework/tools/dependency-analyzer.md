# Tool: Dependency Analyzer

## Purpose
Analyzes project dependencies for security vulnerabilities, outdated versions, license issues, and bundle impact. Called by Maintenance Agent and Review Agent.

## Inputs
- Package manifest files (package.json, requirements.txt, pom.xml, etc.)
- `config/project.yaml` for allowed licenses and version policies

## Outputs
- Security vulnerability report
- Outdated package report
- License compliance report
- Bundle impact analysis (frontend)

## Security Vulnerability Scan

### JavaScript / TypeScript
```bash
# npm audit (built-in)
npm audit --json 2>/dev/null | jq '{
  vulnerabilities: .vulnerabilities,
  metadata: .metadata
}'

# Output severity levels: critical, high, moderate, low, info

# Fix automatically where safe
npm audit fix

# Force fix (potentially breaking — review manually)
npm audit fix --force
```

### Python
```bash
# pip-audit
pip-audit --format json 2>/dev/null

# Safety (alternative)
safety check --json
```

### Java
```bash
# OWASP Dependency-Check
mvn org.owasp:dependency-check-maven:check -DfailBuildOnCVSS=7

# Gradle
./gradlew dependencyCheckAnalyze
```

### Go
```bash
govulncheck -json ./...
```

## Vulnerability Severity Response Matrix

| Severity | CVSS Score | Required Action | Timeline |
|----------|------------|-----------------|----------|
| Critical | 9.0-10.0 | Immediate patch | 24 hours |
| High | 7.0-8.9 | Sprint-level fix | 1 week |
| Moderate | 4.0-6.9 | Routine update | Next sprint |
| Low | 0.1-3.9 | Batch update | Monthly |

## Outdated Package Analysis

### Check Outdated
```bash
# npm
npm outdated --json

# pip
pip list --outdated --format=json

# Maven
mvn versions:display-dependency-updates

# Gradle
./gradlew dependencyUpdates
```

### Update Strategy
Parse semantic version changes:
```
patch: 1.2.3 → 1.2.4  (safe to batch)
minor: 1.2.3 → 1.3.0  (check changelog, group by ecosystem)
major: 1.2.3 → 2.0.0  (individual PR, read migration guide)
```

### Prioritization
```
Score = (days_since_release × 0.3) + (major_versions_behind × 30) + (security_issues × 100)
```
Sort by score descending — highest priority first.

## License Compliance

### Check Licenses
```bash
# npm (license-checker)
npx license-checker --json --out licenses.json

# Python (pip-licenses)
pip-licenses --format=json

# Java
mvn license:aggregate-third-party-report
```

### License Policy (from project.yaml)
```yaml
licensing:
  allowed:
    - MIT
    - Apache-2.0
    - BSD-2-Clause
    - BSD-3-Clause
    - ISC
  restricted:
    - GPL-2.0    # copyleft — requires legal review
    - GPL-3.0    # copyleft — requires legal review
    - AGPL-3.0   # strong copyleft — likely incompatible
  blockers:
    - UNLICENSED # no license — do not use
    - UNKNOWN    # unknown license — manual review required
```

Flag any dependency using a restricted or blocked license.

## Bundle Impact (Frontend Only)

```bash
# Analyze bundle size impact of a new package
npx bundlephobia-cli [package-name]@[version]

# Or: check bundlephobia.com programmatically
curl "https://bundlephobia.com/api/size?package=[name]@[version]"
```

Report:
```
Package: lodash@4.17.21
Size (minified+gzip): 24.5 kB
Side effects: YES (import all)
Tree-shakeable: NO

Recommendation: Use lodash-es for tree-shaking, or import specific functions:
  import debounce from 'lodash/debounce'  → 1.8 kB
```

## Full Analysis Report

```markdown
# Dependency Analysis Report
Date: [timestamp]
Project: [name from project.yaml]

## Security Summary
- Critical: 0 ✅
- High: 1 ⚠️ (express@4.18.2 — CVE-2024-XXXX — fix: upgrade to 4.19.0)
- Moderate: 3 (routine update cycle)
- Low: 7 (batch with next scheduled update)

## Outdated Packages
- 14 patch updates available (safe to batch)
- 6 minor updates (review changelogs)
- 2 major updates: react 18→19, typescript 5.1→5.4 (individual PRs needed)

## License Issues
- 0 blocked licenses
- 1 restricted (GPL): [package] — requires legal review

## Action Items
1. [URGENT] Patch CVE in express: npm install express@4.19.0
2. [ROUTINE] Batch patch updates in next sprint
3. [REVIEW] Check react 18→19 migration guide before upgrading
```
