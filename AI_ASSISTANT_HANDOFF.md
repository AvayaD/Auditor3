# Auditor3 AI Assistant - Detailed Handoff

## 1. Purpose

This document allows work on the Auditor3 AI engineering assistant to continue if the current chat session is lost.

Read this file together with:

- `AI_AGENT_PLAN.md`
- The current Auditor3 source code
- The current Git history and branch status

This document records:

- What has already been completed.
- The current repository state.
- The files that contain the completed foundation.
- Known limitations.
- The next recommended implementation milestone.
- Testing and Git procedures.
- Files that should be provided to a future AI session.

---

## 2. Project Overview

Auditor3 is a .NET 9 WPF desktop application used to collect, parse, audit, and repair Avaya Communication Manager translation records.

The application currently supports:

- PREC collection from CM systems.
- PREC parsing.
- Station audits.
- Trunk audits.
- Announcement audits.
- EECCR investigation.
- Repair script generation.
- Lab staging.
- CM, ToolsA, and DRCCD connections.
- AREC, DREC, and PREC investigation scripts.

The planned AI assistant will help engineers understand:

- PREC structures.
- Raw PREC dumps.
- Compiled C structure layouts.
- Field offsets and sizes.
- Alignment holes and compiler padding.
- Encoded extensions and UIDs.
- AREC/DREC/PREC relationships.
- Related PREC records.
- Audit failures.
- Existing deterministic repair recommendations.

The assistant is advisory only.

The assistant must not become authoritative for:

- Field offsets.
- Structure sizes.
- Record decoding.
- Audit results.
- Repair generation.
- SAT or TCM execution.
- CM system modification.

The authoritative sources are:

1. Auditor3's deterministic code.
2. CM header files.
3. Cscope results.
4. GDB `ptype /o` output.
5. Release-specific `.ptype` layout files.
6. Auditor3 audit strategies.
7. Auditor3 `Fixer` output.
8. Engineer review.

---

## 3. Repository Information

Repository directory:

```text
C:\Users\mcnuttd\projects\Auditor3_Modernized
```

Remote repository:

```text
https://github.com/AvayaD/Auditor3.git
```

Solution:

```text
Auditor3.sln
```

Base branch:

```text
master
```

Approved internal AI service:

```text
webui.avaya.com
```

Important service rule:

The official API, SDK, authentication method, request schema, and response schema for `webui.avaya.com` must be obtained before implementing network integration. Do not guess the API or automate the browser unless that is officially approved.

---

## 4. Repository State at Handoff

The AI foundation and PREC investigation scripts have been merged into `master`.

The expected current branch is:

```text
master
```

The expected working tree is clean:

```powershell
git status --short
```

A clean working tree produces no output.

Verify the state with:

```powershell
git branch --show-current
git status --short
git log -5 --oneline
```

The latest known merged commits are:

```text
dfd2521
```

for the AI foundation, and:

```text
e12aeda
```

for the PREC investigation scripts and temporary S34/S35 change.

The exact commit currently checked out should always be verified locally rather than assumed.

---

## 5. Validation Already Completed

The solution has successfully built with:

```powershell
dotnet build .\Auditor3.sln -c Debug
```

The solution tests have successfully run with:

```powershell
dotnet test .\Auditor3.sln -c Debug
```

The latest known test result was:

```text
Test summary: total: 5, failed: 0, succeeded: 5, skipped: 0
```

Before beginning new development, run:

```powershell
dotnet build .\Auditor3.sln -c Debug
dotnet test .\Auditor3.sln -c Debug
```

If either command fails, investigate that failure before adding new functionality.

---

## 6. Completed Work

The following work is already merged:

- Added `AI_AGENT_PLAN.md`.
- Added compiled PREC layout models.
- Added the GDB `ptype /o` layout parser.
- Added assistant context and evidence models.
- Added assistant request and response models.
- Added a disabled assistant service.
- Added assistant configuration settings.
- Added assistant redaction interface and implementation.
- Added the `Auditor3.Tests` xUnit project.
- Added parser tests.
- Added disabled-service tests.
- Added redaction tests.
- Removed the obsolete Updater smoke-test step from CI.
- Added `arecfind.sh`.
- Added `findprecs.sh`.
- Added `precstruct.sh`.
- Temporarily disabled `AUDIT-S34`.
- Temporarily disabled `AUDIT-S35`.

No web service integration has been implemented.

No WPF assistant interface has been implemented.

No AI network calls are currently made by the application.

No SAT or TCM execution has been added to the assistant.

---

## 7. Safety Rules

The AI assistant must not:

- Execute SAT commands.
- Execute TCM commands.
- Execute shell commands.
- Execute repair scripts.
- Modify CM translations.
- Open or control live CM sessions.
- Change `Fixer` output automatically.
- Invent structure offsets.
- Invent structure sizes.
- Claim that it executed a command.
- Receive passwords or authentication tokens.
- Log credentials or ASG responses.

The assistant may:

- Explain supplied facts.
- Compare supplied records.
- Explain field layouts.
- Identify supplied record relationships.
- Explain deterministic audit failures.
- Explain deterministic repair proposals.
- Suggest read-only verification steps.

The intended workflow is:

```text
Auditor3 determines authoritative facts.
The assistant explains those facts.
The engineer reviews the explanation.
Auditor3 remains responsible for validation and repairs.
```

---

## 8. Git Workflow

For a new milestone, use:

```powershell
git switch master
git pull --ff-only
git switch -c feature/<descriptive-name>
```

Before making changes:

```powershell
git status --short
```

After making changes:

```powershell
git diff
git status --short
```

Build and test:

```powershell
dotnet build .\Auditor3.sln -c Debug
dotnet test .\Auditor3.sln -c Debug
```

Stage only reviewed files:

```powershell
git add -- <specific-files>
```

Do not use:

```powershell
git add .
```

unless every changed file has been reviewed and is intentionally part of the commit.

Commit:

```powershell
git commit -m "<clear description>"
```

Push:

```powershell
git push -u origin feature/<descriptive-name>
```

Pull requests must target:

```text
master
```

---

## 9. Completed AI Assistant Files

The following files were added under:

```text
Auditor3/Assistant/
```

### 9.1 AssistantContext.cs

Purpose:

- Holds structured information supplied to the AI assistant.
- Does not call a network service.
- Does not parse CM records.
- Is currently a data model only.

Current properties:

```text
ApplicationVersion
CmRelease
PrecType
StructureName
HeaderFile
StructureSourceLine
CompiledSize
DumpSize
Fields
MappingDetails
ProposedFixes
Evidence
```

Important limitation:

`AssistantContext` does not currently contain a dedicated `RawPrec` property.

If raw PREC text is needed, decide whether to:

1. Add a `RawPrec` property.
2. Store raw PREC text as an `AssistantEvidence` item.
3. Omit raw text and send only structured fields.

This decision must follow the approved data-classification policy.

---

### 9.2 AssistantEvidence.cs

Purpose:

Represents evidence supplied to the assistant.

Current properties:

```text
Type
Source
Description
Content
```

Recommended evidence types:

```text
RawPrec
CompiledLayout
CStructure
DecodedField
AuditResult
RelatedRecord
DrecMapping
ProposedFix
ApplicationMetadata
```

The source should identify where the evidence came from, for example:

```text
cm10.2/pr_ext.ptype
Auditor3/Modules/Auditor.cs
Database.PR_EXTs
AuditS05Strategy
```

---

### 9.3 AssistantRequest.cs

Purpose:

Represents a question submitted to an assistant implementation.

Current properties:

```text
Question
SystemInstructions
Context
CorrelationId
```

The request should not contain:

- Passwords.
- ASG challenges.
- ASG responses.
- Authorization headers.
- Cookies.
- SSH credentials.
- Unredacted connection errors.

---

### 9.4 AssistantResponse.cs

Purpose:

Represents a result returned by an assistant implementation.

Current properties:

```text
Succeeded
Answer
ErrorMessage
CorrelationId
ContainsSuggestedCommands
Warnings
```

Important rule:

An assistant response is advisory text. It must never be treated as:

- An audit result.
- A CM state result.
- A verified repair.
- Evidence that a command was executed.

---

### 9.5 AssistantSettings.cs

Purpose:

Stores configuration for the optional assistant feature.

Current configuration concepts:

```text
Enabled
ServiceEndpoint
TimeoutSeconds
MaximumContextBytes
SendRawPrecData
SendRelatedRecords
SendCompiledLayout
SendSourceDeclaration
EnableRequestLogging
AllowCommandSuggestions
```

Recommended defaults:

```text
Enabled = false
SendRawPrecData = false
SendRelatedRecords = true
SendCompiledLayout = true
SendSourceDeclaration = false
EnableRequestLogging = false
AllowCommandSuggestions = false
TimeoutSeconds = 60
MaximumContextBytes = 250000
```

Important rule:

Do not place secrets in `AssistantSettings`.

Never store these values in source code or plain-text configuration:

```text
Passwords
Tokens
Cookies
Private keys
Client secrets
Authorization headers
```

---

### 9.6 IAssistantService.cs

Purpose:

Defines the service boundary for all assistant implementations.

Conceptually:

```csharp
Task<AssistantResponse> AskAsync(
    AssistantRequest request,
    CancellationToken cancellationToken = default);
```

Expected implementations:

```text
DisabledAssistantService
LocalAssistantService
MockAssistantService
WebUiAssistantService
```

Only `WebUiAssistantService` should make network calls.

The existing audit and CM connection classes should not call the AI service directly.

---

### 9.7 DisabledAssistantService.cs

Purpose:

Provides a safe implementation when AI assistance is disabled.

Behavior:

- Makes no network calls.
- Does not access CM.
- Returns a failed response with a clear disabled message.
- Preserves the request correlation ID.

This is currently the only assistant service implementation.

---

### 9.8 IAssistantRedactor.cs

Purpose:

Defines the redaction boundary before context can be transmitted.

Conceptually:

```csharp
AssistantContext Redact(AssistantContext context);
```

The redactor must be applied before any future network adapter.

---

### 9.9 AssistantRedactor.cs

Purpose:

- Copies the supplied assistant context.
- Processes evidence content.
- Replaces lines containing likely secret names with `[REDACTED]`.

Current sensitive-name detection includes:

```text
password
passwd
challenge:
response:
authorization:
bearer
cookie:
private key
private-key
secret
token:
```

Important limitations:

1. Redaction currently focuses on evidence content.
2. It does not fully inspect all structured fields.
3. It does not pseudonymize UIDs.
4. It does not pseudonymize extensions.
5. It does not classify customer data.
6. It does not replace a formal security review.
7. It should not be considered production-ready for service transmission without enhancement.

Future improvements:

- Add structured-property redaction.
- Add configurable policies.
- Add tests for sensitive JSON.
- Add tests for secrets embedded in exception text.
- Add tests for case variations.
- Add pseudonymization options.
- Add an explicit allowlist and denylist.
- Add a user-visible context preview.
- Add tests ensuring redacted content cannot be recovered.

---

### 9.10 PrecLayoutParser.cs

Purpose:

Parses GDB output generated by:

```text
ptype /o struct <name>
```

The parser currently identifies:

- Structure name.
- Field offsets.
- Field sizes.
- Field declarations.
- Padding/hole entries.
- Bit-field offsets.
- Bit-field sizes.
- Total compiled structure size.

Example input:

```text
(gdb) ptype /o struct pr_ext
/* offset    |  size */  type = struct pr_ext {
/*    0      |     2 */    short no_digits;
/*    2      |     8 */    NYBLE ext[8];
/* XXX  2-byte hole */
/*   12      |     4 */    UID p_uid;
/* total size (bytes):   32 */
}
```

The parser returns a `PrecLayout`.

The parser is a prototype and requires additional testing against the complete layouts for:

```text
pr_ext
pr_mobd
pr_stn
```

---

## 10. Completed PREC Layout Models

The following files were added under:

```text
Auditor3/Models/Assistant/
```

```text
PrecLayout.cs
PrecLayoutField.cs
PrecFieldValue.cs
```

### 10.1 PrecLayout

Represents one compiled layout.

Conceptual properties:

```text
PrecType
StructureName
Release
SourceFile
TotalSize
Fields
```

`TotalSize` is nullable because a layout file may fail to contain a recognizable GDB total-size line.

`Fields` contains both actual fields and padding entries.

---

### 10.2 PrecLayoutField

Represents one field, bit field, or padding region.

Conceptual properties:

```text
Name
Type
Offset
Size
IsPadding
IsBitField
BitOffset
BitSize
SourceText
```

For a normal field:

```text
IsPadding = false
IsBitField = false
BitOffset = null
BitSize = null
```

For a padding region:

```text
Name = "padding"
Type = "padding"
IsPadding = true
```

For a bit field:

```text
IsBitField = true
BitOffset = <bit position>
BitSize = <number of bits>
```

The full original GDB line should be preserved in `SourceText`.

---

### 10.3 PrecFieldValue

Represents a value mapped to a layout field.

Conceptual properties:

```text
Name
Type
Offset
Size
RawValue
DecodedValue
DecodeStatus
IsPadding
```

`RawValue` and `DecodedValue` must remain separate.

Example:

```text
RawValue:     8aa1
DecodedValue: 1008
DecodeStatus: Decoded
```

Possible decode statuses:

```text
Decoded
RawOnly
Unknown
NotApplicable
```

Do not place a guessed value into `DecodedValue`.

---

## 11. Existing Test Project

Project:

```text
Auditor3.Tests/Auditor3.Tests.csproj
```

It is included in:

```text
Auditor3.sln
```

Target framework:

```text
net9.0-windows
```

It references:

```text
Auditor3/Auditor3.csproj
```

Current test file:

```text
Auditor3.Tests/UnitTest1.cs
```

The file should eventually be renamed to:

```text
PrecLayoutParserTests.cs
```

Additional recommended test files:

```text
AssistantRedactorTests.cs
DisabledAssistantServiceTests.cs
PrecAnalysisContextBuilderTests.cs
```

Current test coverage includes:

1. PR_EXT layout parsing.
2. Padding parsing.
3. Bit-field metadata parsing.
4. Disabled assistant behavior.
5. Redaction of sensitive evidence lines.
6. Preservation of structured assistant context.

Known current result:

```text
5 tests passed
0 tests failed
```

Run all tests with:

```powershell
dotnet test .\Auditor3.sln -c Debug
```

---

## 12. Existing Domain and Audit Files

The assistant context builder will eventually need to work with facts produced by these components:

```text
Auditor3/Modules/PRECParser.cs
Auditor3/Database.cs
Auditor3/Modules/Auditor.cs
Auditor3/Modules/Fixer.cs
Auditor3/Models/AuditResult.cs
Auditor3/Models/AuditStatistics.cs
Auditor3/Services/AuditEngine.cs
Auditor3/Services/AuditStrategies.cs
Auditor3/Services/Strategies/AllStationStrategies.cs
Auditor3/Services/Strategies/AllAnnouncementStrategies.cs
Auditor3/Services/Strategies/AllTrunkStrategies.cs
```

The following classes are especially relevant:

```text
Auditor3/PRECs/PR_EXT.cs
Auditor3/PRECs/PR_MOBD.cs
Auditor3/PRECs/PR_STN.cs
Auditor3/PRECs/PR_MOPORT.cs
Auditor3/PRECs/PR_PORT_UID.cs
Auditor3/PRECs/PR_ST_CPS.cs
Auditor3/PRECs/PR_UDATA.cs
Auditor3/PRECs/PR_FEXT.cs
```

Review the actual source files before designing a generic decoder. Existing PREC classes may already have domain-specific decoding logic that must be reused rather than duplicated.

---

## 13. Investigation Scripts

The following scripts are merged into:

```text
Auditor3/Scripts/
```

### 13.1 arecfind.sh

Purpose:

- Investigate AREC-related mappings.
- Search the CM source tree.
- Support investigation of action/object relationships.
- Intended for execution in the DRCCD environment.

### 13.2 findprecs.sh

Purpose:

- Resolve an action.
- Resolve an object.
- Resolve an optional qualifier.
- Locate AREC data.
- Locate DREC data.
- Locate mapped PREC types.

Example previously resolved:

```text
display cluster master-cm
```

Result:

```text
AREC: UI_ARRAYINFO
DREC: DM_ARRAYINFO
PREC: PR_ARRAYINFO
```

### 13.3 precstruct.sh

Purpose:

- Locate the matching C structure with cscope.
- Identify the source header.
- Identify the structure line.
- Derive the active CM release from `strings.ct`.
- Locate a release-specific `.ptype` file.
- Display the source structure.
- Display the compiled GDB memory layout.

Example DRCCD layout directory:

```text
/home/mcnuttd/precstruct_layouts/cm10.2/
```

Example files:

```text
/home/mcnuttd/precstruct_layouts/cm10.2/pr_ext.ptype
/home/mcnuttd/precstruct_layouts/cm10.2/pr_mobd.ptype
/home/mcnuttd/precstruct_layouts/cm10.2/pr_stn.ptype
```

The scripts are not compiled into Auditor3.

They are investigation tools intended for DRCCD/CM environments.

### 13.4 Script limitations

Do not assume these scripts run unchanged under Windows PowerShell.

The DRCCD environment has:

- Older `grep`.
- Older `awk`.
- Different shell behavior.
- Cscope databases tied to the CM source tree.
- DRCCD-specific absolute paths.
- Different line-ending behavior.
- Potential LF/CRLF conversion when edited on Windows.

The scripts should be tested on DRCCD, not treated as Windows application code.

---

## 14. Temporary Audit Change

The following calls are currently commented out in:

```text
Auditor3/Modules/Auditor.cs
```

```text
AUDIT-S34
AUDIT-S35
```

These audits are intentionally disabled temporarily.

### AUDIT-S34

Purpose:

```text
Detect orphaned PR_FEXT records.
```

### AUDIT-S35

Purpose:

```text
Detect duplicate PR_FEXT records.
```

Do not restore these automatically.

A future cleanup task should determine whether:

- The audits should be restored.
- The audits should be replaced by investigation scripts.
- The audits should be permanently removed.
- New behavior should be added.
- Regression tests are required.

---

## 15. PREC Layout Data Collected from CM 10.2

The following layout information was obtained from GDB on the DRCCD CM 10.2 environment.

The layout files were captured with commands equivalent to:

```text
set pagination off
ptype /o struct pr_ext
ptype /o struct pr_mobd
ptype /o struct pr_stn
```

The output was saved as:

```text
pr_ext.ptype
pr_mobd.ptype
pr_stn.ptype
```

These layout files are release-specific. They must not be assumed to apply to every CM release or patch level.

---

### 15.1 PR_EXT Layout

C structure:

```text
struct pr_ext
```

Compiled size:

```text
32 bytes
```

Top-level fields:

| Offset | Size | Field |
|---:|---:|---|
| 0 | 2 | `no_digits` |
| 2 | 8 | `ext[8]` |
| 12 | 4 | `p_uid` |
| 16 | 2 | `ctbl_idx` |
| 18 | 2 | `ctbl2_idx` |
| 20 | 2 | `lainfo_idx` |
| 22 | 1 | `cidx_type` |
| 23 | 1 | `cidx2_type` |
| 24 | 1 | `ctbl_opt` |
| 25 | 1 | `is_xdid` |
| 26 | 1 | `free_xdid` |
| 27 | 1 | `utype` |
| 28 | 1 | `list_cmdext` |

Compiler layout gaps:

| Offset | Size | Description |
|---:|---:|---|
| 10 | 2 | Alignment hole before `p_uid` |
| 29 | 3 | Trailing structure padding |

The total layout is:

```text
offset 0 through offset 31
total size: 32 bytes
```

The original GDB output was conceptually:

```text
/* offset    |  size */  type = struct pr_ext {
/*    0      |     2 */    short no_digits;
/*    2      |     8 */    NYBLE ext[8];
/* XXX  2-byte hole */
/*   12      |     4 */    UID p_uid;
/*   16      |     2 */    unsigned short ctbl_idx;
/*   18      |     2 */    unsigned short ctbl2_idx;
/*   20      |     2 */    unsigned short lainfo_idx;
/*   22      |     1 */    unsigned char cidx_type;
/*   23      |     1 */    unsigned char cidx2_type;
/*   24      |     1 */    unsigned char ctbl_opt;
/*   25      |     1 */    unsigned char is_xdid;
/*   26      |     1 */    unsigned char free_xdid;
/*   27      |     1 */    unsigned char utype;
/*   28      |     1 */    unsigned char list_cmdext;
/* XXX  3-byte padding */
/* total size (bytes):   32 */
}
```

---

### 15.2 PR_MOBD Layout

C structure:

```text
struct pr_mobd
```

Compiled size:

```text
12 bytes
```

Fields:

| Offset | Size | Field |
|---:|---:|---|
| 0 | 4 | `pn1` |
| 4 | 4 | `board_id` |
| 8 | 2 | `MO_lname` |
| 10 | 1 | `suffix` |
| 11 | 1 | `bd_ttistate` |

There is no padding between the displayed top-level fields.

The original GDB output was conceptually:

```text
/* offset    |  size */  type = struct pr_mobd {
/*    0      |     4 */    PN1 pn1;
/*    4      |     4 */    PN_PORT_ID board_id;
/*    8      |     2 */    LNAME MO_lname;
/*   10      |     1 */    char suffix;
/*   11      |     1 */    char bd_ttistate;
/* total size (bytes):   12 */
}
```

---

### 15.3 PR_STN Layout

C structure:

```text
struct pr_stn
```

Compiled size:

```text
412 bytes
```

Important top-level fields:

| Offset | Size | Field |
|---:|---:|---|
| 0 | 4 | `uid` |
| 4 | 2 | `set_type` |
| 6 | 1 | `mod_info` |
| 7 | 1 | `feat_info` |
| 8 | 1 | `oth_info` |
| 12 | 4 | `hl_dest` |
| 16 | 1 | `more_info` |
| 17 | 1 | `ymore_info` |
| 18 | 2 | `disp_set_type` |
| 20 | 4 | `e_uid` |
| 24 | 2 | `nsd_info` |
| 26 | 1 | `yamore_info` |
| 27 | 1 | `ybmore_info` |
| 28 | 8 | `mwl_ext` |
| 36 | 2 | `m_trk_grp` |
| 38 | 1 | `mwi_type` |
| 39 | 1 | `disp_len` |
| 40 | 1 | `spare` |
| 41 | 1 | `h320_conv` |
| 42 | 1 | `vis_auto_start` |
| 43 | 1 | `vis_echo_dgts` |
| 44 | 4 | `mtmdatauid` |
| 48 | 1 | `laser` |
| 52 | 8 | `emloc_ext` |
| 60 | 2 | `st_flags` |
| 62 | 2 | `st_flags1` |
| 64 | 2 | `st_flags2` |
| 66 | 2 | `st_flags3` |
| 68 | 2 | `st_flags4` |
| 70 | 2 | `st_flags5` |
| 72 | 2 | `st_flags6` |
| 74 | 2 | `ip_grp_id` |
| 76 | 2 | `srvgk_nnidx` |
| 78 | 1 | `always_use` |
| 80 | 2 | `last_netrgn` |
| 84 | 4 | `def_ext_uid` |
| 88 | 1 | `language` |
| 89 | 1 | `xoip_edpt_type` |
| 90 | 1 | `type_of_3pcc` |
| 92 | 2 | `stn_adm_location` |
| 94 | 1 | `sip_stn_dc_rch` |
| 95 | 71 | `features` |
| 166 | 71 | `features2` |
| 237 | 10 | `systemid` |
| 247 | 72 | `sm_ipv4_addr` |
| 320 | 8 | `sm_ipv4_nidx` |
| 328 | 72 | `sm_ipv6_addr` |
| 400 | 8 | `sm_ipv6_nidx` |
| 408 | 2 | `cor_id` |

Important alignment gaps:

| Region | Description |
|---|---|
| Offset 9 through 11 | Alignment before `hl_dest` |
| Offset 49 through 51 | Alignment before `emloc_ext` |
| Offset 79 | Alignment around `last_netrgn` |
| Offset 82 through 83 | Alignment before `def_ext_uid` |
| Offset 91 | Alignment before `stn_adm_location` |
| Final bytes after `cor_id` | Trailing structure padding |

The `PR_STN` structure contains nested bit-field structures:

```text
st_flags
st_flags1
st_flags2
st_flags3
st_flags4
st_flags5
st_flags6
```

The layout parser must preserve nested bit-field information rather than flattening it incorrectly.

The complete GDB layout contains fields such as:

```text
SBITFLD cpn_restrict : 2;
SBITFLD mm_eanswer : 1;
SBITFLD mm_lamp : 1;
SBITFLD sl_mode : 1;
SBITFLD mm_mode : 2;
```

These fields have bit positions in addition to byte offsets.

---

## 16. Raw PREC Examples

### 16.1 PR_EXT Example

Raw record:

```text
PR_EXT 8aa10004 00000000 00000000 0000971d 00000000 0000ffff 01000000 00000000
```

Known interpretation:

```text
PREC type: PR_EXT
Extension: 1008
Digit count: 4
Principal UID: 0000971d
Compiled structure size: 32 bytes
```

The first word contains both packed extension information and the digit count:

```text
8aa10004
```

The exact decoding is CM-specific.

Do not assume that every 32-bit word in every PREC uses the same transformation.

For this example, the encoded extension portion is related to:

```text
8aa1
```

The extension is decoded by the existing CM-specific nibble convention to:

```text
1008
```

The decoder must remain field-specific.

Do not implement a generic rule such as:

```text
reverse every word
```

unless the particular field definition proves that rule is correct.

---

### 16.2 PR_EXT Memory Mapping

Using the compiled layout:

```text
PR_EXT
total size: 32 bytes
```

The logical mapping is:

| Offset | Size | Field | Known value |
|---:|---:|---|---|
| 0 | 2 | `no_digits` | 4 |
| 2 | 8 | `ext` | encoded extension data |
| 10 | 2 | compiler padding | not a field |
| 12 | 4 | `p_uid` | `0000971d` |
| 16 | 2 | `ctbl_idx` | `0000` |
| 18 | 2 | `ctbl2_idx` | `0000` |
| 20 | 2 | `lainfo_idx` | `0000` |
| 22 | 1 | `cidx_type` | `ff` |
| 23 | 1 | `cidx2_type` | `ff` |
| 24 | 1 | `ctbl_opt` | `01` |
| 25 | 1 | `is_xdid` | `00` |
| 26 | 1 | `free_xdid` | `00` |
| 27 | 1 | `utype` | `00` |
| 28 | 1 | `list_cmdext` | `00` |
| 29 | 3 | trailing padding | not a field |

Raw PREC words may not visually align one-for-one with fields because:

- The dump uses CM record encoding.
- Fields can be packed into words.
- The C structure contains compiler alignment.
- Some values use nibble encoding.
- Some fields use byte-order conventions.
- Some fields require domain-specific decoding.

---

### 16.3 PR_STN Example

A station record was obtained with:

```text
prec pr_stn r l0xe
```

The dump began with:

```text
PR_STN 0000000e 100c00e0 00000000 00000000 04e00c00 00000000 00000000 00006368
PR_STN 00000000 00000000 01010000 00000000 00000000 00006368 00000000 194c0042
PR_STN 094da000 c0000000 ffff0000 00000000 00000000 00000000 00000004 00020000
```

It continued through the complete 412-byte record.

Record-size calculation:

```text
12 complete lines × 8 words × 4 bytes = 384 bytes
1 final line × 7 words × 4 bytes      = 28 bytes
                                      ------
                                        412 bytes
```

This matches:

```text
sizeof(struct pr_stn) = 412
```

The first value identifies the requested station:

```text
UID: 0000000e
```

Important warning:

The raw PREC dump is CM-encoded. The layout supplies offsets and sizes, but field-specific decoding is still required for:

- `set_type`
- `mwl_ext`
- `emloc_ext`
- Bit-field structures
- Address fields
- Encoded UIDs
- Feature arrays

---

## 17. Layout Fixture Storage

For local development, the layout fixtures should eventually be stored in a predictable location.

The original DRCCD prototype location was:

```text
/home/mcnuttd/precstruct_layouts/cm10.2/
```

The corresponding Windows development location should be:

```text
C:\Users\mcnuttd\precstruct_layouts\cm10.2\
```

Recommended fixture files:

```text
C:\Users\mcnuttd\precstruct_layouts\cm10.2\pr_ext.ptype
C:\Users\mcnuttd\precstruct_layouts\cm10.2\pr_mobd.ptype
C:\Users\mcnuttd\precstruct_layouts\cm10.2\pr_stn.ptype
```

For tests, sanitized copies should eventually be placed inside the repository:

```text
Auditor3.Tests\Fixtures\Layouts\cm10.2\pr_ext.ptype
Auditor3.Tests\Fixtures\Layouts\cm10.2\pr_mobd.ptype
Auditor3.Tests\Fixtures\Layouts\cm10.2\pr_stn.ptype
```

Do not commit:

- Customer data.
- Production paths that reveal sensitive infrastructure unless approved.
- GDB logs containing credentials.
- Full production dumps.
- Authentication data.

The `.ptype` layout output itself may be safe to commit only after confirming the applicable data-classification policy.

---

## 18. Next Development Milestone

The next milestone is:

```text
Build a deterministic PREC analysis context.
```

Do not begin with:

- WebUI integration.
- Network calls.
- WPF changes.
- SAT/TCM execution.
- Repair execution.
- Live CM access.

The next feature branch should be:

```text
feature/prec-analysis-context
```

Create it from an updated clean `master`.

The main new class should be:

```text
Auditor3/Assistant/PrecAnalysisContextBuilder.cs
```

Possible supporting interface:

```text
Auditor3/Assistant/IPrecAnalysisContextBuilder.cs
```

The exact interface should be decided after reviewing the existing models and tests.

---

## 19. Context Builder Responsibilities

The context builder should combine:

```text
PrecLayout
PrecLayoutField
Raw PREC record
PrecFieldValue objects
CM release
Application version
Header/source information
Related record summaries
Audit evidence
Deterministic proposed fixes
```

and produce:

```text
AssistantContext
```

The builder must:

- Preserve PREC type.
- Preserve C structure name.
- Preserve CM release.
- Preserve layout source file.
- Preserve structure source line.
- Preserve compiled structure size.
- Record dump size.
- Preserve raw field values.
- Preserve decoded field values separately.
- Mark unknown values explicitly.
- Add compiled-layout evidence.
- Add raw-record evidence only when permitted.
- Detect record-size mismatches.
- Report missing layout information.
- Avoid network access.
- Avoid modifying audit results.
- Avoid modifying repair behavior.
- Avoid accessing live CM systems.

---

## 20. Suggested Context Builder API

A possible first version:

```csharp
public interface IPrecAnalysisContextBuilder
{
    AssistantContext Build(
        PrecLayout layout,
        IReadOnlyList<PrecFieldValue> fields,
        string rawPrec,
        string cmRelease,
        string applicationVersion);
}
```

Possible implementation:

```csharp
public sealed class PrecAnalysisContextBuilder
    : IPrecAnalysisContextBuilder
{
    public AssistantContext Build(
        PrecLayout layout,
        IReadOnlyList<PrecFieldValue> fields,
        string rawPrec,
        string cmRelease,
        string applicationVersion)
    {
        // Build deterministic context.
    }
}
```

This API may need to change if the existing domain objects provide a better source of:

- Raw record lines.
- PREC type.
- UID.
- Port.
- Release.
- Related records.
- Audit results.

Review the repository before finalizing the API.

---

## 21. Record-Size Validation

The context builder should compare:

```text
Raw dump size
```

with:

```text
Compiled layout total size
```

If the sizes match:

```text
RecordSizeStatus = Match
```

If they differ:

```text
RecordSizeStatus = Mismatch
```

If one value is unavailable:

```text
RecordSizeStatus = Unknown
```

Do not silently truncate, pad, or reinterpret a mismatched record.

Recommended future model addition:

```csharp
public string RecordSizeStatus { get; init; } = "Unknown";
```

or a strongly typed enum:

```csharp
public enum RecordSizeStatus
{
    Unknown,
    Match,
    Mismatch
}
```

The builder should add evidence such as:

```text
Raw dump size: 28 bytes
Compiled structure size: 32 bytes
Status: mismatch
```

---

## 22. Proposed Context Builder Tests

Add tests for:

1. Valid `PR_EXT` context.
2. Valid `PR_MOBD` context.
3. Valid `PR_STN` context.
4. Matching raw and compiled sizes.
5. Mismatched raw and compiled sizes.
6. Missing compiled size.
7. Missing raw dump.
8. Empty field collection.
9. Unknown decoded value.
10. Raw and decoded values remain distinct.
11. Layout source is included.
12. Compiled-layout evidence is included.
13. Raw evidence is included only when allowed.
14. Release is preserved.
15. Application version is preserved.
16. Null layout handling.
17. Invalid or empty PREC type handling.
18. Duplicate field handling.
19. Padding fields are not treated as ordinary values.
20. Nested bit-field metadata is preserved.

---

## 23. Section 3 Completion

---

## 24. Step-by-Step Implementation Sequence

Work one small, verifiable step at a time.

Do not combine multiple milestones into one large change.

### Step 1 - Verify the repository

From the repository root:

```powershell
git branch --show-current
git status --short
git log -5 --oneline
```

Expected branch:

```text
master
```

Expected status:

```text
clean
```

If `git status --short` shows changes, stop and inspect them.

Do not discard changes automatically.

---

### Step 2 - Update master

```powershell
git switch master
git pull --ff-only
```

If Git reports local changes that prevent switching or pulling, stop and review them.

Do not use a destructive reset.

---

### Step 3 - Run the baseline build and tests

```powershell
dotnet build .\Auditor3.sln -c Debug
dotnet test .\Auditor3.sln -c Debug
```

Record the result before beginning the new milestone.

Expected known result:

```text
Build succeeded
5 tests passed
0 tests failed
```

---

### Step 4 - Create the feature branch

```powershell
git switch -c feature/prec-analysis-context
```

Verify:

```powershell
git branch --show-current
```

Expected:

```text
feature/prec-analysis-context
```

---

### Step 5 - Inspect the existing models

Read the following files before writing the builder:

```powershell
Get-Content .\Auditor3\Assistant\AssistantContext.cs
Get-Content .\Auditor3\Assistant\AssistantEvidence.cs
Get-Content .\Auditor3\Assistant\PrecLayoutParser.cs
Get-Content .\Auditor3\Models\Assistant\PrecLayout.cs
Get-Content .\Auditor3\Models\Assistant\PrecLayoutField.cs
Get-Content .\Auditor3\Models\Assistant\PrecFieldValue.cs
```

Confirm the actual property names and types. Do not rely only on this handoff document.

---

### Step 6 - Inspect representative PREC classes

Read:

```powershell
Get-Content .\Auditor3\PRECs\PR_EXT.cs
Get-Content .\Auditor3\PRECs\PR_MOBD.cs
Get-Content .\Auditor3\PRECs\PR_STN.cs
```

Also inspect:

```powershell
Get-Content .\Auditor3\Modules\PRECParser.cs
Get-Content .\Auditor3\Database.cs
```

Determine:

- How raw PREC lines are stored.
- How PREC types are identified.
- How UIDs are represented.
- How field-specific decoding currently works.
- Which values can be reused.
- Which values should not be decoded generically.

---

### Step 7 - Decide whether `AssistantContext` needs changes

Before writing the builder, decide whether to add:

```csharp
public string RawPrec { get; init; }
```

and whether to add:

```csharp
public string RecordSizeStatus { get; init; }
```

Do not add fields simply because they appear in the plan.

Add only fields needed by the first deterministic context milestone.

If a model change is needed:

1. Make the smallest change.
2. Update tests.
3. Build.
4. Commit separately if practical.

---

### Step 8 - Create the context builder interface

Suggested file:

```text
Auditor3/Assistant/IPrecAnalysisContextBuilder.cs
```

Possible initial interface:

```csharp
using System.Collections.Generic;

namespace Auditor3;

public interface IPrecAnalysisContextBuilder
{
    AssistantContext Build(
        PrecLayout layout,
        IReadOnlyList<PrecFieldValue> fields,
        string rawPrec,
        string cmRelease,
        string applicationVersion);
}
```

The interface should remain:

- Deterministic.
- Synchronous unless there is a strong reason otherwise.
- Independent of WPF.
- Independent of HTTP.
- Independent of CM connections.
- Easy to unit test.

---

### Step 9 - Create the builder implementation

Suggested file:

```text
Auditor3/Assistant/PrecAnalysisContextBuilder.cs
```

Initial responsibilities:

1. Validate the layout argument.
2. Validate or normalize the PREC type.
3. Copy layout metadata.
4. Copy field values.
5. Preserve raw values.
6. Preserve decoded values.
7. Add compiled-layout evidence.
8. Add raw-record evidence only if the model supports policy control.
9. Calculate dump size when possible.
10. Compare dump size with compiled size.
11. Add mismatch information.
12. Return an `AssistantContext`.

The builder must not:

- Call `HttpClient`.
- Call `WebUiAssistantService`.
- Call `CMConnection`.
- Call `Shell`.
- Execute scripts.
- Modify `Database`.
- Modify `Fixer`.
- Change audit counters.

---

### Step 10 - Add one PR_EXT test

Initially add only one focused test.

The test should verify:

- `PR_EXT` is preserved.
- `pr_ext` structure name is preserved.
- `cm10.2` release is preserved.
- Compiled size is preserved.
- Field values are preserved.
- Raw and decoded values remain separate.
- Compiled-layout evidence is present.

Use an in-memory layout and field list first. Do not require external files for the first unit test.

---

### Step 11 - Build and run tests

```powershell
dotnet build .\Auditor3.sln -c Debug
dotnet test .\Auditor3.sln -c Debug
```

If the test fails:

1. Read the first failure.
2. Inspect the actual model property names.
3. Fix one issue.
4. Rerun the test.
5. Do not add unrelated changes.

---

### Step 12 - Add mismatch handling

Add a deterministic mismatch result.

Possible approach:

```text
Match
Mismatch
Unknown
```

The exact model can be a string initially or an enum if the codebase supports it cleanly.

Test at least:

```text
raw size = compiled size
raw size != compiled size
compiled size unavailable
raw size unavailable
```

---

### Step 13 - Add PR_MOBD and PR_STN tests

After PR_EXT works, add tests for:

```text
PR_MOBD
PR_STN
```

Use sanitized layout fixtures.

Do not implement complete semantic decoding of every PR_STN field in this milestone.

For PR_STN, initially verify:

- Total size is 412.
- Top-level offsets are preserved.
- Nested bit-field metadata is not discarded.
- Raw values remain available.
- Unknown values remain marked unknown.

---

### Step 14 - Review the diff

```powershell
git diff
git diff --check
git status --short
```

Review every changed file.

Ensure that unrelated files are not modified:

```text
Auditor3/Modules/Auditor.cs
Auditor3/Scripts/*
MainWindow.xaml
MainWindow.xaml.cs
```

unless the milestone specifically requires them.

---

### Step 15 - Commit the context-builder milestone

Stage only intended files:

```powershell
git add -- Auditor3/Assistant Auditor3/Models/Assistant Auditor3.Tests
```

Review the staged summary:

```powershell
git diff --cached --stat
git diff --cached --check
```

Commit:

```powershell
git commit -m "Build deterministic PREC analysis context"
```

---

### Step 16 - Push the branch

```powershell
git push -u origin feature/prec-analysis-context
```

Create a pull request with:

```text
Base branch: master
Compare branch: feature/prec-analysis-context
```

The pull request description should state:

- No network integration was added.
- No WPF UI was added.
- No SAT/TCM execution was added.
- Existing audit behavior is unchanged.
- The builder is deterministic and local.
- Tests were added and passed.

---

## 25. Local Explanation Mode

After the context builder is complete, implement local deterministic explanations before connecting to the AI service.

Suggested component:

```text
Auditor3/Assistant/LocalAssistantService.cs
```

This component should not call the network.

It may generate explanations such as:

```text
PR_EXT is a 32-byte compiled structure.

The p_uid field begins at offset 12 and has a size of 4 bytes.

The ext field begins at offset 2 and has a compiled size of 8 bytes.

The layout contains a 2-byte compiler alignment hole before p_uid.
```

Local explanation topics should include:

- Structure size.
- Field offsets.
- Field sizes.
- Padding.
- Raw values.
- Decoded values.
- Unknown values.
- Record-size mismatches.
- Audit messages.
- Related records.
- DREC/PREC mappings.
- Deterministic proposed fixes.

Local explanation output should label facts clearly:

```text
Authoritative fact:
The compiled layout reports a total size of 32 bytes.

Interpretation:
The current raw record appears to match the compiled size.

Verification:
Confirm the layout file belongs to the same CM release and build.
```

---

## 26. Future WebUI Service Integration

Do not implement the network adapter until the approved service contract has been obtained.

Required documentation:

- Official endpoint.
- Official .NET SDK or client library.
- Authentication mechanism.
- SSO behavior.
- Required request headers.
- Request body schema.
- Response body schema.
- Streaming support.
- Error response schema.
- Timeout recommendations.
- Retry recommendations.
- Rate limits.
- Maximum context size.
- TLS requirements.
- Proxy requirements.
- Test environment.
- Production environment.
- Data-retention policy.
- Logging policy.
- Browser automation policy.

The eventual implementation should be:

```text
IAssistantService
    ├── DisabledAssistantService
    ├── LocalAssistantService
    ├── MockAssistantService
    └── WebUiAssistantService
```

The network service should use:

- `HttpClient`.
- `CancellationToken`.
- Explicit timeout.
- Correlation ID.
- Response validation.
- Safe error handling.
- No secret logging.
- No command execution.

---

## 27. WebUI Request Flow

The future request flow should be:

```text
Engineer selects a PREC or audit result
        ↓
Auditor3 builds deterministic AssistantContext
        ↓
User sees context categories
        ↓
Redactor removes prohibited data
        ↓
PromptBuilder creates the request
        ↓
WebUiAssistantService sends approved request
        ↓
ResponseValidator validates the response
        ↓
UI displays advisory response and evidence
```

The service should not receive an entire application dump by default.

For a question about `PR_EXT`, the context might contain:

```text
PREC type
CM release
structure name
compiled size
selected field values
raw values if permitted
decoded values
related record summaries
audit result
deterministic repair proposal
```

It should not automatically contain:

```text
all stations
all trunks
all announcements
shell history
connection state
passwords
ASG prompts
unrelated customer records
```

---

## 28. Response Validation

A future `AssistantResponseValidator` should inspect returned text for:

- Empty responses.
- Unexpected response structure.
- Unsupported command formats.
- Command-like text.
- Claims of execution.
- Claims of modifying CM.
- Unverified field offsets.
- Contradictions with authoritative context.

The validator should not attempt to prove that an explanation is correct. It should identify obvious policy violations and uncertainty.

Potential warnings:

```text
The response contains command-like text.
The response refers to data not present in the supplied context.
The response claims an action was executed.
The response provides an offset not found in the supplied layout.
```

Any warning should be visible to the engineer.

---

## 29. Repair Safety

The existing deterministic `Fixer` remains authoritative.

The AI may explain:

```text
Auditor3 proposed the following repair:
prec pr_fext a ...
```

The AI must not silently invent a replacement repair.

If AI output contains a command:

1. Display it as read-only text.
2. Mark it as AI-generated.
3. Compare it with the deterministic `Fixer` output.
4. Require explicit engineer review.
5. Reject unsupported commands.
6. Never execute it directly.
7. Route approved repair generation through existing Auditor3 code.

Required warning:

```text
AI-generated command text is advisory only and has not been executed.
```

---

## 30. WPF Integration Plan

Do not change the WPF UI during the deterministic context-builder milestone.

The future UI may be a panel or separate window.

Suggested future files:

```text
Auditor3/WPF/AssistantWindow.xaml
Auditor3/WPF/AssistantWindow.xaml.cs
```

Initial controls:

- Current PREC summary.
- Current UID or key.
- Current audit code.
- Question text box.
- Ask button.
- Cancel button.
- Response output.
- Evidence output.
- Context categories.
- Warning display.
- Copy response button.
- Clear button.
- Service status.

The UI must:

- Remain responsive.
- Use asynchronous calls.
- Support cancellation.
- Disable duplicate requests.
- Display errors safely.
- Show the advisory warning.
- Work in light and dark themes.
- Never directly execute response text.

---

## 31. Security and Privacy Requirements

Never send:

```text
Passwords
ASG challenges
ASG responses
SSH private keys
Authentication tokens
Cookies
Authorization headers
Session identifiers
Shell history
Unredacted crash logs
Unrelated customer records
```

Before sending any future request:

1. Apply data minimization.
2. Apply redaction.
3. Validate the service endpoint.
4. Limit context size.
5. Add a correlation ID.
6. Apply timeout and cancellation.
7. Avoid writing the payload to logs.
8. Avoid retaining the payload longer than necessary.

Potentially sensitive data requiring policy review:

```text
Extensions
UIDs
IP addresses
Customer names
Station names
Trunk information
Routing information
Raw translation data
Source paths
Internal CM release paths
```

Do not assume that internal service approval automatically means every category of customer data is approved for transmission.

---

## 32. Test Plan for Future Work

### Unit tests

Add tests for:

- Layout parsing.
- Context building.
- Record-size validation.
- Raw/decoded separation.
- Redaction.
- Prompt construction.
- Response validation.
- Disabled service.
- Local service.
- Command detection.
- Cancellation.
- Timeout behavior.

### Integration tests

Use a mock service or approved test endpoint.

Test:

- Authentication.
- Serialization.
- Deserialization.
- HTTP success.
- HTTP failure.
- Timeout.
- Cancellation.
- Rate limiting.
- Invalid response.
- Oversized context.
- Service unavailable.

### UI tests

Verify:

- Assistant opens.
- Assistant can be disabled.
- Question submission works.
- Cancel works.
- Error display works.
- Long responses scroll.
- Evidence display works.
- Warning display works.
- Themes work.
- No command executes automatically.
- Existing audit workflows continue to work.

---

## 33. Pull Request Requirements

Every future pull request should include:

### Summary

What changed.

### Scope

What was intentionally not changed.

### Validation

Commands run and results:

```text
dotnet build Auditor3.sln -c Debug
dotnet test Auditor3.sln -c Debug
```

### Safety

State whether the change:

- Uses network access.
- Connects to CM.
- Executes SAT/TCM.
- Changes repair behavior.
- Sends customer data.
- Changes the UI.

### Review Notes

Mention:

- Known limitations.
- Temporary behavior.
- Follow-up work.
- Any required security review.

---

## 34. Files to Provide to a New Chat

### Always provide

```text
AI_AGENT_PLAN.md
AI_ASSISTANT_HANDOFF.md
Auditor3.sln
Auditor3/Auditor3.csproj
Auditor3.Tests/Auditor3.Tests.csproj
Auditor3.Tests/UnitTest1.cs
```

### For layout/parser work

```text
Auditor3/Assistant/PrecLayoutParser.cs
Auditor3/Assistant/AssistantContext.cs
Auditor3/Assistant/AssistantEvidence.cs
Auditor3/Models/Assistant/PrecLayout.cs
Auditor3/Models/Assistant/PrecLayoutField.cs
Auditor3/Models/Assistant/PrecFieldValue.cs
```

Also provide sanitized layout fixtures:

```text
pr_ext.ptype
pr_mobd.ptype
pr_stn.ptype
```

### For context-builder work

```text
Auditor3/Assistant/PrecAnalysisContextBuilder.cs
Auditor3/Assistant/IPrecAnalysisContextBuilder.cs
```

if those files exist, plus:

```text
Auditor3/PRECs/PR_EXT.cs
Auditor3/PRECs/PR_MOBD.cs
Auditor3/PRECs/PR_STN.cs
Auditor3/Modules/PRECParser.cs
Auditor3/Database.cs
```

### For assistant service work

```text
Auditor3/Assistant/IAssistantService.cs
Auditor3/Assistant/AssistantRequest.cs
Auditor3/Assistant/AssistantResponse.cs
Auditor3/Assistant/AssistantSettings.cs
Auditor3/Assistant/DisabledAssistantService.cs
Auditor3/Assistant/IAssistantRedactor.cs
Auditor3/Assistant/AssistantRedactor.cs
```

Also provide the approved non-secret `webui.avaya.com` API contract.

### For audit work

```text
Auditor3/Modules/Auditor.cs
Auditor3/Database.cs
Auditor3/Modules/Fixer.cs
Auditor3/Models/AuditResult.cs
Auditor3/Services/AuditEngine.cs
Auditor3/Services/AuditStrategies.cs
Auditor3/Services/Strategies/AllStationStrategies.cs
Auditor3/Services/Strategies/AllAnnouncementStrategies.cs
Auditor3/Services/Strategies/AllTrunkStrategies.cs
```

### For WPF work

```text
Auditor3/MainWindow.xaml
Auditor3/MainWindow.xaml.cs
Auditor3/App.xaml
Auditor3/App.xaml.cs
Auditor3/WPF/Styles.xaml
Auditor3/WPF/Icons.xaml
```

### For investigation script work

```text
Auditor3/Scripts/arecfind.sh
Auditor3/Scripts/findprecs.sh
Auditor3/Scripts/precstruct.sh
```

### Do not provide

```text
Passwords
ASG challenges
ASG responses
SSH private keys
Authentication tokens
Cookies
Customer credentials
Production secrets
Unredacted connection logs
Shell history
Crash logs containing credentials
bin/
obj/
packages/
Build output
```

---

## 35. Suggested New Chat Opening Message

Use this opening message in a future chat:

```text
I am continuing development of the Auditor3 AI engineering assistant.

Please read these files first:

AI_AGENT_PLAN.md
AI_ASSISTANT_HANDOFF.md

Current repository state:

- Repository: Auditor3_Modernized
- Application: .NET 9 WPF
- Base branch: master
- AI foundation is merged.
- PREC investigation scripts are merged.
- AUDIT-S34 and AUDIT-S35 are intentionally disabled temporarily.
- The working tree should be clean.
- The latest known tests are 5 passing.
- No webui.avaya.com integration exists yet.
- No WPF assistant UI exists yet.
- The next milestone is deterministic PREC analysis context construction.
- Do not add network integration yet.
- Do not modify live CM behavior.
- Do not execute SAT or TCM commands.
- Work one step at a time.
- Give exact PowerShell commands.
- Do not stage or commit unrelated files.

The immediate task is to inspect the current models and design the
PrecAnalysisContextBuilder.cs implementation.
```

---

## 36. Definition of Done for the Next Milestone

The deterministic PREC analysis context milestone is complete when:

- [ ] A context builder exists.
- [ ] The builder accepts a parsed `PrecLayout`.
- [ ] The builder accepts raw or structured PREC values.
- [ ] The builder preserves raw values.
- [ ] The builder preserves decoded values.
- [ ] The builder records the CM release.
- [ ] The builder records the layout source.
- [ ] The builder records the structure name.
- [ ] The builder records the compiled size.
- [ ] The builder records or calculates dump size.
- [ ] Size mismatches are explicit.
- [ ] Unknown values are explicit.
- [ ] Compiled layout evidence is included.
- [ ] Raw evidence is policy-controlled.
- [ ] No network calls occur.
- [ ] No UI changes are required.
- [ ] Existing audit behavior is unchanged.
- [ ] Unit tests cover normal and error paths.
- [ ] `dotnet build` passes.
- [ ] `dotnet test` passes.
- [ ] The work is committed on a feature branch.
- [ ] The pull request targets `master`.

---

## 37. Change Log

### 2026-08-08

- Added detailed continuation instructions.
- Documented the current merged repository state.
- Documented completed assistant files.
- Documented completed PREC layout models.
- Documented current parser behavior.
- Documented investigation scripts.
- Documented CM 10.2 layout information.
- Documented known raw PREC examples.
- Documented the next deterministic context-builder milestone.
- Documented the future WebUI integration boundary.
- Documented repair safety requirements.
- Documented security and privacy requirements.
- Documented testing requirements.
- Documented future pull-request requirements.
- Documented files to provide to future AI sessions.

Mark deterministic context construction as completed.
Document LocalAssistantService.
Document WebUiAssistantService as an implemented adapter prototype.
Update the test count and test files.
State clearly that no WPF assistant UI is wired in.
State that service-contract/security approval granted
