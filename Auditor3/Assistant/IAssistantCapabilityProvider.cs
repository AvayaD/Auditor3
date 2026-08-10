using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Handles one or more governed assistant capabilities.
///
/// Implementations are responsible for capability-specific behavior, while
/// policy validation remains outside this interface. A provider must not
/// execute a request unless it has been authorized by the caller.
///
/// This interface does not grant unrestricted shell, SSH, CM, SAT, TCM, or
/// repair access.
/// </summary>
public interface IAssistantCapabilityProvider
{
    /// <summary>
    /// Indicates whether this provider handles the requested capability.
    /// </summary>
    bool CanHandle(AssistantCapability capability);

    /// <summary>
    /// Handles one already-authorized capability request.
    /// </summary>
    Task<AssistantCapabilityResult> HandleAsync(
        AssistantCapabilityRequest request,
        CancellationToken cancellationToken = default);
}