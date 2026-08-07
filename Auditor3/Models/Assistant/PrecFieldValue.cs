namespace Auditor3;

/// <summary>
/// Stores a raw and optionally decoded value for a PREC field.
/// </summary>
public sealed class PrecFieldValue
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int Offset { get; init; }

    public int Size { get; init; }

    public string RawValue { get; init; } = string.Empty;

    public string DecodedValue { get; init; } = string.Empty;

    public string DecodeStatus { get; init; } = "Unknown";

    public bool IsPadding { get; init; }
}