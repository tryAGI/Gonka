/*
order: 10
title: Generate
slug: generate

Basic example showing how to create a client and make a request.
*/

namespace Gonka.IntegrationTests;

public partial class Tests
{
    [TestMethod]
    public async Task Example_Generate()
    {
        //// Direct Gonka requests are signed with your Gonka private key.
        //// Provide either GONKA_ENDPOINTS (`https://host/v1;gonka1provider...`) or GONKA_SOURCE_URL for endpoint discovery.
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

        response.Choices.Should().NotBeEmpty();
    }
}
