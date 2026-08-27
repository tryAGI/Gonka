#nullable enable

namespace Gonka.JsonConverters
{
    /// <inheritdoc />
    public sealed class ChatCompletionDeltaRoleNullableJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Gonka.ChatCompletionDeltaRole?>
    {
        /// <inheritdoc />
        public override global::Gonka.ChatCompletionDeltaRole? Read(
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
                        return global::Gonka.ChatCompletionDeltaRoleExtensions.ToEnum(stringValue);
                    }

                    break;
                }
                case global::System.Text.Json.JsonTokenType.Number:
                {
                    var numValue = reader.GetInt32();
                    return (global::Gonka.ChatCompletionDeltaRole)numValue;
                }
                case global::System.Text.Json.JsonTokenType.Null:
                {
                    return default(global::Gonka.ChatCompletionDeltaRole?);
                }
                default:
                    throw new global::System.ArgumentOutOfRangeException(nameof(reader));
            }

            return default;
        }

        /// <inheritdoc />
        public override void Write(
            global::System.Text.Json.Utf8JsonWriter writer,
            global::Gonka.ChatCompletionDeltaRole? value,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            writer = writer ?? throw new global::System.ArgumentNullException(nameof(writer));

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(global::Gonka.ChatCompletionDeltaRoleExtensions.ToValueString(value.Value));
            }
        }
    }
}
