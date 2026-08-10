using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Minimal abstraction over the legacy DRCCD interactive shell.
///
/// Implementations own the shell lifetime. This interface exposes only
/// controlled command writing and prompt-delimited output reading.
/// </summary>
public interface IDrccdShell : IDisposable
{
    /// <summary>
    /// Sends one already-validated command line.
    /// </summary>
    void WriteLine(string command);

    /// <summary>
    /// Reads output until the standard DRCCD prompt is observed.
    /// </summary>
    Task<string> ReadUntilPromptAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default);
}