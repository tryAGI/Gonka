# Microsoft.Extensions.AI Integration

!!! tip "Cross-SDK comparison"
    See the [centralized MEAI documentation](https://tryagi.github.io/docs/meai/) for feature matrices and comparisons across all tryAGI SDKs.

The Gonka SDK implements `Microsoft.Extensions.AI.IChatClient` on top of the direct signed Gonka client. The adapter supports text chat, streaming updates, image URL/data content, JSON response-format hints, and function-tool request/response mapping.

## Installation

```bash
dotnet add package Gonka
```

## Usage

```csharp
using Gonka;
using Meai = Microsoft.Extensions.AI;

using var client = await GonkaClient.CreateFromEnvironmentAsync();
Meai.IChatClient chatClient = client;

var response = await chatClient.GetResponseAsync(
    [new Meai.ChatMessage(Meai.ChatRole.User, "Hello, Gonka!")],
    new Meai.ChatOptions
    {
        ModelId = "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8",
    });
```

## Notes

- Requests still use Gonka direct-network signing, so configure `GONKA_PRIVATE_KEY` plus `GONKA_ENDPOINTS` or `GONKA_SOURCE_URL`.
- Streaming uses `CreateChatCompletionStreamingAsync` internally and maps server-sent chunks to `ChatResponseUpdate`.
- See the [centralized MEAI docs](https://tryagi.github.io/docs/meai/) for cross-SDK comparisons.
