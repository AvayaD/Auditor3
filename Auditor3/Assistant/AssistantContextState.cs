using System;

namespace Auditor3;

public enum AssistantMode
{
    Disabled,
    Local,
    WebAI
}

public enum AssistantContextStatus
{
    None,
    Ready,
    Requesting,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// Represents the current assistant UI/session state.
/// The authoritative PREC data remains in AssistantContext.
/// </summary>
public sealed class AssistantContextState
{
    public AssistantMode Mode { get; init; } =
        AssistantMode.Disabled;

    public AssistantContextStatus Status { get; init; } =
        AssistantContextStatus.None;

    public AssistantContext Context { get; init; }

    public AssistantResponse LastResponse { get; init; }

    public string ErrorMessage { get; init; } =
        string.Empty;

    public bool HasContext => Context is not null;

    public bool IsBusy =>
        Status == AssistantContextStatus.Requesting;
}