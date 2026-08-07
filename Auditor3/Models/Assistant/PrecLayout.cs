using System;
using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Represents the compiled memory layout of one PREC structure.
/// </summary>
public sealed class PrecLayout
{
    public string PrecType { get; init; } = string.Empty;

    public string StructureName { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    public string SourceFile { get; init; } = string.Empty;

    public int? TotalSize { get; init; }

    public IReadOnlyList<PrecLayoutField> Fields { get; init; } =
        Array.Empty<PrecLayoutField>();

    public bool HasKnownSize => TotalSize.HasValue;
}