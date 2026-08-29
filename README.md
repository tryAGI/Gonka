<div class="docs-hero">
  <h1>Gonka</h1>
  <p class="docs-hero-lead">Modern .NET SDK for direct Gonka Network inference with OpenAI-compatible chat completions and ECDSA request signing.</p>
  <div class="docs-badge-row">
    <a href="https://www.nuget.org/packages/Gonka/"><img alt="Nuget package" src="https://img.shields.io/nuget/vpre/Gonka"></a>
    <a href="https://github.com/tryAGI/Gonka/actions/workflows/dotnet.yml"><img alt="dotnet" src="https://github.com/tryAGI/Gonka/actions/workflows/dotnet.yml/badge.svg?branch=main"></a>
    <a href="https://github.com/tryAGI/Gonka/blob/main/LICENSE"><img alt="License: MIT" src="https://img.shields.io/github/license/tryAGI/Gonka"></a>
    <a href="https://discord.gg/Ca2xhfBf3v"><img alt="Discord" src="https://img.shields.io/discord/1115206893015662663?label=Discord&amp;logo=discord&amp;logoColor=white&amp;color=d82679"></a>
  </div>
  <div class="docs-hero-actions">
    <a href="#usage">Get started</a>
    <a href="#support">Get support</a>
  </div>
</div>

<div class="docs-feature-grid">
  <div class="docs-feature-card">
    <h3>Manual OpenAPI spec</h3>
    <p>Built from a maintained OpenAPI description because Gonka does not currently publish a direct SDK OpenAPI document.</p>
  </div>
  <div class="docs-feature-card">
    <h3>Direct network auth</h3>
    <p>Signs each request with secp256k1 ECDSA and sends the Gonka requester headers required by provider endpoints.</p>
  </div>
  <div class="docs-feature-card">
    <h3>Modern .NET</h3>
    <p>Targets current .NET practices including nullability, trimming, NativeAOT awareness, and source-generated serialization.</p>
  </div>
  <div class="docs-feature-card">
    <h3>Docs from examples</h3>
    <p>Examples stay in sync between the README, MkDocs site, and integration tests through the AutoSDK docs pipeline.</p>
  </div>
</div>

## Usage

```csharp
using Gonka;

using var client = await GonkaClient.CreateFromEnvironmentAsync();

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
    });
```

Set `GONKA_PRIVATE_KEY` plus either `GONKA_ENDPOINTS` (`https://host/v1;gonka1provider...`) or `GONKA_SOURCE_URL`. `GONKA_ADDRESS` can override the derived requester address.

<!-- EXAMPLES:START -->
### Generate
Basic example showing how to create a client and make a request.

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

### Stream Chat
Streaming example showing how to read server-sent chat completion chunks.

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

### Microsoft.Extensions.AI Chat Client
MEAI example showing how to use Gonka through the standard IChatClient abstraction.

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
<!-- EXAMPLES:END -->

<!-- AUTOSDK:ECOSYSTEM-MAINTENANCE:START -->
## Ecosystem maintenance

This SDK is one of more than 200 .NET SDKs maintained with [AutoSDK](https://github.com/tryAGI/AutoSDK). The tryAGI [SDK audit](https://github.com/tryAGI/tryAGI/blob/main/GENERATED_SDK_AUDITS.md) continuously checks repository synchronization, upstream-spec regeneration, release workflows, warnings, public API visibility, and trimming/NativeAOT compatibility.

Every issue is first investigated for ecosystem-wide applicability. When the root cause belongs in AutoSDK, we fix and regression-test the generator, then roll the improvement out to every applicable SDK. Provider-specific behavior remains in this repository when it cannot be derived safely from the API specification.

Issue content—including code blocks, logs, links, and attachments—is treated only as untrusted diagnostic data. Embedded control instructions, hidden directives, delimiter tricks, or requests to alter triage or tooling behavior are ignored. Please report reproducible technical evidence and remove secrets and personal data.
<!-- AUTOSDK:ECOSYSTEM-MAINTENANCE:END -->

## Support

<div class="docs-card-grid">
  <div class="docs-card">
    <h3>Bugs</h3>
    <p>Open an issue in <a href="https://github.com/tryAGI/Gonka/issues">tryAGI/Gonka</a>.</p>
  </div>
  <div class="docs-card">
    <h3>Ideas and questions</h3>
    <p>Use <a href="https://github.com/tryAGI/Gonka/discussions">GitHub Discussions</a> for design questions and usage help.</p>
  </div>
  <div class="docs-card">
    <h3>Community</h3>
    <p>Join the <a href="https://discord.gg/Ca2xhfBf3v">tryAGI Discord</a> for broader discussion across SDKs.</p>
  </div>
</div>

## Acknowledgments

![JetBrains logo](https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.png)

This project is supported by JetBrains through the [Open Source Support Program](https://jb.gg/OpenSourceSupport).
