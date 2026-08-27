
#nullable enable

namespace Gonka
{
    /// <summary>
    ///
    /// </summary>
    public enum ChatCompletionDeltaRole
    {
        /// <summary>
        ///
        /// </summary>
        Assistant,
        /// <summary>
        ///
        /// </summary>
        System,
        /// <summary>
        ///
        /// </summary>
        Tool,
        /// <summary>
        ///
        /// </summary>
        User,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ChatCompletionDeltaRoleExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ChatCompletionDeltaRole value)
        {
            return value switch
            {
                ChatCompletionDeltaRole.Assistant => "assistant",
                ChatCompletionDeltaRole.System => "system",
                ChatCompletionDeltaRole.Tool => "tool",
                ChatCompletionDeltaRole.User => "user",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ChatCompletionDeltaRole? ToEnum(string value)
        {
            return value switch
            {
                "assistant" => ChatCompletionDeltaRole.Assistant,
                "system" => ChatCompletionDeltaRole.System,
                "tool" => ChatCompletionDeltaRole.Tool,
                "user" => ChatCompletionDeltaRole.User,
                _ => null,
            };
        }
    }
}