#nullable enable

namespace Gonka
{
    public partial interface IGonkaClient
    {
        /// <summary>
        /// List available models<br/>
        /// Returns the models exposed by the selected Gonka provider endpoint.
        /// </summary>
        /// <param name="requestOptions">Per-request overrides such as headers, query parameters, timeout, retries, and response buffering.</param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gonka.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gonka.ListModelsResponse> ListModelsAsync(
            global::Gonka.AutoSDKRequestOptions? requestOptions = default,
            global::System.Threading.CancellationToken cancellationToken = default);
        /// <summary>
        /// List available models<br/>
        /// Returns the models exposed by the selected Gonka provider endpoint.
        /// </summary>
        /// <param name="requestOptions">Per-request overrides such as headers, query parameters, timeout, retries, and response buffering.</param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gonka.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gonka.AutoSDKHttpResponse<global::Gonka.ListModelsResponse>> ListModelsAsResponseAsync(
            global::Gonka.AutoSDKRequestOptions? requestOptions = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}