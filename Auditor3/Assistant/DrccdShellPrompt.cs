using System.Text.RegularExpressions;

namespace Auditor3;

/// <summary>
/// Identifies the legacy DRCCD interactive shell prompt.
/// </summary>
public static class DrccdShellPrompt
{
    public static readonly Regex Pattern = new(
        @"drccd\s+\[[0-9]+\]->",
        RegexOptions.Compiled);
}