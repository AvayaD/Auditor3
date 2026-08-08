using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Defines the interface for Auditor3 AI assistant implementations.
/// </summary>
public interface IAssistantService
{
    /// <summary>
    /// Sends a question and structured context to the assistant service.
    /// </summary>
    Task<AssistantResponse> AskAsync(
        AssistantRequest request,
        CancellationToken cancellationToken = default);
}