namespace Auditor3;

/// <summary>
/// Request sent to the configured AI assistant service.
/// </summary>
public sealed class AssistantRequest
{
    public string Question { get; init; } = string.Empty;

    public string SystemInstructions { get; init; } = string.Empty;

    public AssistantContext Context { get; init; } = new();

    public string CorrelationId { get; init; } = string.Empty;
}