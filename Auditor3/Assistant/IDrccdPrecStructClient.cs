using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Retrieves read-only PREC structure information from DRCCD.
///
/// Implementations may use the approved DRCCD precstruct workflow, but this
/// interface does not grant arbitrary shell or command access.
/// </summary>
public interface IDrccdPrecStructClient
{
    /// <summary>
    /// Retrieves the source declaration and compiled GDB layout for one PREC.
    /// </summary>
    Task<string> GetPrecStructAsync(
        string precType,
        CancellationToken cancellationToken = default);
}