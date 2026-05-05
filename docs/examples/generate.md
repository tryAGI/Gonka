# Generate

Basic example showing how to create a client and make a request.

This example assumes `using Gonka;` is in scope and the Gonka environment variables described in the README are configured.

```csharp
// Direct Gonka requests are signed with your Gonka private key.
// Provide either GONKA_ENDPOINTS (`https://host/v1;gonka1provider...`) or GONKA_SOURCE_URL for endpoint discovery.
using var client = await GetAuthenticatedClientAsync().ConfigureAwait(false);

var model = Environment.GetEnvironmentVariable("GONKA_CHAT_MODEL") is { Length: > 0 } modelValue
    ? modelValue
    : "Qwen/Qwen3-235B-A22B-Instruct-2507-FP8";

var response = await client.CreateChatCompletionAsync(
    new CreateChatCompletionRequest
    {
        Model = model,
        Messages =
        [
            new ChatCompletionMessage
            {
                Role = ChatCompletionMessageRole.User,
                Content = "Hello, Gonka!",
            },
        ],
    }).ConfigureAwait(false);
```
