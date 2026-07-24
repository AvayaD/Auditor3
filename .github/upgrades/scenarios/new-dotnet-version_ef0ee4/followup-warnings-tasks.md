# Follow-up Tasks: Warnings Triage (Post-upgrade)

This follow-up task list tracks remediation of warnings identified during the .NET 9 upgrade build and smoke runs. Address these before or shortly after merge to reduce technical debt and security risk.

## Overview
- Branch: `master` (upgrade merged)
- Source commit: `066c64d`
- Priority: High → medium

## Tasks

### [ ] W-001: Replace `RijndaelManaged` usages with `Aes`
- Scope: `Auditor3/Encrypt.cs` and any other files referencing `RijndaelManaged`.
- Actions:
  - Locate all `RijndaelManaged` usages.
  - Replace with `Aes`/`AesManaged` or `Aes.Create()` and adapt key/IV handling if required.
  - Add unit tests for encryption/decryption if not present (basic round-trip).
- Verify:
  - Build with 0 errors; SYSLIB0022 warnings removed.
  - Manual validation of features relying on encryption.
- Estimate: 1-2h

### [ ] W-002: Add platform annotations or runtime guards for Windows-only APIs (CA1416)
- Scope: `Auditor3/App.xaml.cs` (Registry calls) and any other Registry/Win32 calls.
- Actions:
  - Add `[SupportedOSPlatform("windows")]` to methods/classes using Windows-only APIs or wrap calls in `OperatingSystem.IsWindows()` checks.
  - Suppress only where acceptable with justification comments.
- Verify:
  - Build with CA1416 warnings resolved or explicitly documented/approved.
- Estimate: 0.5-1h

### [ ] W-003: Scan and resolve other compiler warnings
- Scope: All projects
- Actions:
  - Run `dotnet build -c Release` and collect warnings.
  - Triage warnings: fix actionable ones (nullability, unused fields, etc.), annotate or document low-risk ones.
- Verify:
  - Build warnings reduced to acceptable baseline per team policy.
- Estimate: 1-4h (depends on count)

### [ ] W-004: Re-run vulnerability scan and address findings
- Scope: Solution packages
- Actions:
  - Run `dotnet list Auditor3.sln package --vulnerable` and/or run your security scanner.
  - Update or mitigate any critical/important package vulnerabilities.
- Verify:
  - Vulnerability report shows no critical/important issues (or documented mitigations).
- Estimate: 1-3h

### [ ] W-005: Add CI gate for warnings policy (optional)
- Scope: `.github/workflows/dotnet9-ci.yml`
- Actions:
  - Add step to fail build on specified warning codes or treat warnings as errors for Release build.
  - Alternatively add report and require manual approval for known warnings.
- Verify:
  - CI blocks merge if policy violated.
- Estimate: 0.5-1h

## How to execute
- Branch from `master`: `git checkout -b fix/warnings-rijndael` (adjust per task)
- Make changes, run `dotnet build Auditor3.sln -c Release` and tests/smoke runs.
- Commit with `W-00X: <short description>` and open PR referencing this follow-up list.

## Owner & Priority
- Suggested owner: team member with security/WPF knowledge
- Priority order: W-001 → W-002 → W-004 → W-003 → W-005

## Links
- CI workflow: `.github/workflows/dotnet9-ci.yml`
- PR checklist: `PR_CHECKLIST.md`
- Changelog: `CHANGELOG.md`


---
Generated: 2026-07-24
