# Stream Chat

Streaming example showing how to read server-sent chat completion chunks.

This example assumes `using Gonka;` is in scope and the Gonka environment variables described in the README are configured.

```csharp
// Streaming requests use the same direct Gonka signing layer as non-streaming requests.
using var client = await GetAuthenticatedClientAsync().ConfigureAwait(false);

var model = Environment.GetEnvironmentVariable("GONKA_CHAT_MODEL") is { Length: > 0 } modelValue
    ? modelValue
    : "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8";

var chunks = new List<CreateChatCompletionResponse>();
await foreach (var chunk in client.CreateChatCompletionStreamingAsync(
    new CreateChatCompletionRequest
    {
        Model = model,
        Messages =
        [
            new ChatCompletionMessage
            {
                Role = ChatCompletionMessageRole.User,
                Content = "Write one short sentence about Gonka.",
            },
        ],
    }))
{
    chunks.Add(chunk);
}
```
