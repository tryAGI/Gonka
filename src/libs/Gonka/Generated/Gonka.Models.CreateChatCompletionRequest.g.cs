
#nullable enable

namespace Gonka
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class CreateChatCompletionRequest
    {
        /// <summary>
        /// Gonka model identifier, for example Qwen/Qwen3-235B-A22B-Instruct-2507-FP8.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("model")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Model { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("messages")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessage> Messages { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public double? Temperature { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("top_p")]
        public double? TopP { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("stream")]
        public bool? Stream { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("stop")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<string>>))]
        public global::Gonka.OneOf<string, global::System.Collections.Generic.IList<string>>? Stop { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tools")]
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionTool>? Tools { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tool_choice")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::Gonka.ChatCompletionToolChoice>))]
        public global::Gonka.OneOf<string, global::Gonka.ChatCompletionToolChoice>? ToolChoice { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response_format")]
        public global::Gonka.ResponseFormat? ResponseFormat { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChatCompletionRequest" /> class.
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
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateChatCompletionRequest(
            string model,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessage> messages,
            double? temperature,
            double? topP,
            int? maxTokens,
            bool? stream,
            global::Gonka.OneOf<string, global::System.Collections.Generic.IList<string>>? stop,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionTool>? tools,
            global::Gonka.OneOf<string, global::Gonka.ChatCompletionToolChoice>? toolChoice,
            global::Gonka.ResponseFormat? responseFormat)
        {
            this.Model = model ?? throw new global::System.ArgumentNullException(nameof(model));
            this.Messages = messages ?? throw new global::System.ArgumentNullException(nameof(messages));
            this.Temperature = temperature;
            this.TopP = topP;
            this.MaxTokens = maxTokens;
            this.Stream = stream;
            this.Stop = stop;
            this.Tools = tools;
            this.ToolChoice = toolChoice;
            this.ResponseFormat = responseFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChatCompletionRequest" /> class.
        /// </summary>
        public CreateChatCompletionRequest()
        {
        }

    }
}