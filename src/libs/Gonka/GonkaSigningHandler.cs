namespace Gonka;

internal sealed class GonkaSigningHandler : DelegatingHandler
{
    private readonly string _privateKey;
    private readonly string _requesterAddress;
    private readonly GonkaEndpoint _endpoint;
    private readonly GonkaSignatureMode _signatureMode;

    public GonkaSigningHandler(
        string privateKey,
        string requesterAddress,
        GonkaEndpoint endpoint,
        GonkaSignatureMode signatureMode,
        HttpMessageHandler? innerHandler = null)
#pragma warning disable CA2000
        : base(innerHandler ?? new HttpClientHandler())
#pragma warning restore CA2000
    {
        _privateKey = privateKey;
        _requesterAddress = requesterAddress;
        _endpoint = endpoint;
        _signatureMode = signatureMode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var payload = await ReadAndRestoreContentAsync(request, cancellationToken).ConfigureAwait(false);
        var timestamp = GonkaTimestamp.GetUnixTimeNanoseconds();
        var signature = GonkaCryptography.SignPayload(
            payload,
            _privateKey,
            timestamp,
            _endpoint.Address,
            _signatureMode);

        request.Headers.Remove("Authorization");
        request.Headers.TryAddWithoutValidation("Authorization", signature);
        request.Headers.Remove(GonkaDefaults.RequesterAddressHeader);
        request.Headers.TryAddWithoutValidation(GonkaDefaults.RequesterAddressHeader, _requesterAddress);
        request.Headers.Remove(GonkaDefaults.TimestampHeader);
        request.Headers.TryAddWithoutValidation(GonkaDefaults.TimestampHeader, timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture));

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<byte[]> ReadAndRestoreContentAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return [];
        }

        var originalContent = request.Content;
        var payload = await originalContent.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        var replacement = new ByteArrayContent(payload);

        foreach (var header in originalContent.Headers)
        {
            replacement.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        request.Content = replacement;
        return payload;
    }
}
