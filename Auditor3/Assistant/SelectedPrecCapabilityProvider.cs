using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Provides the engineer-selected PREC context as a governed capability.
///
/// This provider performs no parsing, networking, connection access, or
/// command execution.
/// </summary>
public sealed class SelectedPrecCapabilityProvider
    : IAssistantCapabilityProvider
{
    private readonly AssistantContext _context;

    public SelectedPrecCapabilityProvider(
        AssistantContext context)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
    }

    public bool CanHandle(AssistantCapability capability)
    {
        return capability ==
            AssistantCapability.SelectedPrecContext;
    }

    public Task<AssistantCapabilityResult> HandleAsync(
        AssistantCapabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Task.FromResult(new AssistantCapabilityResult
            {
                Capability =
                    AssistantCapability.SelectedPrecContext,
                Succeeded = false,
                ErrorMessage =
                    "Capability request cannot be null."
            });
        }

        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new AssistantCapabilityResult
        {
            Capability =
                AssistantCapability.SelectedPrecContext,
            Succeeded = true,
            Context = _context,
            Summary =
                "The selected PREC context was supplied successfully.",
            CorrelationId = request.CorrelationId
        });
    }
}