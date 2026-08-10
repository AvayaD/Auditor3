using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Dispatches authorized assistant capability requests to registered providers.
///
/// This dispatcher does not open connections or execute commands itself.
/// </summary>
public sealed class AssistantCapabilityDispatcher
    : IAssistantCapabilityDispatcher
{
    private readonly IReadOnlyList<IAssistantCapabilityProvider> _providers;

    public AssistantCapabilityDispatcher(
        IEnumerable<IAssistantCapabilityProvider> providers)
    {
        _providers = (providers ??
            throw new ArgumentNullException(nameof(providers)))
            .ToArray();
    }

    public async Task<AssistantCapabilityResult> DispatchAsync(
        AssistantCapabilityRequest request,
        AssistantExecutionMode executionMode,
        bool targetIsDesignatedLab = false,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Failure(
                AssistantCapability.SelectedPrecContext,
                "Capability request cannot be null.",
                string.Empty);
        }

        if (!AssistantCapabilityPolicy.IsAllowed(
                executionMode,
                request.Capability,
                request.EngineerApproved,
                targetIsDesignatedLab))
        {
            return Failure(
                request.Capability,
                "Capability request is not allowed in the current " +
                "execution mode.",
                request.CorrelationId);
        }

        var provider = _providers.FirstOrDefault(
            item => item.CanHandle(request.Capability));

        if (provider is null)
        {
            return Failure(
                request.Capability,
                "No provider is registered for the requested capability.",
                request.CorrelationId);
        }

        cancellationToken.ThrowIfCancellationRequested();

        return await provider
            .HandleAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

    private static AssistantCapabilityResult Failure(
        AssistantCapability capability,
        string message,
        string correlationId)
    {
        return new AssistantCapabilityResult
        {
            Capability = capability,
            Succeeded = false,
            ErrorMessage = message,
            CorrelationId = correlationId
        };
    }
}