#nullable enable

namespace Gonka.JsonConverters
{
    /// <inheritdoc />
    public sealed class ChatCompletionToolChoiceTypeJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Gonka.ChatCompletionToolChoiceType>
    {
        /// <inheritdoc />
        public override global::Gonka.ChatCompletionToolChoiceType Read(
            ref global::System.Text.Json.Utf8JsonReader reader,
            global::System.Type typeToConvert,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case global::System.Text.Json.JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (stringValue != null)
                    {
                        return global::Gonka.ChatCompletionToolChoiceTypeExtensions.ToEnum(stringValue) ?? default;
                    }
                    
                    break;
                }
                case global::System.Text.Json.JsonTokenType.Number:
                {
                    var numValue = reader.GetInt32();
                    return (global::Gonka.ChatCompletionToolChoiceType)numValue;
                }
                case global::System.Text.Json.JsonTokenType.Null:
                {
                    return default(global::Gonka.ChatCompletionToolChoiceType);
                }
                default:
                    throw new global::System.ArgumentOutOfRangeException(nameof(reader));
            }

            return default;
        }

        /// <inheritdoc />
        public override void Write(
            global::System.Text.Json.Utf8JsonWriter writer,
            global::Gonka.ChatCompletionToolChoiceType value,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            writer = writer ?? throw new global::System.ArgumentNullException(nameof(writer));

            writer.WriteStringValue(global::Gonka.ChatCompletionToolChoiceTypeExtensions.ToValueString(value));
        }
    }
}
