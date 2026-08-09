using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class WebUiAssistantServiceTests
{
    [Fact]
    public async Task AskAsync_WhenApiKeyIsMissing_DoesNotSendRequest()
    {
        var originalKey =
            Environment.GetEnvironmentVariable("WEBAI_KEY");

        try
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                null);

            var handler = new RecordingHandler();
            using var client = new HttpClient(handler);

            var service = new WebUiAssistantService(
                client,
                CreateEnabledSettings());

            var response = await service.AskAsync(
                CreateRequest());

            Assert.False(response.Succeeded);
            Assert.Contains(
                "WEBAI_KEY is not set",
                response.ErrorMessage);
            Assert.False(handler.RequestWasSent);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                originalKey);
        }
    }

    [Fact]
    public async Task AskAsync_WhenServiceIsDisabled_DoesNotSendRequest()
    {
        var originalKey =
            Environment.GetEnvironmentVariable("WEBAI_KEY");

        try
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                "test-key-not-a-real-key");

            var handler = new RecordingHandler();
            using var client = new HttpClient(handler);

            var settings = new AssistantSettings
            {
                Enabled = false,
                ServiceEndpoint =
                    "https://gateway.webai.avaya.com/chat/completions"
            };

            var service = new WebUiAssistantService(
                client,
                settings);

            var response = await service.AskAsync(
                CreateRequest());

            Assert.False(response.Succeeded);
            Assert.Equal(
                "WebAI assistant is disabled.",
                response.ErrorMessage);
            Assert.False(handler.RequestWasSent);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                originalKey);
        }
    }

    [Fact]
    public async Task AskAsync_SuccessfulResponse_ReturnsAnswer()
    {
        var originalKey =
            Environment.GetEnvironmentVariable("WEBAI_KEY");

        try
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                "test-key-not-a-real-key");

            var handler = new RecordingHandler
            {
                Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """
                        {
                          "choices": [
                            {
                              "message": {
                                "role": "assistant",
                                "content": "The record size matches the compiled layout."
                              }
                            }
                          ]
                        }
                        """)
                }
            };

            using var client = new HttpClient(handler);

            var service = new WebUiAssistantService(
                client,
                CreateEnabledSettings());

            var response = await service.AskAsync(
                CreateRequest());

            Assert.True(response.Succeeded);
            Assert.Equal(
                "The record size matches the compiled layout.",
                response.Answer);
            Assert.Equal(
                "test-correlation-id",
                response.CorrelationId);
            Assert.False(response.ContainsSuggestedCommands);

            Assert.True(handler.RequestWasSent);
            Assert.Equal(
                HttpMethod.Post,
                handler.RequestMethod);
            Assert.Equal(
                "Bearer",
                handler.AuthorizationScheme);
            Assert.Equal(
                "test-key-not-a-real-key",
                handler.AuthorizationParameter);

            Assert.Contains(
                "\"model\":\"claude-sonnet-4-6\"",
                handler.RequestBody);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                originalKey);
        }
    }

    [Fact]
    public async Task AskAsync_WhenContextIsTooLarge_DoesNotSendRequest()
    {
        var originalKey =
            Environment.GetEnvironmentVariable("WEBAI_KEY");

        try
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                "test-key-not-a-real-key");

            var handler = new RecordingHandler();
            using var client = new HttpClient(handler);

            var settings = new AssistantSettings
            {
                Enabled = true,
                MaximumContextBytes = 10
            };

            var service = new WebUiAssistantService(
                client,
                settings);

            var response = await service.AskAsync(
                new AssistantRequest
                {
                    Question = "Explain this record.",
                    CorrelationId = "oversized-context-test",
                    Context = new AssistantContext
                    {
                        PrecType = "PR_EXT",
                        Evidence =
                        [
                            new AssistantEvidence
                            {
                                Type = "Test",
                                Content =
                                    "This context is intentionally too large."
                            }
                        ]
                    }
                });

            Assert.False(response.Succeeded);
            Assert.Contains(
                "context is too large",
                response.ErrorMessage,
                StringComparison.OrdinalIgnoreCase);
            Assert.Equal(
                "oversized-context-test",
                response.CorrelationId);
            Assert.False(handler.RequestWasSent);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                originalKey);
        }
    }

    [Fact]
    public async Task AskAsync_HttpFailure_ReturnsFailureWithoutThrowing()
    {
        var originalKey =
            Environment.GetEnvironmentVariable("WEBAI_KEY");

        try
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                "test-key-not-a-real-key");

            var handler = new RecordingHandler
            {
                Response = new HttpResponseMessage(
                    HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Unauthorized"
                }
            };

            using var client = new HttpClient(handler);

            var service = new WebUiAssistantService(
                client,
                CreateEnabledSettings());

            var response = await service.AskAsync(
                CreateRequest());

            Assert.False(response.Succeeded);
            Assert.Contains(
                "HTTP 401",
                response.ErrorMessage);
            Assert.Equal(
                "test-correlation-id",
                response.CorrelationId);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                "WEBAI_KEY",
                originalKey);
        }
    }

    private static AssistantSettings CreateEnabledSettings()
    {
        return new AssistantSettings
        {
            Enabled = true,
            ServiceEndpoint =
                "https://gateway.webai.avaya.com/chat/completions",
            TimeoutSeconds = 30
        };
    }

    private static AssistantRequest CreateRequest()
    {
        return new AssistantRequest
        {
            Question = "Explain this test record.",
            SystemInstructions =
                "Use only the supplied authoritative facts.",
            CorrelationId = "test-correlation-id",
            Context = new AssistantContext
            {
                PrecType = "PR_EXT",
                StructureName = "pr_ext",
                CmRelease = "cm10.2",
                CompiledSize = 32,
                DumpSize = 32,
                RecordSizeStatus = "Match"
            }
        };
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } =
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "choices": [
                        {
                          "message": {
                            "content": "Default test response."
                          }
                        }
                      ]
                    }
                    """)
            };

        public bool RequestWasSent { get; private set; }

        public HttpMethod? RequestMethod { get; private set; }

        public string AuthorizationScheme { get; private set; } =
            string.Empty;

        public string AuthorizationParameter { get; private set; } =
            string.Empty;

        public string RequestBody { get; private set; } =
            string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestWasSent = true;
            RequestMethod = request.Method;

            if (request.Headers.Authorization is not null)
            {
                AuthorizationScheme =
                    request.Headers.Authorization.Scheme;

                AuthorizationParameter =
                    request.Headers.Authorization.Parameter ??
                    string.Empty;
            }

            RequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(
                    cancellationToken);

            return Response;
        }
    }
}