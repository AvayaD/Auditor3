using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Coordinates read-only assistant requests for the WPF layer.
///
/// This class is intentionally independent of CM, DRCCD, ToolsA, SSH,
/// SFTP, ShellStream, SAT, TCM, and repair execution.
/// </summary>
public sealed class AssistantCoordinator
{
    private readonly IAssistantService _assistantService;
    private readonly IAssistantRedactor _redactor;
    private readonly AssistantSettings _settings;

    public AssistantCoordinator(
        IAssistantService assistantService,
        IAssistantRedactor redactor,
        AssistantSettings settings)
    {
        _assistantService = assistantService ??
            throw new ArgumentNullException(nameof(assistantService));

        _redactor = redactor ??
            throw new ArgumentNullException(nameof(redactor));

        _settings = settings ??
            throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Creates and submits one read-only assistant request.
    /// </summary>
    public async Task<AssistantResponse> AskAsync(
        string question,
        AssistantContext context,
        string systemInstructions = "",
        string correlationId = "",
        CancellationToken cancellationToken = default)
    {
        var requestCorrelationId = string.IsNullOrWhiteSpace(correlationId)
            ? Guid.NewGuid().ToString("N")
            : correlationId;

        if (!_settings.Enabled)
        {
            return new AssistantResponse
            {
                Succeeded = false,
                ErrorMessage = "AI assistant is disabled.",
                CorrelationId = requestCorrelationId
            };
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            return new AssistantResponse
            {
                Succeeded = false,
                ErrorMessage = "Assistant question cannot be empty.",
                CorrelationId = requestCorrelationId
            };
        }

        if (context is null)
        {
            return new AssistantResponse
            {
                Succeeded = false,
                ErrorMessage = "Assistant context cannot be null.",
                CorrelationId = requestCorrelationId
            };
        }

        cancellationToken.ThrowIfCancellationRequested();

        var redactedContext = _redactor.Redact(context);

        var request = new AssistantRequest
        {
            Question = question.Trim(),
            SystemInstructions = systemInstructions ?? string.Empty,
            Context = redactedContext,
            CorrelationId = requestCorrelationId
        };

        return await _assistantService
            .AskAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}