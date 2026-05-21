using System.Runtime.CompilerServices;
using System.Text;

namespace Gonka;

public sealed partial class GonkaClient
{
    /// <summary>
    /// Creates a streaming chat completion and yields each server-sent data chunk.
    /// </summary>
    public async IAsyncEnumerable<CreateChatCompletionResponse> CreateChatCompletionStreamingAsync(
        CreateChatCompletionRequest request,
        AutoSDKRequestOptions? requestOptions = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var streamingRequest = CreateStreamingRequest(request);

        PrepareArguments(HttpClient);
        PrepareCreateChatCompletionArguments(HttpClient, streamingRequest);

        using var timeoutCancellationTokenSource = AutoSDKRequestOptionsSupport.CreateTimeoutCancellationTokenSource(
            Options,
            requestOptions,
            cancellationToken);
        var effectiveCancellationToken = timeoutCancellationTokenSource?.Token ?? cancellationToken;

        using var httpRequest = CreateStreamingHttpRequest(streamingRequest, requestOptions);

        await AutoSDKRequestOptionsSupport.OnBeforeRequestAsync(
            Options,
            AutoSDKRequestOptionsSupport.CreateHookContext(
                operationId: "CreateChatCompletion",
                methodName: "CreateChatCompletionStreamingAsync",
                pathTemplate: "\"/chat/completions\"",
                httpMethod: "POST",
                baseUri: BaseUri,
                request: httpRequest,
                response: null,
                exception: null,
                clientOptions: Options,
                requestOptions: requestOptions,
                attempt: 1,
                maxAttempts: 1,
                willRetry: false,
                retryDelay: null,
                retryReason: string.Empty,
                cancellationToken: effectiveCancellationToken)).ConfigureAwait(false);

        HttpResponseMessage? response = null;
        try
        {
            response = await HttpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                effectiveCancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            await AutoSDKRequestOptionsSupport.OnAfterErrorAsync(
                Options,
                AutoSDKRequestOptionsSupport.CreateHookContext(
                    operationId: "CreateChatCompletion",
                    methodName: "CreateChatCompletionStreamingAsync",
                    pathTemplate: "\"/chat/completions\"",
                    httpMethod: "POST",
                    baseUri: BaseUri,
                    request: httpRequest,
                    response: null,
                    exception: exception,
                    clientOptions: Options,
                    requestOptions: requestOptions,
                    attempt: 1,
                    maxAttempts: 1,
                    willRetry: false,
                    retryDelay: null,
                    retryReason: string.Empty,
                    cancellationToken: effectiveCancellationToken)).ConfigureAwait(false);
            throw;
        }

        using (response)
        {
            ProcessResponse(HttpClient, response);
            ProcessCreateChatCompletionResponse(HttpClient, response);

            if (!response.IsSuccessStatusCode)
            {
                await AutoSDKRequestOptionsSupport.OnAfterErrorAsync(
                    Options,
                    AutoSDKRequestOptionsSupport.CreateHookContext(
                        operationId: "CreateChatCompletion",
                        methodName: "CreateChatCompletionStreamingAsync",
                        pathTemplate: "\"/chat/completions\"",
                        httpMethod: "POST",
                        baseUri: BaseUri,
                        request: httpRequest,
                        response: response,
                        exception: null,
                        clientOptions: Options,
                        requestOptions: requestOptions,
                        attempt: 1,
                        maxAttempts: 1,
                        willRetry: false,
                        retryDelay: null,
                        retryReason: string.Empty,
                        cancellationToken: effectiveCancellationToken)).ConfigureAwait(false);

                await ThrowStreamingErrorAsync(response, effectiveCancellationToken).ConfigureAwait(false);
            }

            await AutoSDKRequestOptionsSupport.OnAfterSuccessAsync(
                Options,
                AutoSDKRequestOptionsSupport.CreateHookContext(
                    operationId: "CreateChatCompletion",
                    methodName: "CreateChatCompletionStreamingAsync",
                    pathTemplate: "\"/chat/completions\"",
                    httpMethod: "POST",
                    baseUri: BaseUri,
                    request: httpRequest,
                    response: response,
                    exception: null,
                    clientOptions: Options,
                    requestOptions: requestOptions,
                    attempt: 1,
                    maxAttempts: 1,
                    willRetry: false,
                    retryDelay: null,
                    retryReason: string.Empty,
                    cancellationToken: effectiveCancellationToken)).ConfigureAwait(false);

            await foreach (var chunk in ReadStreamingResponseAsync(response, effectiveCancellationToken).ConfigureAwait(false))
            {
                yield return chunk;
            }
        }
    }

    /// <summary>
    /// Creates a streaming chat completion and yields each server-sent data chunk.
    /// </summary>
    public IAsyncEnumerable<CreateChatCompletionResponse> CreateChatCompletionStreamingAsync(
        string model,
        IList<ChatCompletionMessage> messages,
        double? temperature = default,
        double? topP = default,
        int? maxTokens = default,
        OneOf<string, IList<string>>? stop = default,
        IList<ChatCompletionTool>? tools = default,
        OneOf<string, ChatCompletionToolChoice>? toolChoice = default,
        ResponseFormat? responseFormat = default,
        AutoSDKRequestOptions? requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateChatCompletionRequest
        {
            Model = model,
            Messages = messages,
            Temperature = temperature,
            TopP = topP,
            MaxTokens = maxTokens,
            Stream = true,
            Stop = stop,
            Tools = tools,
            ToolChoice = toolChoice,
            ResponseFormat = responseFormat,
        };

        return CreateChatCompletionStreamingAsync(request, requestOptions, cancellationToken);
    }

    private HttpRequestMessage CreateStreamingHttpRequest(
        CreateChatCompletionRequest request,
        AutoSDKRequestOptions? requestOptions)
    {
        var pathBuilder = new PathBuilder("/chat/completions", HttpClient.BaseAddress);
        var path = AutoSDKRequestOptionsSupport.AppendQueryParameters(
            pathBuilder.ToString(),
            Options.QueryParameters,
            requestOptions?.QueryParameters);
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(path, UriKind.RelativeOrAbsolute));

#if NET6_0_OR_GREATER
        httpRequest.Version = System.Net.HttpVersion.Version11;
        httpRequest.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
#endif

        httpRequest.Content = new StringContent(
            request.ToJson(JsonSerializerContext),
            Encoding.UTF8,
            "application/json");

        AutoSDKRequestOptionsSupport.ApplyHeaders(
            httpRequest,
            Options.Headers,
            requestOptions?.Headers);

        PrepareRequest(HttpClient, httpRequest);
        PrepareCreateChatCompletionRequest(HttpClient, httpRequest, request);

        return httpRequest;
    }

    private static CreateChatCompletionRequest CreateStreamingRequest(CreateChatCompletionRequest request)
    {
        return new CreateChatCompletionRequest
        {
            Model = request.Model,
            Messages = request.Messages,
            Temperature = request.Temperature,
            TopP = request.TopP,
            MaxTokens = request.MaxTokens,
            Stream = true,
            Stop = request.Stop,
            Tools = request.Tools,
            ToolChoice = request.ToolChoice,
            ResponseFormat = request.ResponseFormat,
            AdditionalProperties = request.AdditionalProperties is { Count: > 0 }
                ? new Dictionary<string, object>(request.AdditionalProperties)
                : new Dictionary<string, object>(),
        };
    }

    private async IAsyncEnumerable<CreateChatCompletionResponse> ReadStreamingResponseAsync(
        HttpResponseMessage response,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        var eventData = new StringBuilder();

        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                var payload = eventData.ToString().Trim();
                eventData.Clear();

                if (payload.Length == 0)
                {
                    continue;
                }

                if (IsDonePayload(payload))
                {
                    yield break;
                }

                yield return DeserializeStreamingPayload(payload, response);
                continue;
            }

            if (line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                eventData.AppendLine(line["data:".Length..].TrimStart());
                continue;
            }

            if (line.StartsWith("event:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("id:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("retry:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith(':'))
            {
                continue;
            }

            if (eventData.Length > 0)
            {
                eventData.AppendLine(line);
                continue;
            }

            if (IsDonePayload(trimmed))
            {
                yield break;
            }

            yield return DeserializeStreamingPayload(trimmed, response);
        }

        var bufferedPayload = eventData.ToString().Trim();
        if (bufferedPayload.Length == 0 || IsDonePayload(bufferedPayload))
        {
            yield break;
        }

        yield return DeserializeStreamingPayload(bufferedPayload, response);
    }

    private CreateChatCompletionResponse DeserializeStreamingPayload(
        string payload,
        HttpResponseMessage response)
    {
        try
        {
            return CreateChatCompletionResponse.FromJson(payload, JsonSerializerContext) ??
                   throw new InvalidOperationException("Streaming response deserialization failed.");
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.Text.Json.JsonException)
        {
            throw new ApiException(payload, exception, response.StatusCode)
            {
                ResponseBody = payload,
                ResponseHeaders = response.Headers.ToDictionary(
                    header => header.Key,
                    header => header.Value),
            };
        }
    }

    private static bool IsDonePayload(string payload)
    {
        return string.Equals(payload.Trim(), "[DONE]", StringComparison.Ordinal);
    }

    private async Task ThrowStreamingErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string? content = null;
        Exception? exception = null;
        ErrorResponse? error = null;

        try
        {
            content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            error = ErrorResponse.FromJson(content, JsonSerializerContext);
        }
        catch (System.Text.Json.JsonException ex)
        {
            exception = ex;
        }
        catch (NotSupportedException ex)
        {
            exception = ex;
        }
        catch (InvalidOperationException ex)
        {
            exception = ex;
        }
        catch (HttpRequestException ex)
        {
            exception = ex;
        }
        catch (IOException ex)
        {
            exception = ex;
        }

        throw new ApiException<ErrorResponse>(
            content ?? response.ReasonPhrase ?? string.Empty,
            exception,
            response.StatusCode)
        {
            ResponseBody = content,
            ResponseObject = error,
            ResponseHeaders = response.Headers.ToDictionary(
                header => header.Key,
                header => header.Value),
        };
    }
}
