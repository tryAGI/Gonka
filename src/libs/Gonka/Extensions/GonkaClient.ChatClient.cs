using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Meai = Microsoft.Extensions.AI;

namespace Gonka;

public sealed partial class GonkaClient : Meai.IChatClient
{
    private const string DefaultChatModel = "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8";
    private Meai.ChatClientMetadata? _chatMetadata;

    object? Meai.IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(Meai.ChatClientMetadata) ? (_chatMetadata ??= new(nameof(GonkaClient), BaseUri)) :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    async Task<Meai.ChatResponse> Meai.IChatClient.GetResponseAsync(
        IEnumerable<Meai.ChatMessage> messages,
        Meai.ChatOptions? options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var request = CreateRequest(messages, options, stream: false);
        var response = await CreateChatCompletionAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);

        return CreateChatResponse(response, options?.ModelId);
    }

    async IAsyncEnumerable<Meai.ChatResponseUpdate> Meai.IChatClient.GetStreamingResponseAsync(
        IEnumerable<Meai.ChatMessage> messages,
        Meai.ChatOptions? options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var request = CreateRequest(messages, options, stream: true);
        var toolCallBuilders = new Dictionary<int, (string Id, string Name, System.Text.StringBuilder Arguments)>();

        await foreach (var chunk in CreateChatCompletionStreamingAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            var choice = chunk.Choices.Count > 0 ? chunk.Choices[0] : null;
            var update = new Meai.ChatResponseUpdate
            {
                ResponseId = chunk.Id,
                ModelId = chunk.Model ?? options?.ModelId,
                RawRepresentation = chunk,
            };

            if (chunk.Created is { } created)
            {
                update.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(created);
            }

            if (choice is not null)
            {
                update.Role = ToChatRole(choice.Delta?.Role) ?? Meai.ChatRole.Assistant;

                var text = ExtractText(choice.Delta?.Content);
                if (!string.IsNullOrEmpty(text))
                {
                    update.Contents.Add(new Meai.TextContent(text)
                    {
                        RawRepresentation = choice.Delta,
                    });
                }

                if (choice.Delta?.ToolCalls is { Count: > 0 } toolCalls)
                {
                    foreach (var toolCall in toolCalls)
                    {
                        var index = toolCall.Index ?? 0;
                        if (!toolCallBuilders.TryGetValue(index, out var builder))
                        {
                            builder = (
                                Id: toolCall.Id ?? string.Empty,
                                Name: toolCall.Function?.Name ?? string.Empty,
                                Arguments: new System.Text.StringBuilder());
                            toolCallBuilders[index] = builder;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(toolCall.Id))
                            {
                                builder = builder with { Id = toolCall.Id! };
                            }

                            if (!string.IsNullOrEmpty(toolCall.Function?.Name))
                            {
                                builder = builder with { Name = toolCall.Function!.Name! };
                            }

                            toolCallBuilders[index] = builder;
                        }

                        if (!string.IsNullOrEmpty(toolCall.Function?.Arguments))
                        {
                            toolCallBuilders[index].Arguments.Append(toolCall.Function!.Arguments);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(choice.FinishReason))
                {
                    update.FinishReason = ToFinishReason(choice.FinishReason);
                    foreach (var (_, toolCall) in toolCallBuilders)
                    {
                        update.Contents.Add(new Meai.FunctionCallContent(
                            toolCall.Id,
                            toolCall.Name,
                            ParseArguments(toolCall.Arguments.ToString())));
                    }

                    toolCallBuilders.Clear();
                }
            }

            if (chunk.Usage is { } usage)
            {
                update.Contents.Add(new Meai.UsageContent(CreateUsageDetails(usage))
                {
                    RawRepresentation = usage,
                });
            }

            yield return update;
        }
    }

    private static CreateChatCompletionRequest CreateRequest(
        IEnumerable<Meai.ChatMessage> messages,
        Meai.ChatOptions? options,
        bool stream)
    {
        var requestMessages = new List<ChatCompletionMessage>();

        if (!string.IsNullOrWhiteSpace(options?.Instructions))
        {
            requestMessages.Add(new ChatCompletionMessage
            {
                Role = ChatCompletionMessageRole.System,
                Content = options!.Instructions!,
            });
        }

        foreach (var message in messages)
        {
            requestMessages.Add(ToGonkaMessage(message));
        }

        var request = options?.RawRepresentationFactory?.Invoke(null!) as CreateChatCompletionRequest;
        if (request is null)
        {
            request = new CreateChatCompletionRequest
            {
                Model = options?.ModelId ?? DefaultChatModel,
                Messages = requestMessages,
            };
        }
        else
        {
            request.Model = string.IsNullOrWhiteSpace(request.Model) ? options?.ModelId ?? DefaultChatModel : request.Model;
            request.Messages ??= [];
            foreach (var message in requestMessages)
            {
                request.Messages.Add(message);
            }
        }

        request.Stream = stream;
        ApplyOptions(request, options);
        return request;
    }

    private static ChatCompletionMessage ToGonkaMessage(Meai.ChatMessage message)
    {
        if (message.Role == Meai.ChatRole.Tool)
        {
            var result = message.Contents.OfType<Meai.FunctionResultContent>().FirstOrDefault();
            return new ChatCompletionMessage
            {
                Role = ChatCompletionMessageRole.Tool,
                ToolCallId = result?.CallId,
                Content = ToResultString(result),
            };
        }

        if (message.Role == Meai.ChatRole.Assistant)
        {
            var text = string.Concat(message.Contents.OfType<Meai.TextContent>().Select(content => content.Text));
            var toolCalls = message.Contents.OfType<Meai.FunctionCallContent>().ToList();
            var assistantMessage = new ChatCompletionMessage
            {
                Role = ChatCompletionMessageRole.Assistant,
                Content = string.IsNullOrEmpty(text)
                    ? (OneOf<string, IList<ChatCompletionContentPart>>?)null
                    : new OneOf<string, IList<ChatCompletionContentPart>>(text),
            };

            if (toolCalls.Count > 0)
            {
                assistantMessage.ToolCalls = toolCalls.Select(toolCall => new ChatCompletionMessageToolCall
                {
                    Id = toolCall.CallId,
                    Type = "function",
                    Function = new FunctionCall
                    {
                        Name = toolCall.Name,
                        Arguments = toolCall.Arguments is { } arguments ? SerializeArguments(arguments) : "{}",
                    },
                }).ToList();
            }

            return assistantMessage;
        }

        return new ChatCompletionMessage
        {
            Role = message.Role == Meai.ChatRole.System ? ChatCompletionMessageRole.System : ChatCompletionMessageRole.User,
            Content = ToContent(message.Contents),
        };
    }

    private static OneOf<string, IList<ChatCompletionContentPart>>? ToContent(
        IList<Meai.AIContent> contents)
    {
        var parts = new List<ChatCompletionContentPart>();

        foreach (var content in contents)
        {
            switch (content)
            {
                case Meai.TextContent text:
                    parts.Add(new ChatCompletionContentPart
                    {
                        Type = "text",
                        Text = text.Text,
                    });
                    break;

                case Meai.DataContent data when data.HasTopLevelMediaType("image"):
                    parts.Add(new ChatCompletionContentPart
                    {
                        Type = "image_url",
                        ImageUrl = new ImageUrlContent
                        {
                            Url = data.Uri?.ToString() ?? ToDataUri(data),
                        },
                    });
                    break;
            }
        }

        return parts.Count switch
        {
            0 => null,
            1 when parts[0].Type == "text" => parts[0].Text ?? string.Empty,
            _ => parts,
        };
    }

    private static string ToDataUri(Meai.DataContent data)
    {
        var mediaType = string.IsNullOrWhiteSpace(data.MediaType) ? "application/octet-stream" : data.MediaType;
        return $"data:{mediaType};base64,{Convert.ToBase64String(data.Data.ToArray())}";
    }

    private static void ApplyOptions(CreateChatCompletionRequest request, Meai.ChatOptions? options)
    {
        if (options is null)
        {
            return;
        }

        request.Temperature ??= options.Temperature;
        request.TopP ??= options.TopP;
        request.MaxTokens ??= options.MaxOutputTokens;

        if (request.Stop is null && options.StopSequences is { Count: > 0 } stopSequences)
        {
            request.Stop = stopSequences.Count == 1
                ? stopSequences[0]
                : stopSequences.ToList();
        }

        if (request.ResponseFormat is null)
        {
            request.ResponseFormat = ToResponseFormat(options.ResponseFormat);
        }

        ApplyTools(request, options);
        ApplyAdditionalProperties(request, options);
    }

    private static ResponseFormat? ToResponseFormat(Meai.ChatResponseFormat? responseFormat)
    {
        return responseFormat switch
        {
            null => null,
            Meai.ChatResponseFormatText => new ResponseFormat { Type = ResponseFormatType.Text },
            Meai.ChatResponseFormatJson jsonFormat => new ResponseFormat
            {
                Type = jsonFormat.Schema is JsonElement schema &&
                       schema.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined
                    ? ResponseFormatType.JsonSchema
                    : ResponseFormatType.JsonObject,
                JsonSchema = CreateJsonSchemaFormat(jsonFormat),
            },
            _ => null,
        };
    }

    private static Dictionary<string, object?>? CreateJsonSchemaFormat(Meai.ChatResponseFormatJson jsonFormat)
    {
        if (jsonFormat.Schema is not JsonElement schema ||
            schema.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        var result = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["name"] = jsonFormat.SchemaName ?? "response",
            ["schema"] = schema,
            ["strict"] = true,
        };

        if (!string.IsNullOrWhiteSpace(jsonFormat.SchemaDescription))
        {
            result["description"] = jsonFormat.SchemaDescription;
        }

        return result;
    }

    private static void ApplyTools(CreateChatCompletionRequest request, Meai.ChatOptions options)
    {
        if (options.ToolMode is Meai.NoneChatToolMode)
        {
            request.ToolChoice = "none";
            return;
        }

        if (options.Tools is { Count: > 0 } tools)
        {
            request.Tools ??= [];
            foreach (var tool in tools)
            {
                if (tool is not Meai.AIFunction function)
                {
                    throw new NotSupportedException(
                        $"Tool type '{tool.GetType().Name}' is not supported by Gonka. Only function tools are supported.");
                }

                request.Tools.Add(new ChatCompletionTool
                {
                    Type = ChatCompletionToolType.Function,
                    Function = new FunctionDefinition
                    {
                        Name = function.Name,
                        Description = function.Description,
                        Parameters = function.JsonSchema.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
                            ? new object()
                            : function.JsonSchema,
                    },
                });
            }
        }

        if (options.ToolMode is Meai.RequiredChatToolMode requiredToolMode)
        {
            request.ToolChoice = !string.IsNullOrWhiteSpace(requiredToolMode.RequiredFunctionName)
                ? new ChatCompletionToolChoice
                {
                    Type = ChatCompletionToolChoiceType.Function,
                    Function = new ChatCompletionToolChoiceFunction
                    {
                        Name = requiredToolMode.RequiredFunctionName,
                    },
                }
                : "required";
        }
    }

    private static void ApplyAdditionalProperties(
        CreateChatCompletionRequest request,
        Meai.ChatOptions options)
    {
        AddAdditionalProperty(request, "frequency_penalty", options.FrequencyPenalty);
        AddAdditionalProperty(request, "presence_penalty", options.PresencePenalty);
        AddAdditionalProperty(request, "seed", options.Seed);

        if (options.AdditionalProperties is not { Count: > 0 } additionalProperties)
        {
            return;
        }

        foreach (var property in additionalProperties)
        {
            if (property.Value is not null)
            {
                request.AdditionalProperties[property.Key] = property.Value;
            }
        }
    }

    private static void AddAdditionalProperty(
        CreateChatCompletionRequest request,
        string name,
        object? value)
    {
        if (value is not null)
        {
            request.AdditionalProperties[name] = value;
        }
    }

    private static Meai.ChatResponse CreateChatResponse(
        CreateChatCompletionResponse response,
        string? requestedModelId)
    {
        var choice = response.Choices.Count > 0 ? response.Choices[0] : null;
        var message = new Meai.ChatMessage
        {
            Role = Meai.ChatRole.Assistant,
            RawRepresentation = response,
        };

        if (choice?.Message is { } assistantMessage)
        {
            AddAssistantContents(message.Contents, assistantMessage);
        }

        var chatResponse = new Meai.ChatResponse(message)
        {
            ResponseId = response.Id,
            ModelId = response.Model ?? requestedModelId,
            RawRepresentation = response,
            FinishReason = choice is not null ? ToFinishReason(choice.FinishReason) : null,
            Usage = response.Usage is { } usage ? CreateUsageDetails(usage) : null,
        };

        if (response.Created is { } created)
        {
            chatResponse.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(created);
        }

        return chatResponse;
    }

    private static void AddAssistantContents(
        IList<Meai.AIContent> contents,
        ChatCompletionMessage message)
    {
        var text = ExtractText(message.Content);
        if (!string.IsNullOrEmpty(text))
        {
            contents.Add(new Meai.TextContent(text)
            {
                RawRepresentation = message,
            });
        }

        if (message.ToolCalls is { Count: > 0 } toolCalls)
        {
            foreach (var toolCall in toolCalls)
            {
                contents.Add(new Meai.FunctionCallContent(
                    toolCall.Id ?? string.Empty,
                    toolCall.Function?.Name ?? string.Empty,
                    ParseArguments(toolCall.Function?.Arguments))
                {
                    RawRepresentation = toolCall,
                });
            }
        }
    }

    private static string? ExtractText(OneOf<string, IList<ChatCompletionContentPart>>? content)
    {
        if (content is null)
        {
            return null;
        }

        if (content.Value.IsValue1)
        {
            return content.Value.Value1;
        }

        return content.Value.IsValue2
            ? string.Concat(content.Value.Value2!
                .Where(part => string.Equals(part.Type, "text", StringComparison.OrdinalIgnoreCase))
                .Select(part => part.Text))
            : null;
    }

    private static Dictionary<string, object?>? ParseArguments(string? arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments) || arguments == "{}")
        {
            return null;
        }

        try
        {
            var element = JsonSerializer.Deserialize(arguments, SourceGenerationContext.Default.JsonElement);
            if (element.ValueKind == JsonValueKind.Object)
            {
                var result = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var property in element.EnumerateObject())
                {
                    result[property.Name] = property.Value;
                }

                return result;
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }

    private static string ToResultString(Meai.FunctionResultContent? functionResult)
    {
        if (functionResult is null)
        {
            return string.Empty;
        }

        if (functionResult.Result is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.String
                ? jsonElement.GetString() ?? string.Empty
                : jsonElement.GetRawText();
        }

        if (functionResult.Result is string text)
        {
            return text;
        }

        return functionResult.Result is not null
            ? SerializeJsonValue(functionResult.Result)
            : functionResult.Exception?.Message ?? string.Empty;
    }

    private static Meai.ChatRole? ToChatRole(ChatCompletionDeltaRole? role)
    {
        return role switch
        {
            ChatCompletionDeltaRole.Assistant => Meai.ChatRole.Assistant,
            ChatCompletionDeltaRole.System => Meai.ChatRole.System,
            ChatCompletionDeltaRole.Tool => Meai.ChatRole.Tool,
            ChatCompletionDeltaRole.User => Meai.ChatRole.User,
            _ => null,
        };
    }

    private static Meai.ChatFinishReason? ToFinishReason(string? finishReason)
    {
        return finishReason switch
        {
            null or "" => null,
            "stop" => Meai.ChatFinishReason.Stop,
            "length" or "model_length" => Meai.ChatFinishReason.Length,
            "tool_calls" or "function_call" => Meai.ChatFinishReason.ToolCalls,
            "content_filter" => Meai.ChatFinishReason.ContentFilter,
            _ => new Meai.ChatFinishReason(finishReason),
        };
    }

    private static Meai.UsageDetails CreateUsageDetails(CompletionUsage usage)
    {
        return new Meai.UsageDetails
        {
            InputTokenCount = usage.PromptTokens,
            OutputTokenCount = usage.CompletionTokens,
            TotalTokenCount = usage.TotalTokens,
        };
    }

    private static string SerializeArguments(IEnumerable<KeyValuePair<string, object?>> arguments)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteDictionary(writer, arguments);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static string SerializeJsonValue(object? value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteJsonValue(writer, value);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteDictionary(
        Utf8JsonWriter writer,
        IEnumerable<KeyValuePair<string, object?>> values)
    {
        writer.WriteStartObject();
        foreach (var value in values)
        {
            writer.WritePropertyName(value.Key);
            WriteJsonValue(writer, value.Value);
        }

        writer.WriteEndObject();
    }

    private static void WriteJsonValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case JsonElement jsonElement:
                jsonElement.WriteTo(writer);
                break;
            case JsonDocument jsonDocument:
                jsonDocument.RootElement.WriteTo(writer);
                break;
            case string text:
                writer.WriteStringValue(text);
                break;
            case bool boolean:
                writer.WriteBooleanValue(boolean);
                break;
            case int number:
                writer.WriteNumberValue(number);
                break;
            case long number:
                writer.WriteNumberValue(number);
                break;
            case float number:
                writer.WriteNumberValue(number);
                break;
            case double number:
                writer.WriteNumberValue(number);
                break;
            case decimal number:
                writer.WriteNumberValue(number);
                break;
            case IEnumerable<KeyValuePair<string, object?>> dictionary:
                WriteDictionary(writer, dictionary);
                break;
            case System.Collections.IDictionary dictionary:
                writer.WriteStartObject();
                foreach (System.Collections.DictionaryEntry entry in dictionary)
                {
                    if (entry.Key is string key)
                    {
                        writer.WritePropertyName(key);
                        WriteJsonValue(writer, entry.Value);
                    }
                }

                writer.WriteEndObject();
                break;
            case System.Collections.IEnumerable enumerable:
                writer.WriteStartArray();
                foreach (var item in enumerable)
                {
                    WriteJsonValue(writer, item);
                }

                writer.WriteEndArray();
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }
}
