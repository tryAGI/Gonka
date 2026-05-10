
#nullable enable

namespace Gonka
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ChatCompletionMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("role")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.ChatCompletionMessageRoleJsonConverter))]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gonka.ChatCompletionMessageRole Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>))]
        public global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tool_call_id")]
        public string? ToolCallId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tool_calls")]
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessageToolCall>? ToolCalls { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatCompletionMessage" /> class.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="toolCallId"></param>
        /// <param name="toolCalls"></param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ChatCompletionMessage(
            global::Gonka.ChatCompletionMessageRole role,
            global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>? content,
            string? name,
            string? toolCallId,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessageToolCall>? toolCalls)
        {
            this.Role = role;
            this.Content = content;
            this.Name = name;
            this.ToolCallId = toolCallId;
            this.ToolCalls = toolCalls;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatCompletionMessage" /> class.
        /// </summary>
        public ChatCompletionMessage()
        {
        }

    }
}