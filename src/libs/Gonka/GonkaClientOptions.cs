namespace Gonka;

/// <summary>
/// Options used to create a signed direct Gonka client.
/// </summary>
public sealed class GonkaClientOptions
{
    /// <summary>
    /// ECDSA secp256k1 private key used to sign Gonka requests. Hex with or without a 0x prefix.
    /// </summary>
    public string? PrivateKey { get; init; }

    /// <summary>
    /// Requester Gonka address. If omitted, the SDK derives it from <see cref="PrivateKey"/>.
    /// </summary>
    public string? RequesterAddress { get; init; }

    /// <summary>
    /// Explicit Gonka endpoints. Each endpoint must include the provider address used for signing.
    /// </summary>
    public IReadOnlyList<GonkaEndpoint>? Endpoints { get; init; }

    /// <summary>
    /// Source node URL used to discover active participant endpoints.
    /// </summary>
    public Uri? SourceUrl { get; init; }

    /// <summary>
    /// Optional endpoint selection strategy. If omitted, the SDK selects a random endpoint.
    /// </summary>
    public Func<IReadOnlyList<GonkaEndpoint>, GonkaEndpoint>? EndpointSelector { get; init; }

    /// <summary>
    /// Optional inner handler for the signed HttpClient pipeline.
    /// </summary>
    public HttpMessageHandler? HttpMessageHandler { get; init; }

    /// <summary>
    /// Optional handler used for endpoint discovery and provider identity probes.
    /// </summary>
    public HttpMessageHandler? DiscoveryHttpMessageHandler { get; init; }

    /// <summary>
    /// Signature payload mode. Defaults to the mode used by current official gonka-openai clients.
    /// </summary>
    public GonkaSignatureMode SignatureMode { get; init; } = GonkaSignatureMode.PayloadHash;

    /// <summary>
    /// Chain ID used when deriving a requester address.
    /// </summary>
    public string ChainId { get; init; } = GonkaDefaults.ChainId;
}
