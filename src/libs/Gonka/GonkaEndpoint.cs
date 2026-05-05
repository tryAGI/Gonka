namespace Gonka;

/// <summary>
/// A Gonka inference endpoint and the provider address used when signing requests to it.
/// </summary>
public sealed record GonkaEndpoint(Uri Url, string Address)
{
    /// <summary>
    /// Endpoint URL normalized to the OpenAI-compatible /v1 base path.
    /// </summary>
    public Uri Url { get; init; } = RequireUrl(Url);

    /// <summary>
    /// Provider address used as the transfer address in Gonka request signatures.
    /// </summary>
    public string Address { get; init; } = RequireAddress(Address);

    /// <summary>
    /// Creates an endpoint from a URL and provider address.
    /// </summary>
    public GonkaEndpoint(string url, string address)
        : this(new Uri(url, UriKind.Absolute), address)
    {
    }

    /// <summary>
    /// Parses an endpoint entry in url;providerAddress format.
    /// </summary>
    public static GonkaEndpoint Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Endpoint value is required.", nameof(value));
        }

        var parts = value.Split(';', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 ||
            string.IsNullOrWhiteSpace(parts[0]) ||
            string.IsNullOrWhiteSpace(parts[1]))
        {
            throw new FormatException("Gonka endpoints must use the 'url;providerAddress' format.");
        }

        return new GonkaEndpoint(EnsureV1(new Uri(parts[0], UriKind.Absolute)), parts[1]);
    }

    internal GonkaEndpoint WithUrl(Uri url)
    {
        return this with { Url = EnsureV1(url) };
    }

    internal static Uri EnsureV1(Uri url)
    {
        var value = url.ToString().TrimEnd('/');
        return value.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
            ? new Uri(value, UriKind.Absolute)
            : new Uri($"{value}/v1", UriKind.Absolute);
    }

    internal static Uri WithoutV1(Uri url)
    {
        var value = url.ToString().TrimEnd('/');
        return value.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
            ? new Uri(value[..^3], UriKind.Absolute)
            : new Uri(value, UriKind.Absolute);
    }

    private static Uri RequireUrl(Uri? url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return EnsureV1(url);
    }

    private static string RequireAddress(string? address)
    {
        return string.IsNullOrWhiteSpace(address)
            ? throw new ArgumentException("Gonka endpoint provider address is required.", nameof(address))
            : address;
    }
}
