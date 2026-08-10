using System;
using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Result returned by an Auditor3 capability provider.
///
/// This is data only. It does not execute commands or open connections.
/// </summary>
public sealed class AssistantCapabilityResult
{
    /// <summary>
    /// The capability that produced this result.
    /// </summary>
    public AssistantCapability Capability { get; init; }

    /// <summary>
    /// Indicates whether the capability completed successfully.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Structured result data, when available.
    /// </summary>
    public AssistantContext Context { get; init; }

    /// <summary>
    /// Human-readable summary of the result.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Error message when the capability fails.
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// Correlation identifier for tracing the request.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// Indicates that the result contains command-like text.
    /// Such text remains advisory and must not be executed directly.
    /// </summary>
    public bool ContainsCommandLikeText { get; init; }

    /// <summary>
    /// Warnings generated while processing the capability.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } =
        Array.Empty<string>();
}