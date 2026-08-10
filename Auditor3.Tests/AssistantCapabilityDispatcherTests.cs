using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantCapabilityDispatcherTests
{
    [Fact]
    public async Task NullRequest_ReturnsFailure()
    {
        var dispatcher = new AssistantCapabilityDispatcher(
            []);

        var result = await dispatcher.DispatchAsync(
            null!,
            AssistantExecutionMode.OfflineReadOnly);

        Assert.False(result.Succeeded);
        Assert.Contains(
            "cannot be null",
            result.ErrorMessage);
    }

    [Fact]
    public async Task DisallowedCapability_IsNotSentToProvider()
    {
        var provider = new RecordingProvider(
            AssistantCapability.PrecLayout);

        var dispatcher = new AssistantCapabilityDispatcher(
            [provider]);

        var result = await dispatcher.DispatchAsync(
            new AssistantCapabilityRequest
            {
                Capability = AssistantCapability.PrecLayout,
                CorrelationId = "denied-test"
            },
            AssistantExecutionMode.OfflineReadOnly);

        Assert.False(result.Succeeded);
        Assert.False(provider.WasCalled);
        Assert.Equal(
            "denied-test",
            result.CorrelationId);
    }

    [Fact]
    public async Task MissingProvider_ReturnsFailure()
    {
        var dispatcher = new AssistantCapabilityDispatcher(
            []);

        var result = await dispatcher.DispatchAsync(
            new AssistantCapabilityRequest
            {
                Capability =
                    AssistantCapability.SelectedPrecContext,
                CorrelationId = "missing-provider-test"
            },
            AssistantExecutionMode.OfflineReadOnly);

        Assert.False(result.Succeeded);
        Assert.Contains(
            "No provider",
            result.ErrorMessage);
    }

    [Fact]
    public async Task AuthorizedRequest_IsSentToMatchingProvider()
    {
        var provider = new RecordingProvider(
            AssistantCapability.SelectedPrecContext)
        {
            Result = new AssistantCapabilityResult
            {
                Capability =
                    AssistantCapability.SelectedPrecContext,
                Succeeded = true,
                Summary = "Selected context accepted.",
                CorrelationId = "dispatch-test"
            }
        };

        var dispatcher = new AssistantCapabilityDispatcher(
            [provider]);

        var request = new AssistantCapabilityRequest
        {
            Capability = AssistantCapability.SelectedPrecContext,
            CorrelationId = "dispatch-test"
        };

        var result = await dispatcher.DispatchAsync(
            request,
            AssistantExecutionMode.OfflineReadOnly);

        Assert.True(result.Succeeded);
        Assert.True(provider.WasCalled);
        Assert.Same(request, provider.Request);
        Assert.Equal(
            "Selected context accepted.",
            result.Summary);
    }

    [Fact]
    public async Task Cancellation_IsCheckedBeforeProviderCall()
    {
        var provider = new RecordingProvider(
            AssistantCapability.SelectedPrecContext);

        var dispatcher = new AssistantCapabilityDispatcher(
            [provider]);

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => dispatcher.DispatchAsync(
                new AssistantCapabilityRequest
                {
                    Capability =
                        AssistantCapability.SelectedPrecContext
                },
                AssistantExecutionMode.OfflineReadOnly,
                cancellationToken: cancellation.Token));

        Assert.False(provider.WasCalled);
    }

    private sealed class RecordingProvider
        : IAssistantCapabilityProvider
    {
        private readonly AssistantCapability _capability;

        public RecordingProvider(
            AssistantCapability capability)
        {
            _capability = capability;
        }

        public bool WasCalled { get; private set; }

        public AssistantCapabilityRequest? Request { get; private set; }

        public AssistantCapabilityResult Result { get; set; } =
            new AssistantCapabilityResult
            {
                Succeeded = true
            };

        public bool CanHandle(
            AssistantCapability capability)
        {
            return capability == _capability;
        }

        public Task<AssistantCapabilityResult> HandleAsync(
            AssistantCapabilityRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            Request = request;

            return Task.FromResult(Result);
        }
    }
}