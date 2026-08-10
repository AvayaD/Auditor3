using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Creates a governed DRCCD shell session.
///
/// Implementations own the connection-specific details. Callers must not
/// receive unrestricted shell or SSH access.
/// </summary>
public interface IDrccdShellFactory
{
    /// <summary>
    /// Opens a DRCCD shell for an approved read-only analysis operation.
    /// </summary>
    Task<IDrccdShell> CreateAsync(
        CancellationToken cancellationToken = default);
}