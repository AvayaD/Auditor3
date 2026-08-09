#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Calls the approved WebAI OpenAI-compatible chat-completions endpoint.
///
/// The API key is read at runtime from the WEBAI_KEY environment variable.
/// This class does not access CM, SAT, TCM, SSH, SFTP, or Fixer.
/// </summary>
public sealed class WebUiAssistantService : IAssistantService
{
    private const string DefaultEndpoint =
        "https://gateway.webai.avaya.com/chat/completions";

    private const string DefaultModel = "claude-sonnet-4-6";

    private readonly HttpClient _httpClient;
    private readonly AssistantSettings _settings;

    public WebUiAssistantService(
        HttpClient httpClient,
        AssistantSettings settings)
    {
        _httpClient = httpClient ??
            throw new ArgumentNullException(nameof(httpClient));

        _settings = settings ??
            throw new ArgumentNullException(nameof(settings));
    }

    public async Task<AssistantResponse> AskAsync(
        AssistantRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Failure(
                "Assistant request cannot be null.",
                string.Empty);
        }

        var apiKey = Environment.GetEnvironmentVariable("WEBAI_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Failure(
                "WEBAI_KEY is not set. The WebAI request was not sent.",
                request.CorrelationId);
        }

        if (!_settings.Enabled)
        {
            return Failure(
                "WebAI assistant is disabled.",
                request.CorrelationId);
        }

        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Failure(
                "Assistant question cannot be empty.",
                request.CorrelationId);
        }

        var endpoint = string.IsNullOrWhiteSpace(
            _settings.ServiceEndpoint)
            ? DefaultEndpoint
            : _settings.ServiceEndpoint;

        var systemInstructions = string.IsNullOrWhiteSpace(
            request.SystemInstructions)
            ? "Explain only the authoritative Auditor3 context supplied."
            : request.SystemInstructions;

        var contextJson = JsonSerializer.Serialize(
            request.Context ?? new AssistantContext(),
            SerializerOptions);

        var contextBytes = Encoding.UTF8.GetByteCount(contextJson);

        if (_settings.MaximumContextBytes > 0 &&
            contextBytes > _settings.MaximumContextBytes)
        {
            return Failure(
                $"Assistant context is too large: {contextBytes} bytes " +
                $" exceeds the configured limit of " +
                $"{_settings.MaximumContextBytes} bytes.",
                request.CorrelationId);
        }

        var userContent = new StringBuilder()
            .AppendLine(request.Question)
            .AppendLine()
            .AppendLine("AUTHORITATIVE AUDITOR3 CONTEXT:")
            .Append(contextJson)
            .ToString();

        var payload = new ChatCompletionRequest
        {
            Model = DefaultModel,
            Messages =
            [
                new ChatMessage
                {
                    Role = "system",
                    Content = systemInstructions
                },
                new ChatMessage
                {
                    Role = "user",
                    Content = userContent
                }
            ]
        };

        var json = JsonSerializer.Serialize(
            payload,
            SerializerOptions);

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint);

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        httpRequest.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Failure(
                    $"WebAI request failed with HTTP " +
                    $"{(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}.",
                    request.CorrelationId);
            }

            var completion = JsonSerializer.Deserialize<
                ChatCompletionResponse>(
                    responseBody,
                    SerializerOptions);

            var answer = completion?
                .Choices?
                .Count > 0
                ? completion.Choices[0].Message?.Content
                : null;

            if (string.IsNullOrWhiteSpace(answer))
            {
                return Failure(
                    "WebAI returned an empty response.",
                    request.CorrelationId);
            }

            return new AssistantResponse
            {
                Succeeded = true,
                Answer = answer,
                CorrelationId = request.CorrelationId,
                ContainsSuggestedCommands =
                    ContainsCommandLikeText(answer),
                Warnings = Array.Empty<string>()
            };
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException error)
        {
            return Failure(
                $"WebAI network request failed: {error.Message}",
                request.CorrelationId);
        }
        catch (JsonException)
        {
            return Failure(
                "WebAI returned an invalid response format.",
                request.CorrelationId);
        }
    }

    private static AssistantResponse Failure(
        string message,
        string correlationId)
    {
        return new AssistantResponse
        {
            Succeeded = false,
            ErrorMessage = message,
            CorrelationId = correlationId,
            ContainsSuggestedCommands = false,
            Warnings = Array.Empty<string>()
        };
    }

    private static bool ContainsCommandLikeText(string answer)
    {
        return answer.Contains("prec ", StringComparison.OrdinalIgnoreCase)
            || answer.Contains("sat ", StringComparison.OrdinalIgnoreCase)
            || answer.Contains("tcm ", StringComparison.OrdinalIgnoreCase);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class ChatCompletionRequest
    {
        public string Model { get; init; } = string.Empty;

        public IReadOnlyList<ChatMessage> Messages { get; init; } =
            Array.Empty<ChatMessage>();
    }

    private sealed class ChatMessage
    {
        public string Role { get; init; } = string.Empty;

        public string Content { get; init; } = string.Empty;
    }

    private sealed class ChatCompletionResponse
    {
        public List<Choice>? Choices { get; init; }
    }

    private sealed class Choice
    {
        public ChatMessage? Message { get; init; }
    }
}