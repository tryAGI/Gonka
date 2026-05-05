using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Gonka;

/// <summary>
/// Cryptographic helpers for direct Gonka request signing and address derivation.
/// </summary>
public static class GonkaCryptography
{
    private const string Bech32Alphabet = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
    private static readonly int[] Bech32ChecksumPadding = [0, 0, 0, 0, 0, 0];
    private static readonly int[] Bech32PolymodGenerator = [0x3b6a57b2, 0x26508e6d, 0x1ea119fa, 0x3d4233dd, 0x2a1462b3];

    /// <summary>
    /// Signs a request payload with a Gonka secp256k1 private key.
    /// </summary>
    public static string SignPayload(
        byte[] payload,
        string privateKeyHex,
        long timestampNanoseconds,
        string providerAddress,
        GonkaSignatureMode signatureMode = GonkaSignatureMode.PayloadHash)
    {
        ArgumentNullException.ThrowIfNull(payload);

        if (string.IsNullOrWhiteSpace(providerAddress))
        {
            throw new ArgumentException("Provider address is required.", nameof(providerAddress));
        }

        var privateKey = ParsePrivateKey(privateKeyHex);
        var signatureInput = CreateSignatureInput(payload, timestampNanoseconds, providerAddress, signatureMode);
        var signatureHash = SHA256.HashData(signatureInput);
        var parameters = CreatePrivateKeyParameters(privateKey);
        var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));

        signer.Init(true, parameters);
        var signature = signer.GenerateSignature(signatureHash);
        var r = signature[0];
        var s = NormalizeLowS(signature[1], parameters.Parameters.N);

        var bytes = new byte[64];
        WriteFixedLength(r, bytes.AsSpan(0, 32));
        WriteFixedLength(s, bytes.AsSpan(32, 32));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Derives a Gonka bech32 requester address from a secp256k1 private key.
    /// </summary>
    public static string DeriveAddress(string privateKeyHex, string chainId = GonkaDefaults.ChainId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chainId);

        var privateKey = ParsePrivateKey(privateKeyHex);
        var curve = SecNamedCurves.GetByName("secp256k1");
        var point = curve.G.Multiply(new BigInteger(1, privateKey)).Normalize();
        var compressedPublicKey = point.GetEncoded(true);
        var sha256 = SHA256.HashData(compressedPublicKey);
        var ripeMd160 = new RipeMD160Digest();
        var addressBytes = new byte[ripeMd160.GetDigestSize()];

        ripeMd160.BlockUpdate(sha256, 0, sha256.Length);
        ripeMd160.DoFinal(addressBytes, 0);

        var prefix = chainId.Split('-', 2, StringSplitOptions.RemoveEmptyEntries)[0];
        return EncodeBech32(prefix, ConvertBits(addressBytes, 8, 5, pad: true));
    }

    private static byte[] CreateSignatureInput(
        byte[] payload,
        long timestampNanoseconds,
        string providerAddress,
        GonkaSignatureMode signatureMode)
    {
        var timestamp = timestampNanoseconds.ToString(CultureInfo.InvariantCulture);
        if (signatureMode == GonkaSignatureMode.RawPayload)
        {
            var timestampBytes = Encoding.UTF8.GetBytes(timestamp);
            var providerBytes = Encoding.UTF8.GetBytes(providerAddress);
            var input = new byte[payload.Length + timestampBytes.Length + providerBytes.Length];

            payload.CopyTo(input, 0);
            timestampBytes.CopyTo(input.AsSpan(payload.Length));
            providerBytes.CopyTo(input.AsSpan(payload.Length + timestampBytes.Length));
            return input;
        }

        // Gonka reference clients use lowercase hexadecimal payload hashes in the signature input.
#pragma warning disable CA1308
        var payloadHashHex = Convert.ToHexString(SHA256.HashData(payload)).ToLowerInvariant();
#pragma warning restore CA1308
        return Encoding.UTF8.GetBytes(payloadHashHex + timestamp + providerAddress);
    }

    private static ECPrivateKeyParameters CreatePrivateKeyParameters(byte[] privateKey)
    {
        var curve = SecNamedCurves.GetByName("secp256k1");
        var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        return new ECPrivateKeyParameters(new BigInteger(1, privateKey), domain);
    }

    private static BigInteger NormalizeLowS(BigInteger s, BigInteger curveOrder)
    {
        var halfOrder = curveOrder.ShiftRight(1);
        return s.CompareTo(halfOrder) > 0
            ? curveOrder.Subtract(s)
            : s;
    }

    private static void WriteFixedLength(BigInteger value, Span<byte> destination)
    {
        var source = value.ToByteArrayUnsigned();
        if (source.Length > destination.Length)
        {
            throw new InvalidOperationException("Signature component is too large.");
        }

        destination.Clear();
        source.CopyTo(destination[^source.Length..]);
    }

    private static byte[] ParsePrivateKey(string privateKeyHex)
    {
        if (string.IsNullOrWhiteSpace(privateKeyHex))
        {
            throw new ArgumentException("Private key is required.", nameof(privateKeyHex));
        }

        var value = privateKeyHex.Trim();
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            value = value[2..];
        }

        if (value.Length != 64)
        {
            throw new FormatException("Gonka private keys must be 32-byte hex strings.");
        }

        return Convert.FromHexString(value);
    }

    private static string EncodeBech32(string humanReadablePart, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(humanReadablePart))
        {
            throw new ArgumentException("Bech32 prefix is required.", nameof(humanReadablePart));
        }

        // Bech32 addresses are conventionally emitted in lowercase for Cosmos-style chains.
#pragma warning disable CA1308
        var prefix = humanReadablePart.ToLowerInvariant();
#pragma warning restore CA1308
        var checksum = CreateChecksum(prefix, data);
        var builder = new StringBuilder(prefix.Length + 1 + data.Length + checksum.Length);

        builder.Append(prefix);
        builder.Append('1');

        foreach (var value in data)
        {
            builder.Append(Bech32Alphabet[value]);
        }

        foreach (var value in checksum)
        {
            builder.Append(Bech32Alphabet[value]);
        }

        return builder.ToString();
    }

    private static byte[] CreateChecksum(string humanReadablePart, byte[] data)
    {
        var values = HumanReadablePartExpand(humanReadablePart)
            .Concat(data.Select(static x => (int)x))
            .Concat(Bech32ChecksumPadding)
            .ToArray();
        var polymod = Polymod(values) ^ 1;
        var checksum = new byte[6];

        for (var i = 0; i < checksum.Length; i++)
        {
            checksum[i] = (byte)((polymod >> (5 * (5 - i))) & 31);
        }

        return checksum;
    }

    private static int Polymod(IEnumerable<int> values)
    {
        var chk = 1;

        foreach (var value in values)
        {
            var top = chk >> 25;
            chk = (chk & 0x1ffffff) << 5 ^ value;

            for (var i = 0; i < Bech32PolymodGenerator.Length; i++)
            {
                if (((top >> i) & 1) != 0)
                {
                    chk ^= Bech32PolymodGenerator[i];
                }
            }
        }

        return chk;
    }

    private static IEnumerable<int> HumanReadablePartExpand(string humanReadablePart)
    {
        foreach (var c in humanReadablePart)
        {
            yield return c >> 5;
        }

        yield return 0;

        foreach (var c in humanReadablePart)
        {
            yield return c & 31;
        }
    }

    private static byte[] ConvertBits(byte[] data, int fromBits, int toBits, bool pad)
    {
        var accumulator = 0;
        var bits = 0;
        var maxValue = (1 << toBits) - 1;
        var result = new List<byte>();

        foreach (var value in data)
        {
            accumulator = (accumulator << fromBits) | value;
            bits += fromBits;

            while (bits >= toBits)
            {
                bits -= toBits;
                result.Add((byte)((accumulator >> bits) & maxValue));
            }
        }

        if (pad && bits > 0)
        {
            result.Add((byte)((accumulator << (toBits - bits)) & maxValue));
        }

        return result.ToArray();
    }
}
