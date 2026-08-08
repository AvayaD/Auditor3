namespace Auditor3;

/// <summary>
/// Describes evidence supplied to the AI assistant.
/// </summary>
public sealed class AssistantEvidence
{
    public string Type { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;
}