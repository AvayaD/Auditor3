namespace Auditor3;

/// <summary>
/// Configuration for the optional AI assistant.
/// No network request is made by this settings model.
/// </summary>
public sealed class AssistantSettings
{
    /// <summary>
    /// Enables the assistant feature.
    /// Keep false until the service integration is approved.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Controls which cumulative assistant capabilities are permitted.
    /// Disabled is the safe default.
    /// </summary>
    public AssistantExecutionMode ExecutionMode { get; init; } =
        AssistantExecutionMode.Disabled;

    /// <summary>
    /// Approved webui.avaya.com service endpoint.
    /// This must be supplied through approved configuration.
    /// </summary>
    public string ServiceEndpoint { get; init; } = string.Empty;

    /// <summary>
    /// Maximum time allowed for one assistant request.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 60;

    /// <summary>
    /// Maximum serialized context size in bytes.
    /// </summary>
    public int MaximumContextBytes { get; init; } = 250_000;

    /// <summary>
    /// Whether raw PREC words may be included in a request.
    /// </summary>
    public bool SendRawPrecData { get; init; }

    /// <summary>
    /// Whether related PREC summaries may be included.
    /// </summary>
    public bool SendRelatedRecords { get; init; } = true;

    /// <summary>
    /// Whether compiled layout information may be included.
    /// </summary>
    public bool SendCompiledLayout { get; init; } = true;

    /// <summary>
    /// Whether source declarations may be included.
    /// </summary>
    public bool SendSourceDeclaration { get; init; }

    /// <summary>
    /// Whether assistant request metadata may be logged.
    /// </summary>
    public bool EnableRequestLogging { get; init; }

    /// <summary>
    /// Whether AI-generated command-like text may be displayed
    /// as a proposed, read-only suggestion.
    /// </summary>
    public bool AllowCommandSuggestions { get; init; }
}