namespace Gonka;

public sealed partial class GonkaClient
{
    /// <summary>
    /// The selected Gonka provider endpoint for signed direct requests.
    /// </summary>
    public GonkaEndpoint? Endpoint { get; private set; }

    /// <summary>
    /// The requester Gonka address sent in the X-Requester-Address header.
    /// </summary>
    public string? RequesterAddress { get; private set; }

    /// <summary>
    /// Creates a signed direct Gonka client for one explicit endpoint.
    /// </summary>
    public GonkaClient(
        string privateKey,
        GonkaEndpoint endpoint,
        string? requesterAddress = null,
        HttpMessageHandler? httpMessageHandler = null,
        GonkaSignatureMode signatureMode = GonkaSignatureMode.PayloadHash,
        string chainId = GonkaDefaults.ChainId)
        : this(
            httpClient: CreateSignedHttpClient(
                privateKey,
                RequireEndpoint(endpoint),
                requesterAddress ?? GonkaCryptography.DeriveAddress(privateKey, chainId),
                httpMessageHandler,
                signatureMode),
            baseUri: RequireEndpoint(endpoint).Url,
            authorizations: null,
            options: null,
            disposeHttpClient: true)
    {
        Endpoint = RequireEndpoint(endpoint);
        RequesterAddress = requesterAddress ?? GonkaCryptography.DeriveAddress(privateKey, chainId);
    }

    /// <summary>
    /// Resolves a Gonka endpoint from options and creates a signed direct client.
    /// </summary>
    public static async Task<GonkaClient> CreateAsync(
        GonkaClientOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var privateKey = options.PrivateKey ??
                         Environment.GetEnvironmentVariable(GonkaDefaults.PrivateKeyEnvironmentVariable) ??
                         throw new InvalidOperationException($"{GonkaDefaults.PrivateKeyEnvironmentVariable} environment variable is not found.");
        var endpoint = await GonkaEndpointResolver.ResolveSelectedEndpointAsync(options, cancellationToken).ConfigureAwait(false);
        var requesterAddress = options.RequesterAddress ??
                               Environment.GetEnvironmentVariable(GonkaDefaults.AddressEnvironmentVariable) ??
                               GonkaCryptography.DeriveAddress(privateKey, options.ChainId);

        var client = new GonkaClient(
            privateKey,
            endpoint,
            requesterAddress,
            options.HttpMessageHandler,
            options.SignatureMode,
            options.ChainId);

        return client;
    }

    /// <summary>
    /// Creates a signed direct client from GONKA_PRIVATE_KEY plus GONKA_ENDPOINTS or GONKA_SOURCE_URL.
    /// </summary>
    public static Task<GonkaClient> CreateFromEnvironmentAsync(CancellationToken cancellationToken = default)
    {
        return CreateAsync(new GonkaClientOptions(), cancellationToken);
    }

    private static HttpClient CreateSignedHttpClient(
        string privateKey,
        GonkaEndpoint endpoint,
        string requesterAddress,
        HttpMessageHandler? httpMessageHandler,
        GonkaSignatureMode signatureMode)
    {
#pragma warning disable CA2000
        return new HttpClient(new GonkaSigningHandler(
            privateKey,
            requesterAddress,
            endpoint,
            signatureMode,
            httpMessageHandler));
#pragma warning restore CA2000
    }

    private static GonkaEndpoint RequireEndpoint(GonkaEndpoint? endpoint)
    {
        return endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }
}
