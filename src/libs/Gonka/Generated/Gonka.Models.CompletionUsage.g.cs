
#nullable enable

namespace Gonka
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class CompletionUsage
    {
        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prompt_tokens")]
        public int? PromptTokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("completion_tokens")]
        public int? CompletionTokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("total_tokens")]
        public int? TotalTokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("base_cost_usd")]
        public double? BaseCostUsd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("platform_fee_usd")]
        public double? PlatformFeeUsd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("total_cost_usd")]
        public double? TotalCostUsd { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionUsage" /> class.
        /// </summary>
        /// <param name="promptTokens"></param>
        /// <param name="completionTokens"></param>
        /// <param name="totalTokens"></param>
        /// <param name="baseCostUsd"></param>
        /// <param name="platformFeeUsd"></param>
        /// <param name="totalCostUsd"></param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CompletionUsage(
            int? promptTokens,
            int? completionTokens,
            int? totalTokens,
            double? baseCostUsd,
            double? platformFeeUsd,
            double? totalCostUsd)
        {
            this.PromptTokens = promptTokens;
            this.CompletionTokens = completionTokens;
            this.TotalTokens = totalTokens;
            this.BaseCostUsd = baseCostUsd;
            this.PlatformFeeUsd = platformFeeUsd;
            this.TotalCostUsd = totalCostUsd;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionUsage" /> class.
        /// </summary>
        public CompletionUsage()
        {
        }

    }
}