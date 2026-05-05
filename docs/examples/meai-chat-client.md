# Microsoft.Extensions.AI Chat Client

MEAI example showing how to use Gonka through the standard IChatClient abstraction.

This example assumes `using Gonka;` is in scope and the Gonka environment variables described in the README are configured.

```csharp
// The Gonka client implements Microsoft.Extensions.AI.IChatClient for shared chat workflows.
using var client = await GetAuthenticatedClientAsync().ConfigureAwait(false);
Microsoft.Extensions.AI.IChatClient chatClient = client;

var model = Environment.GetEnvironmentVariable("GONKA_CHAT_MODEL") is { Length: > 0 } modelValue
    ? modelValue
    : "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8";

var response = await chatClient.GetResponseAsync(
    [new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, "Write one short sentence about Gonka.")],
    new Microsoft.Extensions.AI.ChatOptions { ModelId = model }).ConfigureAwait(false);
```
