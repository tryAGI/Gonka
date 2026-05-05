
#nullable enable

namespace Gonka
{
    /// <summary>
    /// 
    /// </summary>
    public enum ChatCompletionToolChoiceType
    {
        /// <summary>
        /// 
        /// </summary>
        Function,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ChatCompletionToolChoiceTypeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ChatCompletionToolChoiceType value)
        {
            return value switch
            {
                ChatCompletionToolChoiceType.Function => "function",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ChatCompletionToolChoiceType? ToEnum(string value)
        {
            return value switch
            {
                "function" => ChatCompletionToolChoiceType.Function,
                _ => null,
            };
        }
    }
}