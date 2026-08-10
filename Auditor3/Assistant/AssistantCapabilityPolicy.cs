namespace Auditor3;

/// <summary>
/// Applies execution-mode rules to assistant capability requests.
///
/// This class does not execute commands, open connections, or modify systems.
/// </summary>
public static class AssistantCapabilityPolicy
{
    /// <summary>
    /// Determines whether a capability is permitted.
    /// </summary>
    public static bool IsAllowed(
        AssistantExecutionMode mode,
        AssistantCapability capability,
        bool engineerApproved = false,
        bool targetIsDesignatedLab = false)
    {
        if (mode == AssistantExecutionMode.Disabled)
        {
            return false;
        }

        if (capability == AssistantCapability.SelectedPrecContext ||
            capability == AssistantCapability.DeterministicRepairExplanation)
        {
            return mode >= AssistantExecutionMode.OfflineReadOnly;
        }

        if (capability == AssistantCapability.PrecLayout ||
            capability == AssistantCapability.FindPrecsMapping ||
            capability == AssistantCapability.RelatedPrecData ||
            capability == AssistantCapability.AuditFailureEvidence)
        {
            return mode >= AssistantExecutionMode.ReadOnly;
        }

        if (capability == AssistantCapability.LabCollection ||
            capability == AssistantCapability.LabAudit ||
            capability == AssistantCapability.LabFixScriptGeneration ||
            capability == AssistantCapability.LabRepairExecution ||
            capability == AssistantCapability.LabVerification)
        {
            return mode >= AssistantExecutionMode.LabAssisted
                && targetIsDesignatedLab
                && engineerApproved;
        }

        return false;
    }
}