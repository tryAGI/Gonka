using Meai = Microsoft.Extensions.AI;

namespace Gonka.IntegrationTests;

[TestClass]
public partial class Tests
{
    private const string TestPrivateKey = "0000000000000000000000000000000000000000000000000000000000000001";
    private const string TestRequesterAddress = "gonka1requesteraddress";
    private const string TestProviderAddress = "gonka1provideraddress";

    private static async Task<GonkaClient> GetAuthenticatedClientAsync()
    {
        if (Environment.GetEnvironmentVariable(GonkaDefaults.PrivateKeyEnvironmentVariable) is not { Length: > 0 })
        {
            throw new AssertInconclusiveException($"{GonkaDefaults.PrivateKeyEnvironmentVariable} environment variable is not found.");
        }

        if (Environment.GetEnvironmentVariable(GonkaDefaults.EndpointsEnvironmentVariable) is not { Length: > 0 } &&
            Environment.GetEnvironmentVariable(GonkaDefaults.SourceUrlEnvironmentVariable) is not { Length: > 0 })
        {
            throw new AssertInconclusiveException($"{GonkaDefaults.EndpointsEnvironmentVariable} or {GonkaDefaults.SourceUrlEnvironmentVariable} environment variable is not found.");
        }

        return await GonkaClient.CreateFromEnvironmentAsync().ConfigureAwait(false);
    }

    [TestMethod]
    public void EndpointParse_AddsV1Suffix()
    {
        var endpoint = GonkaEndpoint.Parse("https://provider.example.com;gonka1provider");
        var constructed = new GonkaEndpoint("https://provider.example.com", "gonka1provider");

        endpoint.Url.Should().Be(new Uri("https://provider.example.com/v1"));
        endpoint.Address.Should().Be("gonka1provider");
        constructed.Url.Should().Be(new Uri("https://provider.example.com/v1"));
        constructed.Address.Should().Be("gonka1provider");
    }

    [TestMethod]
    public async Task CreateAsync_SourceUrl_FiltersAndUsesDelegateEndpoint()
    {
        var discoveryHandler = new DiscoveryHandler(request =>
        {
            return request.RequestUri?.AbsolutePath switch
            {
                "/v1/epochs/current/participants" => JsonResponse(
                    """
                    {
                      "active_participants": {
                        "participants": [
                          {
                            "inference_url": "https://provider.example.com/provider-a",
                            "index": "gonka1allowed"
                          },
                          {
                            "inference_url": "https://provider.example.com/provider-b",
                            "index": "gonka1blocked"
                          }
                        ]
                      },
                      "excluded_participants": []
                    }
                    """),
                "/chain-api/productscience/inference/inference/params" => JsonResponse(
                    """
                    {
                      "params": {
                        "transfer_agent_access_params": {
                          "allowed_transfer_addresses": [
                            "gonka1allowed"
                          ]
                        }
                      }
                    }
                    """),
                "/provider-a/v1/identity" => JsonResponse(
                    """
                    {
                      "data": {
                        "delegate_ta": {
                          "https://delegate.example.com": "gonka1delegate"
                        }
                      }
                    }
                    """),
                _ => new HttpResponseMessage(System.Net.HttpStatusCode.NotFound),
            };
        });

        using var client = await GonkaClient.CreateAsync(
            new GonkaClientOptions
            {
                PrivateKey = TestPrivateKey,
                SourceUrl = new Uri("https://source.example.com"),
                EndpointSelector = endpoints => endpoints[0],
                DiscoveryHttpMessageHandler = discoveryHandler,
                HttpMessageHandler = new CaptureHandler(),
            }).ConfigureAwait(false);

        client.Endpoint.Should().NotBeNull();
        client.Endpoint!.Url.Should().Be(new Uri("https://delegate.example.com/v1"));
        client.Endpoint.Address.Should().Be("gonka1allowed");
        discoveryHandler.RequestUris.Should().Equal(
            new Uri("https://source.example.com/v1/epochs/current/participants"),
            new Uri("https://source.example.com/chain-api/productscience/inference/inference/params"),
            new Uri("https://provider.example.com/provider-a/v1/identity"));
    }

    [TestMethod]
    public async Task CreateAsync_ExplicitEndpoints_SkipsDiscovery()
    {
        var discoveryHandler = new DiscoveryHandler(_ => throw new InvalidOperationException("Discovery should not be called."));

        using var client = await GonkaClient.CreateAsync(
            new GonkaClientOptions
            {
                PrivateKey = TestPrivateKey,
                Endpoints =
                [
                    new GonkaEndpoint("https://explicit.example.com", "gonka1explicit"),
                ],
                DiscoveryHttpMessageHandler = discoveryHandler,
                HttpMessageHandler = new CaptureHandler(),
            }).ConfigureAwait(false);

        client.Endpoint.Should().NotBeNull();
        client.Endpoint!.Url.Should().Be(new Uri("https://explicit.example.com/v1"));
        client.Endpoint.Address.Should().Be("gonka1explicit");
        discoveryHandler.RequestUris.Should().BeEmpty();
    }

    [TestMethod]
    public void Cryptography_DerivesExpectedAddress()
    {
        var address = GonkaCryptography.DeriveAddress(TestPrivateKey);

        address.Should().Be("gonka1w508d6qejxtdg4y5r3zarvary0c5xw7k2gsyg6");
    }

    [TestMethod]
    public void Cryptography_SignsPayloadDeterministically()
    {
        var signature = GonkaCryptography.SignPayload(
            "{}"u8.ToArray(),
            TestPrivateKey,
            1_234_567_890,
            TestProviderAddress);

        signature.Should().Be("EPwEwDfixIpikezhf1lSiUxn+bIsNaRvq9VO+UxUAYwizk0tef663NLzR4jeCbz+Vg4wHMNvrVWe7u0pRhVNlg==");
        Convert.FromBase64String(signature).Should().HaveCount(64);
    }

    [TestMethod]
    public async Task CreateChatCompletionAsync_SignsOutgoingRequest()
    {
        var handler = new CaptureHandler();
        var endpoint = new GonkaEndpoint("https://provider.example.com/v1", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);

        var response = await client.CreateChatCompletionAsync(
            new CreateChatCompletionRequest
            {
                Model = "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8",
                Messages =
                [
                    new ChatCompletionMessage
                    {
                        Role = ChatCompletionMessageRole.User,
                        Content = "Hello, Gonka!",
                    },
                ],
            }).ConfigureAwait(false);

        response.Choices.Should().ContainSingle();
        handler.RequestUri.Should().Be(new Uri("https://provider.example.com/v1/chat/completions"));
        handler.Body.Should().Contain("\"model\"");
        handler.Body.Should().Contain("Hello, Gonka!");
        handler.Headers.Should().ContainKey("Authorization");
        handler.Headers["Authorization"].Should().ContainSingle();
        Convert.FromBase64String(handler.Headers["Authorization"][0]).Should().HaveCount(64);
        handler.Headers[GonkaDefaults.RequesterAddressHeader].Should().ContainSingle(TestRequesterAddress);
        handler.Headers[GonkaDefaults.TimestampHeader].Should().ContainSingle();
    }

    [TestMethod]
    public async Task CreateChatCompletionStreamingAsync_ReadsServerSentEvents()
    {
        var handler = new CaptureHandler(
            """
            data: {"id":"chunk-1","choices":[{"index":0,"delta":{"role":"assistant","content":"Hello"}}]}

            data: [DONE]

            """,
            "text/event-stream");
        var endpoint = new GonkaEndpoint("https://provider.example.com", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);

        var chunks = new List<CreateChatCompletionResponse>();
        await foreach (var chunk in client.CreateChatCompletionStreamingAsync(
            new CreateChatCompletionRequest
            {
                Model = "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8",
                Messages =
                [
                    new ChatCompletionMessage
                    {
                        Role = ChatCompletionMessageRole.User,
                        Content = "Hello, Gonka!",
                    },
                ],
            }))
        {
            chunks.Add(chunk);
        }

        chunks.Should().ContainSingle();
        chunks[0].Choices.Should().ContainSingle();
        chunks[0].Choices[0].Delta.Should().NotBeNull();
        chunks[0].Choices[0].Delta!.Content!.Value.Value1.Should().Be("Hello");
        handler.RequestUri.Should().Be(new Uri("https://provider.example.com/v1/chat/completions"));
        handler.Body.Should().Contain("\"stream\":true");
        handler.Headers.Should().ContainKey("Authorization");
        Convert.FromBase64String(handler.Headers["Authorization"][0]).Should().HaveCount(64);
    }

    [TestMethod]
    public async Task CreateChatCompletionStreamingAsync_ReadsRawJsonLinesWithoutDeltaRole()
    {
        var handler = new CaptureHandler(
            """
            {"id":"chunk-1","model":"test-model","choices":[{"index":0,"delta":{"content":"Hello"}}]}
            {"id":"chunk-2","model":"test-model","choices":[{"index":0,"delta":{"content":" there"},"finish_reason":"stop"}]}
            """,
            "application/x-ndjson");
        var endpoint = new GonkaEndpoint("https://provider.example.com", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);

        var chunks = new List<CreateChatCompletionResponse>();
        await foreach (var chunk in client.CreateChatCompletionStreamingAsync(
            new CreateChatCompletionRequest
            {
                Model = "test-model",
                Messages =
                [
                    new ChatCompletionMessage
                    {
                        Role = ChatCompletionMessageRole.User,
                        Content = "Hello",
                    },
                ],
            }))
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(2);
        chunks.Select(chunk => chunk.Choices[0].Delta!.Content!.Value.Value1).Should().Equal("Hello", " there");
        chunks[1].Choices[0].FinishReason.Should().Be("stop");
    }

    [TestMethod]
    public async Task CreateChatCompletionStreamingAsync_ThrowsApiExceptionForErrorResponse()
    {
        var handler = new CaptureHandler(
            """
            {
              "error": {
                "message": "invalid request",
                "type": "invalid_request_error"
              }
            }
            """,
            statusCode: System.Net.HttpStatusCode.BadRequest);
        var endpoint = new GonkaEndpoint("https://provider.example.com", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);

        Func<Task> act = async () =>
        {
            await foreach (var _ in client.CreateChatCompletionStreamingAsync(
                new CreateChatCompletionRequest
                {
                    Model = "test-model",
                    Messages =
                    [
                        new ChatCompletionMessage
                        {
                            Role = ChatCompletionMessageRole.User,
                            Content = "Hello",
                        },
                    ],
                }))
            {
            }
        };

        var exception = await act.Should().ThrowAsync<ApiException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.Which.ResponseBody.Should().Contain("invalid request");
        exception.Which.ResponseObject?.Error?.Message.Should().Be("invalid request");
    }

    [TestMethod]
    public async Task ChatClient_GetResponseAsync_MapsTextRequestAndResponse()
    {
        var handler = new CaptureHandler();
        var endpoint = new GonkaEndpoint("https://provider.example.com", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);
        Meai.IChatClient chatClient = client;

        var response = await chatClient.GetResponseAsync(
            [new Meai.ChatMessage(Meai.ChatRole.User, "Hello, Gonka!")],
            new Meai.ChatOptions
            {
                ModelId = "test-model",
                Temperature = 0.2f,
                MaxOutputTokens = 32,
            }).ConfigureAwait(false);

        response.Text.Should().Be("ok");
        handler.Body.Should().Contain("\"model\":\"test-model\"");
        handler.Body.Should().Contain("\"temperature\":0.2");
        handler.Body.Should().Contain("\"max_tokens\":32");
        handler.Body.Should().Contain("Hello, Gonka!");
    }

    [TestMethod]
    public async Task ChatClient_GetStreamingResponseAsync_MapsTextUpdates()
    {
        var handler = new CaptureHandler(
            """
            data: {"id":"chunk-1","model":"test-model","choices":[{"index":0,"delta":{"role":"assistant","content":"Hello"}}]}

            data: {"id":"chunk-2","model":"test-model","choices":[{"index":0,"delta":{"content":" Gonka"},"finish_reason":"stop"}]}

            data: [DONE]

            """,
            "text/event-stream");
        var endpoint = new GonkaEndpoint("https://provider.example.com", TestProviderAddress);

        using var client = new GonkaClient(
            TestPrivateKey,
            endpoint,
            TestRequesterAddress,
            handler);
        Meai.IChatClient chatClient = client;

        var text = new System.Text.StringBuilder();
        var finishReasons = new List<Meai.ChatFinishReason?>();
        await foreach (var update in chatClient.GetStreamingResponseAsync(
            [new Meai.ChatMessage(Meai.ChatRole.User, "Hello")],
            new Meai.ChatOptions { ModelId = "test-model" }))
        {
            text.Append(update.Text);
            finishReasons.Add(update.FinishReason);
        }

        text.ToString().Should().Be("Hello Gonka");
        finishReasons.Should().Contain(Meai.ChatFinishReason.Stop);
        handler.Body.Should().Contain("\"stream\":true");
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        private const string DefaultResponseBody =
            """
            {
              "choices": [
                {
                  "message": {
                    "role": "assistant",
                    "content": "ok"
                  }
                }
              ]
            }
            """;

        private readonly string _responseBody;
        private readonly string _mediaType;
        private readonly System.Net.HttpStatusCode _statusCode;

        public CaptureHandler(
            string? responseBody = null,
            string mediaType = "application/json",
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK)
        {
            _responseBody = responseBody ?? DefaultResponseBody;
            _mediaType = mediaType;
            _statusCode = statusCode;
        }

        public Uri? RequestUri { get; private set; }

        public string Body { get; private set; } = string.Empty;

        public Dictionary<string, string[]> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            Body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            foreach (var header in request.Headers)
            {
                Headers[header.Key] = header.Value.ToArray();
            }

#pragma warning disable CA2000
            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(
                    _responseBody,
                    System.Text.Encoding.UTF8,
                    _mediaType),
            };
#pragma warning restore CA2000
        }
    }

    private sealed class DiscoveryHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public List<Uri> RequestUris { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.RequestUri is not null)
            {
                RequestUris.Add(request.RequestUri);
            }

            return Task.FromResult(responder(request));
        }
    }

    private static HttpResponseMessage JsonResponse(string content)
    {
#pragma warning disable CA2000
        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(
                content,
                System.Text.Encoding.UTF8,
                "application/json"),
        };
#pragma warning restore CA2000
    }
}
