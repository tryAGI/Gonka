namespace Gonka;

/// <summary>
/// Controls how the request body is transformed before ECDSA signing.
/// </summary>
public enum GonkaSignatureMode
{
    /// <summary>
    /// Sign SHA256(body) hex, timestamp, and provider address. This matches the current gonka-openai clients.
    /// </summary>
    PayloadHash = 0,

    /// <summary>
    /// Sign raw body bytes, timestamp, and provider address. This matches older direct API examples.
    /// </summary>
    RawPayload = 1,
}
