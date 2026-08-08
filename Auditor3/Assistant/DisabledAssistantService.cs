using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Assistant implementation used when AI assistance is disabled.
/// It never performs network operations.
/// </summary>
public sealed class DisabledAssistantService : IAssistantService
{
    public Task<AssistantResponse> AskAsync(
        AssistantRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new AssistantResponse
        {
            Succeeded = false,
            ErrorMessage = "AI assistant is disabled.",
            CorrelationId = request?.CorrelationId ?? string.Empty
        };

        return Task.FromResult(response);
    }
}