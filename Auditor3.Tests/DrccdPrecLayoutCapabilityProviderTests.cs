using System;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdPrecLayoutCapabilityProviderTests
{
    [Fact]
    public void CanHandle_OnlyPrecLayout()
    {
        var provider = new DrccdPrecLayoutCapabilityProvider(
            new RecordingClient());

        Assert.True(
            provider.CanHandle(
                AssistantCapability.PrecLayout));

        Assert.False(
            provider.CanHandle(
                AssistantCapability.FindPrecsMapping));
    }

    [Fact]
    public async Task HandleAsync_ParsesLayoutAndReturnsEvidence()
    {
        var output = """
            Found release: cm10.2
            Layout file: /home/test/pr_ext.ptype
            (gdb) ptype /o struct pr_ext
            /* offset    |  size */  type = struct pr_ext {
            /*    0      |     2 */    short no_digits;
            /*    2      |     8 */    NYBLE ext[8];
            /* XXX  2-byte hole */
            /*   12      |     4 */    UID p_uid;
            /* total size (bytes):   32 */
            }
            """;

        var client = new RecordingClient
        {
            Output = output
        };

        var provider = new DrccdPrecLayoutCapabilityProvider(client);

        var result = await provider.HandleAsync(
            new AssistantCapabilityRequest
            {
                Capability = AssistantCapability.PrecLayout,
                PrecType = "PR_EXT",
                CorrelationId = "layout-test"
            });

        Assert.True(result.Succeeded);
        Assert.Equal(
            AssistantCapability.PrecLayout,
            result.Capability);
        Assert.Equal(
            "layout-test",
            result.CorrelationId);
        Assert.Same(client, client.LastClient);
        Assert.Equal("PR_EXT", client.LastPrecType);
        Assert.NotNull(result.Context);
        Assert.Equal("PR_EXT", result.Context.PrecType);
        Assert.Equal("pr_ext", result.Context.StructureName);
        Assert.Equal(32, result.Context.CompiledSize);

        Assert.Contains(
            result.Context.Evidence,
            evidence => evidence.Type == "CompiledLayout");

        Assert.Contains(
            result.Context.Evidence,
            evidence => evidence.Type == "PrecStructOutput");
    }

    [Fact]
    public async Task HandleAsync_RequiresPrecType()
    {
        var client = new RecordingClient();
        var provider = new DrccdPrecLayoutCapabilityProvider(client);

        var result = await provider.HandleAsync(
            new AssistantCapabilityRequest
            {
                Capability = AssistantCapability.PrecLayout
            });

        Assert.False(result.Succeeded);
        Assert.Contains(
            "PREC type is required",
            result.ErrorMessage);
        Assert.Null(client.LastPrecType);
    }

    [Fact]
    public async Task HandleAsync_EmptyOutputReturnsFailure()
    {
        var client = new RecordingClient
        {
            Output = string.Empty
        };

        var provider = new DrccdPrecLayoutCapabilityProvider(client);

        var result = await provider.HandleAsync(
            new AssistantCapabilityRequest
            {
                Capability = AssistantCapability.PrecLayout,
                PrecType = "PR_EXT"
            });

        Assert.False(result.Succeeded);
        Assert.Contains(
            "no precstruct output",
            result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_ClientFailureReturnsFailure()
    {
        var client = new RecordingClient
        {
            Error = new InvalidOperationException(
                "DRCCD unavailable")
        };

        var provider = new DrccdPrecLayoutCapabilityProvider(client);

        var result = await provider.HandleAsync(
            new AssistantCapabilityRequest
            {
                Capability = AssistantCapability.PrecLayout,
                PrecType = "PR_EXT",
                CorrelationId = "client-failure-test"
            });

        Assert.False(result.Succeeded);
        Assert.Contains(
            "DRCCD precstruct retrieval failed",
            result.ErrorMessage);
        Assert.Contains(
            "DRCCD unavailable",
            result.ErrorMessage);
        Assert.Equal(
            "client-failure-test",
            result.CorrelationId);
    }

    [Fact]
    public async Task HandleAsync_HonorsCancellation()
    {
        var client = new RecordingClient();
        var provider = new DrccdPrecLayoutCapabilityProvider(client);

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.HandleAsync(
                new AssistantCapabilityRequest
                {
                    Capability = AssistantCapability.PrecLayout,
                    PrecType = "PR_EXT"
                },
                cancellation.Token));

        Assert.Null(client.LastPrecType);
    }

    private sealed class RecordingClient
        : IDrccdPrecStructClient
    {
        public string Output { get; set; } =
            "No layout";

        public Exception? Error { get; set; }

        public string? LastPrecType { get; private set; }

        public RecordingClient LastClient => this;

        public Task<string> GetPrecStructAsync(
            string precType,
            CancellationToken cancellationToken = default)
        {
            LastPrecType = precType;

            if (Error is not null)
            {
                throw Error;
            }

            return Task.FromResult(Output);
        }
    }
}