
#nullable enable

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Gonka
{
    /// <summary>
    ///
    /// </summary>
    [global::System.Text.Json.Serialization.JsonSourceGenerationOptions(
        DefaultIgnoreCondition = global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = new global::System.Type[]
        {
            typeof(global::Gonka.JsonConverters.ChatCompletionMessageRoleJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionMessageRoleNullableJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionToolTypeJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionToolTypeNullableJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionToolChoiceTypeJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionToolChoiceTypeNullableJsonConverter),

            typeof(global::Gonka.JsonConverters.ResponseFormatTypeJsonConverter),

            typeof(global::Gonka.JsonConverters.ResponseFormatTypeNullableJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionDeltaRoleJsonConverter),

            typeof(global::Gonka.JsonConverters.ChatCompletionDeltaRoleNullableJsonConverter),

            typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<string>>),

            typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::Gonka.ChatCompletionToolChoice>),

            typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>),

            typeof(global::Gonka.JsonConverters.OneOfJsonConverter<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>),

            typeof(global::Gonka.JsonConverters.UnixTimestampJsonConverter),
        })]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.JsonSerializerContextTypes))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.CreateChatCompletionRequest))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(string))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessage>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionMessage))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(double))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(int))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(bool))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.OneOf<string, global::System.Collections.Generic.IList<string>>), TypeInfoPropertyName = "OneOfStringIListString2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<string>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.ChatCompletionTool>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionTool))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.OneOf<string, global::Gonka.ChatCompletionToolChoice>), TypeInfoPropertyName = "OneOfStringChatCompletionToolChoice2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionToolChoice))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ResponseFormat))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionMessageRole), TypeInfoPropertyName = "ChatCompletionMessageRole2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.OneOf<string, global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>>), TypeInfoPropertyName = "OneOfStringIListChatCompletionContentPart2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.ChatCompletionContentPart>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionContentPart))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.ChatCompletionMessageToolCall>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionMessageToolCall))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ImageUrlContent))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionToolType), TypeInfoPropertyName = "ChatCompletionToolType2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.FunctionDefinition))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(object))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionToolChoiceType), TypeInfoPropertyName = "ChatCompletionToolChoiceType2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionToolChoiceFunction))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ResponseFormatType), TypeInfoPropertyName = "ResponseFormatType2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.CreateChatCompletionResponse))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(long))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.ChatCompletionChoice>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionChoice))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.CompletionUsage))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionDelta))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.FunctionCall))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ChatCompletionDeltaRole), TypeInfoPropertyName = "ChatCompletionDeltaRole2")]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ListModelsResponse))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.IList<global::Gonka.Model>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.Model))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.ErrorResponse))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.Error))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.ChatCompletionMessage>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.OneOf<string, global::System.Collections.Generic.List<string>>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<string>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.ChatCompletionTool>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gonka.OneOf<string, global::System.Collections.Generic.List<global::Gonka.ChatCompletionContentPart>>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.ChatCompletionContentPart>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.ChatCompletionMessageToolCall>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.ChatCompletionChoice>))]
    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::System.Collections.Generic.List<global::Gonka.Model>))]
    public sealed partial class SourceGenerationContext : global::System.Text.Json.Serialization.JsonSerializerContext
    {
    }
}