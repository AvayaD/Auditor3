using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Dispatches structured assistant capability requests to governed providers.
///
/// The dispatcher is responsible for authorization and provider selection.
/// It does not grant unrestricted shell, SSH, CM, SAT, TCM, or repair access.
/// </summary>
public interface IAssistantCapabilityDispatcher
{
    /// <summary>
    /// Evaluates and dispatches one capability request.
    /// </summary>
    Task<AssistantCapabilityResult> DispatchAsync(
        AssistantCapabilityRequest request,
        AssistantExecutionMode executionMode,
        bool targetIsDesignatedLab = false,
        CancellationToken cancellationToken = default);
}