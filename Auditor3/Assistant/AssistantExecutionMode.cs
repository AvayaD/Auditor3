namespace Auditor3;

/// <summary>
/// Defines the cumulative execution permissions available to the assistant.
///
/// The values are ordered from most restrictive to least restrictive.
/// Each higher mode includes the capabilities of the preceding mode.
/// </summary>
public enum AssistantExecutionMode
{
    /// <summary>
    /// No assistant functionality is available.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Explains already-supplied local Auditor3 data only.
    /// </summary>
    OfflineReadOnly = 1,

    /// <summary>
    /// May request approved read-only Auditor3 and DRCCD analysis.
    /// </summary>
    ReadOnly = 2,

    /// <summary>
    /// May request approved read-only operations against live systems.
    /// Live repair and translation modification remain prohibited.
    /// </summary>
    LiveReadOnly = 3,

    /// <summary>
    /// May request approved operations on a designated lab, including
    /// explicitly approved lab repair workflows.
    /// </summary>
    LabAssisted = 4
}