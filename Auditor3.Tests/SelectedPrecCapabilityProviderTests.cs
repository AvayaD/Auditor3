using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class SelectedPrecCapabilityProviderTests
{
    [Fact]
    public void CanHandle_OnlySelectedPrecContext()
    {
        var provider = new SelectedPrecCapabilityProvider(
            new AssistantContext());

        Assert.True(
            provider.CanHandle(
                AssistantCapability.SelectedPrecContext));

        Assert.False(
            provider.CanHandle(
                AssistantCapability.PrecLayout));
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuppliedContext()
    {
        var context = new AssistantContext
        {
            PrecType = "PR_EXT",
            RawPrec = "PR_EXT test-data"
        };

        var provider = new SelectedPrecCapabilityProvider(context);

        var request = new AssistantCapabilityRequest
        {
            Capability =
                AssistantCapability.SelectedPrecContext,
            CorrelationId = "selected-prec-test"
        };

        var result = await provider.HandleAsync(request);

        Assert.True(result.Succeeded);
        Assert.Same(context, result.Context);
        Assert.Equal(
            AssistantCapability.SelectedPrecContext,
            result.Capability);
        Assert.Equal(
            "selected-prec-test",
            result.CorrelationId);
        Assert.Contains(
            "supplied successfully",
            result.Summary);
    }

    [Fact]
    public async Task HandleAsync_NullRequest_ReturnsFailure()
    {
        var provider = new SelectedPrecCapabilityProvider(
            new AssistantContext());

        var result = await provider.HandleAsync(null!);

        Assert.False(result.Succeeded);
        Assert.Contains(
            "cannot be null",
            result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_CancellationIsHonored()
    {
        var provider = new SelectedPrecCapabilityProvider(
            new AssistantContext());

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.HandleAsync(
                new AssistantCapabilityRequest
                {
                    Capability =
                        AssistantCapability.SelectedPrecContext
                },
                cancellation.Token));
    }
}