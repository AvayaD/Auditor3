# Changelog

## Unreleased

### Upgrade to .NET 9.0 (branch: `upgrade-to-NET9`, commit: `9c0f489`) - 2026-07-24

- Converted projects to SDK-style:
  - `Auditor3` → `net9.0-windows` (WPF enabled)
  - `Updater` → `net9.0`
- Updated NuGet packages:
  - `Newtonsoft.Json` → `13.0.4`
  - `Microsoft.AspNet.WebApi.Client` → `6.0.0`
  - `SSH.NET` → `2025.1.0`
- Added CI workflow: `.github/workflows/dotnet9-ci.yml` (build + smoke run on Windows)
- Notes:
  - Local Release build: succeeded (0 errors, 12 warnings)
  - Manual GUI smoke validation for `Auditor3` completed
  - Remaining warnings: obsolete `RijndaelManaged` (recommend replace with `Aes`), CA1416 registry platform annotations

---
