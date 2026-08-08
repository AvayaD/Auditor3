using System;
using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Response returned by the configured AI assistant service.
/// </summary>
public sealed class AssistantResponse
{
    public bool Succeeded { get; init; }

    public string Answer { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;

    public string CorrelationId { get; init; } = string.Empty;

    public bool ContainsSuggestedCommands { get; init; }

    public IReadOnlyList<string> Warnings { get; init; } =
        Array.Empty<string>();
}