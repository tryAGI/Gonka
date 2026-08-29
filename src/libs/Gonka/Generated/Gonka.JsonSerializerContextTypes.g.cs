
#nullable enable

#pragma warning disable CS0618 // Type or member is obsolete

namespace Gonka
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class JsonSerializerContextTypes
    {
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.Dictionary<string, string>? StringStringDictionary { get; set; }

        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.Dictionary<string, object>? StringObjectDictionary { get; set; }

        /// <summary>
        /// Runtime object lists used by dynamic JSON payloads such as tool arguments.
        /// </summary>
        public global::System.Collections.Generic.List<object>? ObjectList { get; set; }

        /// <summary>
        ///
        /// </summary>
        public global::System.Text.Json.JsonElement? JsonElement { get; set; }

        /// <summary>
        ///
        /// </summary>
        public global::Gonka.CreateChatCompletionRequest? Type0 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string? Type1 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessage>? Type2 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionMessage? Type3 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public double? Type4 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public int? Type5 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public bool? Type6 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.OneOf<string, global::System.Collections.Generic.IList<string>>? Type7 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<string>? Type8 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionTool>? Type9 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionTool? Type10 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.OneOf<string, global::Gonka.ChatCompletionToolChoice>? Type11 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionToolChoice? Type12 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ResponseFormat? Type13 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionMessageRole? Type14 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>? Type15 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>? Type16 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionContentPart? Type17 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessageToolCall>? Type18 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionMessageToolCall? Type19 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ImageUrlContent? Type20 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionToolType? Type21 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.FunctionDefinition? Type22 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public object? Type23 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionToolChoiceType? Type24 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionToolChoiceFunction? Type25 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ResponseFormatType? Type26 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.CreateChatCompletionResponse? Type27 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public long? Type28 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.ChatCompletionChoice>? Type29 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionChoice? Type30 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.CompletionUsage? Type31 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionDelta? Type32 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.FunctionCall? Type33 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ChatCompletionDeltaRole? Type34 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ListModelsResponse? Type35 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.IList<global::Gonka.Model>? Type36 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.Model? Type37 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.ErrorResponse? Type38 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.Error? Type39 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.ChatCompletionMessage>? ListType0 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.OneOf<string, global::System.Collections.Generic.List<string>>? ListType1 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<string>? ListType2 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.ChatCompletionTool>? ListType3 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::Gonka.OneOf<string, global::System.Collections.Generic.List<global::Gonka.ChatCompletionContentPart>>? ListType4 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.ChatCompletionContentPart>? ListType5 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.ChatCompletionMessageToolCall>? ListType6 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.ChatCompletionChoice>? ListType7 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public global::System.Collections.Generic.List<global::Gonka.Model>? ListType8 { get; set; }
    }
}