# Auditor3 AI Engineering Assistant Implementation Plan

## Document Status

| Field | Value |
|---|---|
| Status | Approved for continued implementation |
| Owner and approver | David McNutt (`mcnuttd@avaya.com`) |
| Approval date | 2026-08-08 |
| Application | Auditor3 |
| Target framework | .NET 9 / WPF |
| Approved service | WebAI at `webui.avaya.com` |
| Primary purpose | Read-only engineering assistance |
| Direct AI command execution | Prohibited |

---

## 1. Purpose

The Auditor3 AI assistant helps engineers understand Avaya Communication
Manager translation data.

The assistant may explain:

- PREC records.
- C structure declarations.
- Compiled memory layouts.
- Field offsets and sizes.
- Alignment holes and padding.
- Raw and decoded field values.
- AREC/DREC/PREC relationships.
- Related records.
- Audit failures.
- Existing deterministic Auditor3 repair recommendations.
- Recommended investigation steps.

Auditor3 remains authoritative for:

1. PREC parsing.
2. Field offsets and structure sizes.
3. Field decoding.
4. Audit results.
5. Repair generation.
6. CM state and command execution.

The AI assistant is advisory only.

---

## 2. Approved Scope

The approved first-release scope is:

- Read-only explanations of Auditor3 data.
- Explanation of PREC records and compiled layouts.
- Explanation of audit results.
- Explanation of deterministic Auditor3 repair recommendations.
- Optional use of the approved WebAI service.
- Local explanation mode.
- Evidence and warning display.
- Engineer review of all responses.

The following behavior is prohibited:

- Executing SAT commands.
- Executing TCM commands.
- Executing shell commands.
- Executing repair scripts.
- Modifying CM translations.
- Modifying Auditor3 audit results.
- Treating an AI response as authoritative evidence.
- Automatically applying AI-generated repairs.
- Sending passwords, ASG challenges, ASG responses, tokens, cookies, or private keys.

---

## 3. Current Repository Implementation

The following components are implemented:

- `AssistantContext`
- `AssistantEvidence`
- `AssistantRequest`
- `AssistantResponse`
- `AssistantSettings`
- `IAssistantService`
- `DisabledAssistantService`
- `LocalAssistantService`
- `WebUiAssistantService`
- `IAssistantRedactor`
- `AssistantRedactor`
- `PrecLayout`
- `PrecLayoutField`
- `PrecFieldValue`
- `PrecLayoutParser`
- `IPrecAnalysisContextBuilder`
- `PrecAnalysisContextBuilder`

The repository also contains automated tests for:

- PREC layout parsing.
- Padding and bit-field metadata.
- Deterministic context construction.
- Redaction.
- Local explanations.
- Disabled assistant behavior.
- WebAI HTTP request and response handling.
- HTTP failure handling.
- Missing API-key handling.

The following components are not yet implemented:

- WPF assistant window or panel.
- Assistant service factory.
- Assistant coordinator or view model.
- Production configuration UI.
- Response validation service.
- Formal assistant request logging.
- Full audit-result context integration.
- Full related-record context integration.
- Automatic command execution.

Normal Auditor3 audit, collection, repair, and CM workflows must continue to
work when the assistant is disabled or unavailable.

---

## 4. Approved WebAI Integration

The WebAI integration has been approved by David McNutt.

The complete controlled service contract is maintained in the approved internal
WebAI documentation. This repository records the implementation-facing
details currently used by Auditor3.

- Approved service: `webui.avaya.com`
- Current API gateway:
  `https://gateway.webai.avaya.com/chat/completions`
- Authentication: Bearer authentication.
- Runtime credential source: `WEBAI_KEY` environment variable.
- Current model: `claude-sonnet-4-6`.
- Request format: OpenAI-compatible chat-completions JSON.
- Response format: OpenAI-compatible chat-completions JSON.
- Current configured timeout: 60 seconds.
- Current configured maximum context size: 250,000 bytes.

Detailed service limits, retention, retry, TLS, proxy, and operational
requirements are defined by the approved internal WebAI contract.

Secrets must not be stored in:

- Source code.
- Git.
- `App.config`.
- Plain-text user data files.
- Audit reports.
- Crash reports.
- Request or response logs.

---

## 5. Approved Data Policy

Approved data categories include:

- Auditor3 structured PREC metadata.
- Compiled PREC layout information.
- Field offsets and sizes.
- Field decode status.
- Approved audit-result information.
- Approved deterministic repair recommendations for explanation only.
- Other data explicitly permitted by the approved internal data policy.

The following data must never be transmitted:

- Passwords.
- ASG challenges.
- ASG responses.
- Authentication tokens.
- Cookies.
- Private keys.
- Authorization headers.
- Shell history.
- Unredacted connection logs.
- Unrelated customer data.
- Data outside the approved service and data-handling policy.

Raw PREC words and customer-identifying information remain subject to the
approved data-classification policy and must not be transmitted unless
explicitly permitted.

The default policy should minimize transmitted data.

---

## 6. Required Safeguards

The assistant implementation must:

- Remain optional.
- Remain disabled by default until enabled through approved configuration.
- Apply data minimization.
- Apply redaction before transmission.
- Use asynchronous requests.
- Support cancellation.
- Enforce request timeouts.
- Enforce maximum context size.
- Avoid logging complete payloads by default.
- Preserve correlation IDs.
- Fail without affecting normal auditing.
- Display uncertainty and missing data.
- Label all responses as advisory.
- Display command-like text as read-only content.
- Never execute AI-generated command text.

The assistant must not call:

- `CMConnection`.
- `ShellStream`.
- `Fixer` for automatic execution.
- SAT commands.
- TCM commands.
- Shell commands.

The assistant may explain deterministic output already produced by Auditor3,
but Auditor3 remains responsible for final repair generation and execution.

---

## 7. Architecture

The intended dependency direction is:

```text
WPF UI
  ↓
Assistant coordinator or view model
  ↓
IAssistantService
  ├── DisabledAssistantService
  ├── LocalAssistantService
  └── WebUiAssistantService

## 8. Context Requirements

An `AssistantContext` may contain:

- Application version.
- CM release.
- PREC type.
- Structure name.
- Header or layout source.
- Structure source line.
- Compiled structure size.
- Raw dump size.
- Record-size status.
- Structured field values.
- Mapping details.
- Proposed deterministic fixes.
- Supporting evidence.

Raw and decoded values must remain separate.

Unknown values must remain explicitly unknown. The assistant must not invent:

- Field offsets.
- Field sizes.
- Structure sizes.
- Decoded values.
- Record relationships.
- CM command results.

Record-size status must be one of:

- `Match`
- `Mismatch`
- `Unknown`

A mismatch must be visible to the engineer and must not be silently corrected.

---

## 9. Milestone Status

### Milestone 0 — Planning and Approval

**Status: Approved**

Approval was provided by David McNutt on 2026-08-08.

The approved scope, WebAI integration, and data-handling policy may proceed to
implementation, subject to testing and remaining release controls.

### Milestone 1 — Deterministic PREC Analysis Foundation

**Status: Completed**

Implemented:

- PREC layout models.
- GDB `ptype /o` parsing.
- Structure names.
- Field offsets and sizes.
- Padding and hole detection.
- Bit-field metadata.
- Compiled layout evidence.
- Deterministic context construction.
- Record-size comparison.
- Unit tests.

### Milestone 2 — Local Explanation Mode

**Status: Completed for the current foundation**

Implemented:

- `LocalAssistantService`.
- Local record-size explanations.
- Field and layout explanations.
- Warning generation.
- No-network local operation.
- Unit tests.

Future work may expand local explanations to include:

- Audit-result evidence.
- Related records.
- DREC/PREC mappings.
- Deterministic repair explanations.

### Milestone 3 — WebAI Service Adapter

**Status: Adapter implemented; integration hardening remains**

Implemented:

- `WebUiAssistantService`.
- Runtime API-key lookup through `WEBAI_KEY`.
- Bearer authentication.
- JSON request serialization.
- JSON response parsing.
- HTTP failure handling.
- Cancellation support.
- Correlation IDs.
- Mock-handler tests.

Remaining work:

- Validate every implementation detail against the approved contract.
- Wire the approved configuration into the application.
- Enforce configured timeout and context-size limits.
- Add production response validation.
- Add approved operational error handling.
- Add integration testing against the approved test service.

### Milestone 4 — Redaction and Governance

**Status: Prototype implemented; formal governance controls remain**

Implemented:

- `IAssistantRedactor`.
- `AssistantRedactor`.
- Sensitive evidence-line masking.
- Redaction tests.

Remaining work:

- Confirm the final approved data allowlist.
- Review structured-property redaction.
- Review raw PREC handling.
- Review question and response logging.
- Add a user-visible context preview if required.
- Complete formal security and privacy review records.

### Milestone 5 — WPF Assistant UI

**Status: Not started**

Planned features:

- Assistant availability indicator.
- Context summary.
- Question input.
- Ask button.
- Cancel button.
- Progress indicator.
- Response display.
- Evidence display.
- Warning display.
- Copy response button.
- Clear response button.
- Advisory-response warning.
- Read-only command-suggestion display.

The UI must not execute response text.

### Milestone 6 — Guided Repair Assistance

**Status: Not started**

Planned safeguards:

- Include deterministic `Fixer` output as authoritative context.
- Clearly label AI-generated command-like text.
- Display suggestions in a separate read-only area.
- Compare suggestions with deterministic Auditor3 output.
- Reject unsupported commands.
- Require explicit engineer review.
- Never execute AI-generated commands directly.
- Route final repairs through existing Auditor3 mechanisms.

### Milestone 7 — Production Readiness

**Status: Not started**

Required work:

- Complete unit tests.
- Complete mock-service integration tests.
- Complete approved service testing.
- Complete WPF testing.
- Complete security and privacy review.
- Complete operational documentation.
- Add feature flag and kill switch.
- Confirm service failure behavior.
- Confirm no secret leakage.
- Update changelog and documentation.
- Obtain final release approval.

---

## 10. Testing Requirements

### Unit tests

Tests should cover:

- Layout parsing.
- Structure-name parsing.
- Padding parsing.
- Bit-field parsing.
- Context construction.
- Record-size matching.
- Record-size mismatch.
- Unknown record-size status.
- Raw and decoded value separation.
- Redaction.
- Local explanations.
- Command-like text detection.
- Invalid response handling.
- Cancellation behavior.
- Maximum context-size behavior.

### Integration tests

Use a mock service or approved test endpoint.

Test:

- Authentication.
- Request serialization.
- Response deserialization.
- HTTP success.
- HTTP failure.
- Timeout.
- Cancellation.
- Invalid response.
- Service unavailable.
- Oversized context.
- Rate limiting where applicable.

### Manual WPF tests

Verify:

- Assistant can be disabled.
- Normal Auditor3 operation works without the assistant.
- The UI remains responsive.
- Cancellation works.
- Long responses scroll correctly.
- Warnings are visible.
- Evidence is visible.
- Command-like text is read-only.
- No response text is executed.
- Service failures do not affect auditing.
- No secrets appear in the UI or logs.

---

## 11. Definition of Done

The read-only assistant is ready for release only when:

- The feature is disabled by default.
- The approved WebAI contract is implemented and tested.
- Approved data categories are enforced.
- Redaction is applied before transmission.
- Requests are asynchronous and cancellable.
- Timeouts and context-size limits are enforced.
- Service failures do not affect auditing.
- The WPF UI clearly labels responses as advisory.
- No assistant output can execute commands.
- No assistant output can modify CM translations.
- Deterministic Auditor3 results remain authoritative.
- Unit tests pass.
- Integration tests pass.
- Manual WPF tests pass.
- Security and privacy reviews are recorded.
- Documentation and changelog are updated.
- Final release approval is obtained.

---

## 12. Immediate Next Task

The next implementation milestone is to add the read-only WPF assistant UI
and coordinator.

Before coding:

1. Confirm the working tree is clean.
2. Create a feature branch from updated `master`.
3. Keep the assistant optional.
4. Do not modify CM connection behavior.
5. Do not add SAT or TCM execution.
6. Do not add automatic repair execution.
7. Add tests for the coordinator before wiring the UI.
8. Validate all requests through the redaction boundary.

Recommended branch name:

`feature/assistant-wpf-readonly`

Recommended initial files:

- `Auditor3/Assistant/AssistantServiceFactory.cs`
- `Auditor3/Assistant/AssistantCoordinator.cs`
- `Auditor3/WPF/AssistantWindow.xaml`
- `Auditor3/WPF/AssistantWindow.xaml.cs`

The first UI release should support local explanations before enabling WebAI
requests in the normal application flow.
