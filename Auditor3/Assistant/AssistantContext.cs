using System;
using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Structured, deterministic context supplied to the AI assistant.
/// </summary>
public sealed class AssistantContext
{
    public string ApplicationVersion { get; init; } = string.Empty;

    public string CmRelease { get; init; } = string.Empty;

    public string PrecType { get; init; } = string.Empty;

    public string StructureName { get; init; } = string.Empty;

    public string HeaderFile { get; init; } = string.Empty;

    public int? StructureSourceLine { get; init; }

    public int? CompiledSize { get; init; }

    public int? DumpSize { get; init; }

    public IReadOnlyList<PrecFieldValue> Fields { get; init; } =
        Array.Empty<PrecFieldValue>();

    public IReadOnlyList<string> MappingDetails { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<string> ProposedFixes { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<AssistantEvidence> Evidence { get; init; } =
        Array.Empty<AssistantEvidence>();
}