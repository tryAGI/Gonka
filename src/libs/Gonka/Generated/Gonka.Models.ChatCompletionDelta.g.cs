
#nullable enable

namespace Gonka
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class ChatCompletionDelta
    {
        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("role")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.ChatCompletionDeltaRoleJsonConverter))]
        public global::Gonka.ChatCompletionDeltaRole? Role { get; set; }

        /// <summary>
        ///
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>))]
        public global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>? Content { get; set; }

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
        /// Initializes a new instance of the <see cref="ChatCompletionDelta" /> class.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        /// <param name="toolCalls"></param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ChatCompletionDelta(
            global::Gonka.ChatCompletionDeltaRole? role,
            global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>? content,
            global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessageToolCall>? toolCalls)
        {
            this.Role = role;
            this.Content = content;
            this.ToolCalls = toolCalls;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatCompletionDelta" /> class.
        /// </summary>
        public ChatCompletionDelta()
        {
        }

    }
}