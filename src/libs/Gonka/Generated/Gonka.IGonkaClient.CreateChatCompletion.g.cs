#nullable enable

namespace Gonka
{
    public partial interface IGonkaClient
    {
        /// <summary>
        /// Create a chat completion<br/>
        /// Creates an OpenAI-compatible chat completion against a selected Gonka provider endpoint.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestOptions">Per-request overrides such as headers, query parameters, timeout, retries, and response buffering.</param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gonka.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gonka.CreateChatCompletionResponse> CreateChatCompletionAsync(

            global::Gonka.CreateChatCompletionRequest request,
            global::Gonka.AutoSDKRequestOptions? requestOptions = default,
            global::System.Threading.CancellationToken cancellationToken = default);
        /// <summary>
        /// Create a chat completion<br/>
        /// Creates an OpenAI-compatible chat completion against a selected Gonka provider endpoint.
        /// </summary>
        /// <param name="model">
        /// Gonka model identifier, for example Qwen/Qwen3-235B-A22B-Instruct-2507-FP8.
        /// </param>
        /// <param name="messages"></param>
        /// <param name="temperature"></param>
        /// <param name="topP"></param>
        /// <param name="maxTokens"></param>
        /// <param name="stream"></param>
        /// <param name="stop"></param>
        /// <param name="tools"></param>
        /// <param name="toolChoice"></param>
        /// <param name="responseFormat"></param>
        /// <param name="requestOptions">Per-request overrides such as headers, query parameters, timeout, retries, and response buffering.</param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gonka.CreateChatCompletionResponse> CreateChatCompletionAsync(
            string model,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessage> messages,
            double? temperature = default,
            double? topP = default,
            int? maxTokens = default,
            bool? stream = default,
            global::Gonka.OneOf<string, global::System.Collections.Generic.IList<string>>? stop = default,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionTool>? tools = default,
            global::Gonka.OneOf<string, global::Gonka.ChatCompletionToolChoice>? toolChoice = default,
            global::Gonka.ResponseFormat? responseFormat = default,
            global::Gonka.AutoSDKRequestOptions? requestOptions = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}