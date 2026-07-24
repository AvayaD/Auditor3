# .NET 9.0 Upgrade Plan for Auditor3

Table of Contents

- Executive Summary
- Migration Strategy
- Detailed Dependency Analysis
- Project-by-Project Plans
  - Auditor3\Auditor3.csproj
  - Updater\Updater.csproj
- Package Update Reference
- Breaking Changes Catalog
- Testing & Validation Strategy
- Risk Management & Mitigation
- Source Control Strategy
- Success Criteria
- Appendix: Assessment Highlights

---

## Executive Summary

Selected Strategy

- All-At-Once Strategy — All projects upgraded simultaneously in a single coordinated operation.

Rationale

- Solution size: 2 projects (small) so atomic upgrade is feasible.
- Both projects are currently targeting .NET Framework 4.8; the main application (`Auditor3`) is a WPF app that requires `net9.0-windows` and Windows desktop support.
- Assessment indicates package upgrades are required and several binary/source incompatibilities exist (primarily in WPF/UI code). Upgrading all projects together avoids mixed framework states and simplifies dependency resolution.

Scope

- Projects in scope: `Auditor3\Auditor3.csproj`, `Updater\Updater.csproj`.
- Target frameworks:
  - `Auditor3` → `net9.0-windows` (enable Windows desktop/WPF support)
  - `Updater` → `net9.0`
- Package updates (applied as part of atomic upgrade): `Newtonsoft.Json` 6.0.4 → 13.0.4, `Microsoft.AspNet.WebApi.Client` 5.2.7 → 6.0.0, `SSH.NET` 2016.1.0 → 2025.1.0

Key Risks (summary)

- WPF binary incompatibilities: many API incompatibilities indicated by analysis — likely require code adjustments in XAML and event handlers.
- Project conversion: classic (non-SDK) project files must be converted to SDK-style projects.
- Package API changes: modern package versions may change APIs; code updates may be required.

Deliverables

- A completed `plan.md` (this file)
- Clear list of project changes, package versions, and validation checklist for execution stage


## Migration Strategy

Approach

- All projects will be upgraded simultaneously (atomic operation). This includes:
  - Converting classic project files to SDK-style where appropriate
  - Changing TargetFramework/TargetFrameworks properties to `net9.0` / `net9.0-windows`
  - Updating package references to the versions listed in §Package Update Reference
  - Restoring packages and building the solution to identify and fix compilation errors
  - Running test projects/assemblies and addressing test failures

Why All-At-Once

- Only two projects exist, which keeps the blast radius manageable.
- WPF and other platform-specific changes are cross-cutting; performing the upgrade in one pass reduces repeated context switching.

Constraints and Important Notes

- Do not attempt to run or execute any upgrade actions from this plan — this document is planning-only.
- Project conversion and package updates may require updating `Directory.Build.props`, `packages.config`, or migrating to `PackageReference` depending on repository state.
- For `Auditor3` (WPF), target `net9.0-windows` and set `<UseWindowsForms>false</UseWindowsForms>` unless WinForms used; enable `<UseWPF>true</UseWPF>` if required or set `<UseWindowsDesktop>true</UseWindowsDesktop>`.


## Detailed Dependency Analysis

Summary

- Total projects: 2
- No project-to-project project references detected; projects are independent.
- `Auditor3` is the main application (WPF) and largest codebase with most API incompatibilities.
- `Updater` is small and has minimal changes required.

Dependency Ordering (applies but atomic upgrade used)

- Although dependency order matters, because there are no project-to-project references the All-At-Once operation will treat both projects in the same atomic pass.

Critical paths

- `Auditor3` WPF code and XAML are the critical path for resolving API incompatibilities and rebuild success.


## Project-by-Project Plans

### Auditor3\Auditor3.csproj

Current State

- TargetFramework: `net48`
- Project style: Classic (non-SDK)
- Project type: WPF (XAML, PresentationFramework, PresentationCore references present)
- LOC: ~9,958 lines
- Assessment: High number of binary incompatibilities (WPF APIs), needs SDK-style conversion, packages updated

Target State

- TargetFramework: `net9.0-windows`
- SDK-style project file
- Packages updated to supported versions for `net9.0`
- Project builds cleanly (0 errors) and tests (if any) pass

Planned Changes

1. Prerequisites
   - Ensure .NET 9.0 SDK is installed on the build machine.
   - Ensure branch `upgrade-to-NET9` is active (already created).
2. Project file conversion
   - Convert `Auditor3.csproj` to SDK-style project. Keep WPF settings: set `<TargetFramework>net9.0-windows</TargetFramework>` and `<UseWPF>true</UseWPF>` (or `<UseWindowsDesktop>true</UseWindowsDesktop>` plus `<UseWPF>true</UseWPF>`).
   - Remove old `packages.config` usage or migrate to `PackageReference` (recommended) for modern package management. If migrating, update project references accordingly.
3. Package updates
   - Update package references to versions in §Package Update Reference.
   - Replace assembly HintPath references (remove direct DLL references where possible) to rely on NuGet-managed packages.
4. Code adjustments
   - Address WPF API incompatibilities reported by assessment (XAML changes, event handler signatures, MessageBox usages, Dispatcher calls, etc.).
   - Replace obsolete cryptography APIs if present (see Breaking Changes Catalog).
   - Update any System.Configuration usage to `System.Configuration.ConfigurationManager` package if needed.
5. Build & fix
   - Restore packages and build; resolve compilation errors.
6. Validation
   - Run smoke validations (application starts, key windows load) in execution stage using automation if available.

Validation Checklist (for executor)

- [ ] Project file converted to SDK-style and targets `net9.0-windows`
- [ ] All package references updated and restored successfully
- [ ] Solution builds with 0 errors
- [ ] No remaining package vulnerabilities flagged by dependency scanner


### Updater\Updater.csproj

Current State

- TargetFramework: `net48`
- Project style: Classic (non-SDK)
- Small utility project with minimal code changes expected

Target State

- TargetFramework: `net9.0`
- SDK-style project file

Planned Changes

1. Convert project file to SDK-style and set `<TargetFramework>net9.0</TargetFramework>`.
2. Migrate package references as needed; this project has no third-party packages flagged as vulnerable in assessment.
3. Restore and build; fix any minor source compatibility issues.

Validation Checklist

- [ ] SDK-style project created and targets `net9.0`
- [ ] Builds with 0 errors


## Package Update Reference

This section consolidates package updates to be applied during the atomic upgrade.

Common package updates (affecting `Auditor3`)

- `Newtonsoft.Json` — current: `6.0.4` → target: `13.0.4` — Reason: security and modern API compatibility
- `Microsoft.AspNet.WebApi.Client` — current: `5.2.7` → target: `6.0.0` — Reason: compatibility with modern HTTP client stacks
- `SSH.NET` — current: `2016.1.0` → target: `2025.1.0` — Reason: security updates and support for modern frameworks

Notes

- If `packages.config` is used, convert to `PackageReference` where possible. This simplifies transitive dependency management and modernizes package handling.
- Use exact versions above in the plan to ensure reproducible builds.


## Breaking Changes Catalog

This catalog highlights the most likely breaking areas identified during assessment. It is not exhaustive — compilation will reveal concrete errors.

WPF and UI

- Routed event handler signatures and XAML elements may require small adjustments. Typical fixes:
  - Ensure event handler method signatures match expected delegates.
  - Update usages of `MessageBox.Show` if overload resolution changes.
  - Review `Dispatcher.Invoke` and asynchronous UI operations.
  - Inspect any XAML that uses legacy type converters or markup that may have been tightened.

Assembly & API changes

- Remove direct assembly `HintPath` references to framework assemblies; rely on TargetFramework to provide system assemblies.
- Replace `System.Configuration` direct usages with `System.Configuration.ConfigurationManager` NuGet package if retaining config-based approach.

Packages

- `Newtonsoft.Json` 13.x: some API surfaces changed (e.g., default settings); verify serialization settings usage.
- `Microsoft.AspNet.WebApi.Client` 6.0.0: check any typed MediaTypeFormatter or HttpClient extension usages.
- `SSH.NET` 2025.x: review connection and SFTP APIs for signature changes.

Cryptography

- Replace uses of `RijndaelManaged` with `Aes` family or modern APIs; RijndaelManaged may be flagged as source-incompatible.


## Testing & Validation Strategy

Levels of testing (for executor stage)

1. Local build verification
   - Restore packages and build solution; fix compilation errors until build succeeds.
2. Automated unit tests (if present)
   - Run unit tests and fix failures.
3. Smoke tests (manual or automated)
   - Launch application main windows; exercise key flows.
4. Integration tests
   - Validate external connections (SSH, HTTP client flows) if automation available.

Checklist

- [ ] Solution builds with 0 errors
- [ ] Unit tests pass
- [ ] Key application windows open without exceptions
- [ ] No security vulnerabilities remain in NuGet dependencies


## Risk Management & Mitigation

Risk: WPF API incompatibilities

- Level: High for `Auditor3`.
- Mitigation:
  - Target `net9.0-windows` and enable WPF support in the SDK-style project file.
  - Resolve XAML and event handler issues during atomic upgrade pass.
  - Keep branches small and make focused commits during execution.

Risk: Project file conversion errors

- Level: Medium.
- Mitigation:
  - Convert projects to SDK-style incrementally in local branch; keep backup of original `.csproj` file before finalizing.
  - If conversion introduces unexpected issues, revert and iterate on conversion steps.

Risk: Package API changes

- Level: Medium.
- Mitigation:
  - Consult package release notes for major-version upgrades (e.g., `Newtonsoft.Json` 6 → 13) and adapt code accordingly.

Contingency

- If blocking binary incompatibilities cannot be resolved quickly, consider pausing and investigating targeted refactor or migrating a smaller set of classes first (requires switching to incremental strategy). This is an execution-time decision and not part of planning.


## Source Control Strategy

Branching

- Work is performed on branch: `upgrade-to-NET9` (already created).
- Use a single atomic-change PR that contains all project file conversions and package updates. This aligns with All-At-Once approach and keeps the upgrade atomic.

Commit strategy

- Commit changes in logical chunks but produce a single squashed commit for the main upgrade PR if your workflow prefers a single commit for easier rollback.
- Include a clear commit message: `Upgrade solution to .NET 9.0 (net9.0-windows for Auditor3); update packages: Newtonsoft.Json 13.0.4, Microsoft.AspNet.WebApi.Client 6.0.0, SSH.NET 2025.1.0`

Review & Merge

- Require code review from developer(s) familiar with WPF and package changes.
- Merge only after the build is green and automated tests (if any) pass.


## Success Criteria

The upgrade is considered complete when all of the following are true:

- All projects target their proposed frameworks (`net9.0` / `net9.0-windows`).
- All package updates listed in §Package Update Reference are applied.
- Solution builds with 0 compilation errors.
- Unit tests (if present) pass.
- No critical security vulnerabilities remain in NuGet dependencies.


## Appendix: Assessment Highlights

- Total projects: 2
- Heavy WPF-related compatibility work expected in `Auditor3` (majority of API issues).
- Packages to update: `Newtonsoft.Json` → 13.0.4, `Microsoft.AspNet.WebApi.Client` → 6.0.0, `SSH.NET` → 2025.1.0

-- End of plan.md
