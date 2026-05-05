namespace Gonka;

/// <summary>
/// Default values and environment variable names used by direct Gonka Network clients.
/// </summary>
public static class GonkaDefaults
{
    /// <summary>
    /// Default Gonka mainnet chain ID used for address derivation.
    /// </summary>
    public const string ChainId = "gonka-mainnet";

    /// <summary>
    /// Environment variable containing a hex ECDSA secp256k1 private key.
    /// </summary>
    public const string PrivateKeyEnvironmentVariable = "GONKA_PRIVATE_KEY";

    /// <summary>
    /// Environment variable containing an optional precomputed requester Gonka address.
    /// </summary>
    public const string AddressEnvironmentVariable = "GONKA_ADDRESS";

    /// <summary>
    /// Environment variable containing comma-separated endpoint entries in url;providerAddress format.
    /// </summary>
    public const string EndpointsEnvironmentVariable = "GONKA_ENDPOINTS";

    /// <summary>
    /// Environment variable containing the source node URL used for endpoint discovery.
    /// </summary>
    public const string SourceUrlEnvironmentVariable = "GONKA_SOURCE_URL";

    /// <summary>
    /// Header carrying the requester Gonka address.
    /// </summary>
    public const string RequesterAddressHeader = "X-Requester-Address";

    /// <summary>
    /// Header carrying the nanosecond timestamp used in the request signature.
    /// </summary>
    public const string TimestampHeader = "X-Timestamp";
}
