namespace Auditor3;

/// <summary>
/// Describes one field or padding region from a compiled GDB ptype layout.
/// </summary>
public sealed class PrecLayoutField
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int Offset { get; init; }

    public int Size { get; init; }

    public bool IsPadding { get; init; }

    public bool IsBitField { get; init; }

    public int? BitOffset { get; init; }

    public int? BitSize { get; init; }

    public string SourceText { get; init; } = string.Empty;
}