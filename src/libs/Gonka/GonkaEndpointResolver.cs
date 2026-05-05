using System.Security.Cryptography;
using System.Text.Json;

namespace Gonka;

internal static class GonkaEndpointResolver
{
    public static async Task<IReadOnlyList<GonkaEndpoint>> ResolveAsync(
        GonkaClientOptions options,
        CancellationToken cancellationToken)
    {
        if (options.Endpoints is { Count: > 0 })
        {
            return options.Endpoints;
        }

        if (Environment.GetEnvironmentVariable(GonkaDefaults.EndpointsEnvironmentVariable) is { Length: > 0 } endpoints)
        {
            return ParseEndpoints(endpoints);
        }

        var sourceUrl = options.SourceUrl ?? GetSourceUrlFromEnvironment();
        if (sourceUrl is not null)
        {
            var discovered = await FetchParticipantsAsync(
                sourceUrl,
                options.DiscoveryHttpMessageHandler,
                cancellationToken).ConfigureAwait(false);
            if (discovered.Count > 0)
            {
                var allowed = await FetchAllowedTransferAddressesAsync(
                    sourceUrl,
                    options.DiscoveryHttpMessageHandler,
                    cancellationToken).ConfigureAwait(false);
                return allowed.Count > 0
                    ? discovered.Where(endpoint => allowed.Contains(endpoint.Address)).ToArray()
                    : discovered;
            }
        }

        return [];
    }

    public static GonkaEndpoint SelectEndpoint(
        IReadOnlyList<GonkaEndpoint> endpoints,
        Func<IReadOnlyList<GonkaEndpoint>, GonkaEndpoint>? selector)
    {
        if (endpoints.Count == 0)
        {
            throw new InvalidOperationException("No Gonka endpoints were resolved.");
        }

        return selector?.Invoke(endpoints) ?? endpoints[RandomNumberGenerator.GetInt32(endpoints.Count)];
    }

    public static async Task<GonkaEndpoint> ResolveSelectedEndpointAsync(
        GonkaClientOptions options,
        CancellationToken cancellationToken)
    {
        var endpoints = await ResolveAsync(options, cancellationToken).ConfigureAwait(false);
        var selected = SelectEndpoint(endpoints, options.EndpointSelector);
        var sourceUrl = options.SourceUrl ?? GetSourceUrlFromEnvironment();

        if (options.Endpoints is null &&
            Environment.GetEnvironmentVariable(GonkaDefaults.EndpointsEnvironmentVariable) is not { Length: > 0 } &&
            sourceUrl is not null)
        {
            var delegateEndpoints = await FetchNodeIdentityAsync(
                selected.Url,
                options.DiscoveryHttpMessageHandler,
                cancellationToken).ConfigureAwait(false);
            if (delegateEndpoints.Count > 0)
            {
                var delegateSelected = SelectEndpoint(delegateEndpoints, options.EndpointSelector);
                return new GonkaEndpoint(delegateSelected.Url, selected.Address);
            }
        }

        return selected;
    }

    private static GonkaEndpoint[] ParseEndpoints(string endpoints)
    {
        return endpoints
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(GonkaEndpoint.Parse)
            .ToArray();
    }

    private static Uri? GetSourceUrlFromEnvironment()
    {
        return Environment.GetEnvironmentVariable(GonkaDefaults.SourceUrlEnvironmentVariable) is { Length: > 0 } value
            ? new Uri(value, UriKind.Absolute)
            : null;
    }

    private static async Task<IReadOnlyList<GonkaEndpoint>> FetchParticipantsAsync(
        Uri sourceUrl,
        HttpMessageHandler? httpMessageHandler,
        CancellationToken cancellationToken)
    {
        using var httpClient = CreateHttpClient(httpMessageHandler);
        using var response = await httpClient.GetAsync(
            new Uri($"{sourceUrl.ToString().TrimEnd('/')}/v1/epochs/current/participants", UriKind.Absolute),
            cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        var root = document.RootElement;
        var excluded = ReadExcludedParticipants(root);
        var endpoints = new List<GonkaEndpoint>();

        if (!root.TryGetProperty("active_participants", out var activeParticipants) ||
            !activeParticipants.TryGetProperty("participants", out var participants) ||
            participants.ValueKind != JsonValueKind.Array)
        {
            return endpoints;
        }

        foreach (var participant in participants.EnumerateArray())
        {
            if (!participant.TryGetProperty("inference_url", out var urlProperty) ||
                !participant.TryGetProperty("index", out var addressProperty))
            {
                continue;
            }

            var url = urlProperty.GetString();
            var address = addressProperty.GetString();
            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(address) ||
                excluded.Contains(address))
            {
                continue;
            }

            endpoints.Add(new GonkaEndpoint(GonkaEndpoint.EnsureV1(new Uri(url, UriKind.Absolute)), address));
        }

        return endpoints;
    }

    private static HashSet<string> ReadExcludedParticipants(JsonElement root)
    {
        var excluded = new HashSet<string>(StringComparer.Ordinal);
        if (!root.TryGetProperty("excluded_participants", out var participants) ||
            participants.ValueKind != JsonValueKind.Array)
        {
            return excluded;
        }

        foreach (var participant in participants.EnumerateArray())
        {
            if (participant.TryGetProperty("address", out var address) &&
                address.GetString() is { Length: > 0 } value)
            {
                excluded.Add(value);
            }
        }

        return excluded;
    }

    private static async Task<HashSet<string>> FetchAllowedTransferAddressesAsync(
        Uri sourceUrl,
        HttpMessageHandler? httpMessageHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = CreateHttpClient(httpMessageHandler);
            var baseUrl = GonkaEndpoint.WithoutV1(sourceUrl).ToString().TrimEnd('/');
            using var response = await httpClient.GetAsync(
                new Uri($"{baseUrl}/chain-api/productscience/inference/inference/params", UriKind.Absolute),
                cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = new HashSet<string>(StringComparer.Ordinal);

            if (!document.RootElement.TryGetProperty("params", out var parameters) ||
                !parameters.TryGetProperty("transfer_agent_access_params", out var transferAgentAccess) ||
                !transferAgentAccess.TryGetProperty("allowed_transfer_addresses", out var addresses) ||
                addresses.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            foreach (var address in addresses.EnumerateArray())
            {
                if (address.GetString() is { Length: > 0 } value)
                {
                    result.Add(value);
                }
            }

            return result;
        }
        catch (HttpRequestException)
        {
            return [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static async Task<IReadOnlyList<GonkaEndpoint>> FetchNodeIdentityAsync(
        Uri nodeUrl,
        HttpMessageHandler? httpMessageHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = CreateHttpClient(httpMessageHandler);
            var baseUrl = GonkaEndpoint.WithoutV1(nodeUrl).ToString().TrimEnd('/');
            using var response = await httpClient.GetAsync(
                new Uri($"{baseUrl}/v1/identity", UriKind.Absolute),
                cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            var endpoints = new List<GonkaEndpoint>();

            if (!document.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("delegate_ta", out var delegates) ||
                delegates.ValueKind != JsonValueKind.Object)
            {
                return endpoints;
            }

            foreach (var property in delegates.EnumerateObject())
            {
                if (property.Value.GetString() is { Length: > 0 } address)
                {
                    endpoints.Add(new GonkaEndpoint(GonkaEndpoint.EnsureV1(new Uri(property.Name, UriKind.Absolute)), address));
                }
            }

            return endpoints;
        }
        catch (HttpRequestException)
        {
            return [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static HttpClient CreateHttpClient(HttpMessageHandler? httpMessageHandler)
    {
        return httpMessageHandler is null
            ? new HttpClient()
            : new HttpClient(httpMessageHandler, disposeHandler: false);
    }
}
