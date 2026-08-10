namespace Auditor3;

/// <summary>
/// Names an approved Auditor3 capability that the assistant may request.
///
/// These names describe governed operations. They are not shell commands.
/// </summary>
public enum AssistantCapability
{
    /// <summary>
    /// Use the engineer-selected PREC context.
    /// </summary>
    SelectedPrecContext = 0,

    /// <summary>
    /// Retrieve and explain a compiled PREC layout.
    /// </summary>
    PrecLayout = 1,

    /// <summary>
    /// Retrieve AREC/DREC/PREC mappings using findprecs.
    /// </summary>
    FindPrecsMapping = 2,

    /// <summary>
    /// Retrieve related PREC records.
    /// </summary>
    RelatedPrecData = 3,

    /// <summary>
    /// Retrieve authoritative audit-failure evidence.
    /// </summary>
    AuditFailureEvidence = 4,

    /// <summary>
    /// Explain an existing deterministic Auditor3 repair recommendation.
    /// </summary>
    DeterministicRepairExplanation = 5,

    /// <summary>
    /// Collect PREC data from a designated lab.
    /// </summary>
    LabCollection = 6,

    /// <summary>
    /// Run an Auditor3 audit against a designated lab dataset.
    /// </summary>
    LabAudit = 7,

    /// <summary>
    /// Generate a deterministic fix script for a designated lab.
    /// </summary>
    LabFixScriptGeneration = 8,

    /// <summary>
    /// Execute an approved fix workflow on a designated lab only.
    /// </summary>
    LabRepairExecution = 9,

    /// <summary>
    /// Re-collect and verify a designated lab after an approved operation.
    /// </summary>
    LabVerification = 10
}