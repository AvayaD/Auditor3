namespace Auditor3;

/// <summary>
/// Structured request for one approved Auditor3 capability.
///
/// This is data only. It does not execute commands or open connections.
/// </summary>
public sealed class AssistantCapabilityRequest
{
    /// <summary>
    /// The governed capability being requested.
    /// </summary>
    public AssistantCapability Capability { get; init; }

    /// <summary>
    /// PREC type used by layout or PREC-related capabilities.
    /// </summary>
    public string PrecType { get; init; } = string.Empty;

    /// <summary>
    /// Action used by the findprecs mapping capability.
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>
    /// Object used by the findprecs mapping capability.
    /// </summary>
    public string Object { get; init; } = string.Empty;

    /// <summary>
    /// Optional qualifier used by the findprecs mapping capability.
    /// </summary>
    public string Qualifier { get; init; } = string.Empty;

    /// <summary>
    /// Target identifier, such as a designated lab identifier.
    /// </summary>
    public string TargetId { get; init; } = string.Empty;

    /// <summary>
    /// Optional correlation identifier for tracing one request.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// Indicates that the engineer explicitly approved this request.
    /// Policy validation must still be performed separately.
    /// </summary>
    public bool EngineerApproved { get; init; }
}