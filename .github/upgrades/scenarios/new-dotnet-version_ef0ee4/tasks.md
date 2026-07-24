# Auditor3 .NET 9.0 Upgrade Tasks

## Overview

Upgrade both projects in the solution (`Auditor3`, `Updater`) simultaneously to .NET 9.0 in a single atomic operation: convert project files to SDK-style, update package references, restore and build, then run automated tests if present. Tasks follow Plan §Migration Strategy and the All-At-Once approach described in the plan.

**Progress**: 3/4 tasks complete (75%) ![75%](https://progress-bar.xyz/75)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-07-24 12:31)*
**References**: Plan §Project-by-Project Plans (Auditor3 ¶Planned Changes §1), Plan §Migration Strategy

- [✓] (1) Verify the required .NET 9.0 SDK is installed on the build machine per Plan §Project-by-Project Plans (Auditor3 ¶Prerequisites)
- [✓] (2) Runtime/SDK version meets minimum requirements for building `net9.0` / `net9.0-windows` projects (**Verify**)
- [✓] (3) Check configuration files referenced by the plan (e.g., `global.json`, `Directory.Build.props`, `Directory.Packages.props`) for compatibility with target frameworks per Plan §Migration Strategy
- [✓] (4) Configuration/build files are compatible with target version (**Verify**)

---

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-07-24 12:33)*
**References**: Plan §Migration Strategy, Plan §Project-by-Project Plans (Auditor3, Updater), Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Convert projects to SDK-style and update target frameworks per Plan §Project-by-Project Plans:
  - `Auditor3` → `net9.0-windows` with WPF enabled (`<UseWPF>true</UseWPF>` / `<UseWindowsDesktop>true</UseWindowsDesktop>`)  
  - `Updater` → `net9.0`
- [✓] (2) All project files updated to target frameworks (**Verify**)
- [✓] (3) Migrate `packages.config` to `PackageReference` where applicable and update package references to versions listed in Plan §Package Update Reference (e.g., `Newtonsoft.Json` → `13.0.4`, `Microsoft.AspNet.WebApi.Client` → `6.0.0`, `SSH.NET` → `2025.1.0`)
- [✓] (4) All package references updated and packages restored successfully (**Verify**)
- [✓] (5) Restore dependencies for the solution (dotnet restore / nuget restore as appropriate)
- [✓] (6) Build the solution and fix all compilation errors caused by framework and package upgrades per Plan §Breaking Changes Catalog (include XAML/WPF fixes, API changes, assembly HintPath removal)
- [✓] (7) Solution builds with 0 errors (**Verify**)

---

### [✓] TASK-003: Run test suite and validate upgrade *(Completed: 2026-07-24 07:48)*
**References**: Plan §Testing & Validation Strategy, Plan §Breaking Changes Catalog

- [✓] (1) Run all automated test projects (unit/integration) present in the repository per Plan §Testing & Validation Strategy
- [✓] (2) Fix any test failures (reference Plan §Breaking Changes Catalog for common issues)
- [✓] (3) Re-run tests after fixes
- [✓] (4) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all remaining changes with message: "TASK-004: Upgrade solution to .NET 9.0 (net9.0-windows for `Auditor3`); update packages per Plan §Package Update Reference"


