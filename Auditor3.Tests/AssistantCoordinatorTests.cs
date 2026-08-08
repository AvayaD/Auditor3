using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantCoordinatorTests
{
    [Fact]
    public async Task AskAsync_WhenDisabled_DoesNotCallService()
    {
        var service = new RecordingAssistantService();
        var redactor = new RecordingRedactor();

        var coordinator = new AssistantCoordinator(
            service,
            redactor,
            new AssistantSettings
            {
                Enabled = false
            });

        var response = await coordinator.AskAsync(
            "Explain this record.",
            new AssistantContext());

        Assert.False(response.Succeeded);
        Assert.Equal(
            "AI assistant is disabled.",
            response.ErrorMessage);
        Assert.False(string.IsNullOrWhiteSpace(response.CorrelationId));
        Assert.False(service.WasCalled);
        Assert.False(redactor.WasCalled);
    }

    [Fact]
    public async Task AskAsync_WhenEnabled_RedactsAndForwardsRequest()
    {
        var service = new RecordingAssistantService
        {
            Response = new AssistantResponse
            {
                Succeeded = true,
                Answer = "Local explanation",
                CorrelationId = "coordinator-test"
            }
        };

        var redactedContext = new AssistantContext
        {
            PrecType = "REDACTED"
        };

        var redactor = new RecordingRedactor
        {
            Result = redactedContext
        };

        var coordinator = new AssistantCoordinator(
            service,
            redactor,
            new AssistantSettings
            {
                Enabled = true
            });

        var response = await coordinator.AskAsync(
            "  Explain this record.  ",
            new AssistantContext
            {
                PrecType = "PR_EXT"
            },
            systemInstructions: "Use authoritative facts only.",
            correlationId: "coordinator-test");

        Assert.True(response.Succeeded);
        Assert.Equal("Local explanation", response.Answer);

        Assert.True(redactor.WasCalled);
        Assert.True(service.WasCalled);
        Assert.NotNull(service.Request);
        Assert.Equal(
            "Explain this record.",
            service.Request.Question);
        Assert.Equal(
            "Use authoritative facts only.",
            service.Request.SystemInstructions);
        Assert.Equal(
            "coordinator-test",
            service.Request.CorrelationId);
        Assert.Same(
            redactedContext,
            service.Request.Context);
    }

    [Fact]
    public async Task AskAsync_WhenQuestionIsEmpty_ReturnsValidationFailure()
    {
        var service = new RecordingAssistantService();
        var redactor = new RecordingRedactor();

        var coordinator = new AssistantCoordinator(
            service,
            redactor,
            new AssistantSettings
            {
                Enabled = true
            });

        var response = await coordinator.AskAsync(
            " ",
            new AssistantContext());

        Assert.False(response.Succeeded);
        Assert.Equal(
            "Assistant question cannot be empty.",
            response.ErrorMessage);
        Assert.False(service.WasCalled);
        Assert.False(redactor.WasCalled);
    }

    [Fact]
    public async Task AskAsync_WhenContextIsNull_ReturnsValidationFailure()
    {
        var service = new RecordingAssistantService();
        var redactor = new RecordingRedactor();

        var coordinator = new AssistantCoordinator(
            service,
            redactor,
            new AssistantSettings
            {
                Enabled = true
            });

        var response = await coordinator.AskAsync(
            "Explain this record.",
            null!);

        Assert.False(response.Succeeded);
        Assert.Equal(
            "Assistant context cannot be null.",
            response.ErrorMessage);
        Assert.False(service.WasCalled);
        Assert.False(redactor.WasCalled);
    }

    private sealed class RecordingAssistantService : IAssistantService
    {
        public bool WasCalled { get; private set; }

        public AssistantRequest? Request { get; private set; }

        public AssistantResponse Response { get; set; } =
            new AssistantResponse
            {
                Succeeded = true,
                Answer = "Test response"
            };

        public Task<AssistantResponse> AskAsync(
            AssistantRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            Request = request;

            return Task.FromResult(Response);
        }
    }

    private sealed class RecordingRedactor : IAssistantRedactor
    {
        public bool WasCalled { get; private set; }

        public AssistantContext Result { get; set; } =
            new AssistantContext();

        public AssistantContext Redact(AssistantContext context)
        {
            WasCalled = true;
            return Result;
        }
    }
}