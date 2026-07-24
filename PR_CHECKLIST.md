PR Review Checklist — Upgrade to .NET 9.0

Before approving/merging, verify the following:

- [ ] CI status: GitHub Actions (.github/workflows/dotnet9-ci.yml) is green on this PR (windows-latest, .NET 9).
- [ ] Build (Release): `dotnet build Auditor3.sln -c Release` completes with 0 errors.
- [ ] Vulnerability scan: `dotnet list Auditor3.sln package --vulnerable` shows no critical vulnerabilities.
- [ ] Updater smoke run: `dotnet run --project Updater/Updater.csproj -c Release` runs and prints expected output.
- [ ] Auditor3 GUI: Launch locally on Windows with .NET 9 Desktop runtime and validate key windows open and core flows (PREC import, collectors, dialogs).
- [ ] Warnings triage: Review and either fix or document acceptance for:
  - `RijndaelManaged` obsoletion (replace with `Aes`)
  - CA1416 platform annotations / Registry usage
- [ ] Package API changes: spot-check serialization/HTTP/SSH flows after package upgrades.
- [ ] No accidental check-ins of binaries, obj/, bin/ or packages/ folders.
- [ ] Changelog updated (see `CHANGELOG.md`).
- [ ] At least one reviewer with WPF/domain knowledge approves.

Merge criteria: All required checks passing and reviewer approval.

Commands for reviewers:
- Build & restore:
  - `dotnet restore Auditor3.sln`
  - `dotnet build Auditor3.sln -c Release`
- Run vuln scan:
  - `dotnet list Auditor3.sln package --vulnerable`
- Updater smoke run:
  - `dotnet run --project Updater/Updater.csproj -c Release`
- Launch Auditor3 (local Windows):
  - Open `Auditor3\bin\Release\net9.0-windows\CorruptionAuditor.exe` or run from VS/`dotnet run` in project (requires desktop runtime).

