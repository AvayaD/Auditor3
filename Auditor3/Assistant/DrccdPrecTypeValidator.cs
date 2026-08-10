#nullable enable
using System.Text.RegularExpressions;

namespace Auditor3;

/// <summary>
/// Validates PREC names before they are passed to the approved DRCCD
/// precstruct workflow.
/// </summary>
public static class DrccdPrecTypeValidator
{
    private static readonly Regex PrecNameRegex = new(
        @"^[A-Za-z_][A-Za-z0-9_]*$",
        RegexOptions.Compiled);

    /// <summary>
    /// Returns a normalized lowercase PREC name when valid.
    /// </summary>
    public static bool TryNormalize(
        string? value,
        out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var candidate = value.Trim();

        if (!PrecNameRegex.IsMatch(candidate))
        {
            return false;
        }

        normalized = candidate.ToLowerInvariant();
        return true;
    }
}