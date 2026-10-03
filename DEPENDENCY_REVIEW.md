# Gonka cryptography dependency review

Reviewed 2026-10-03 against the SDK runtime dependency policy. This is a
compatibility assessment, not a cryptographic security audit.

## Why BouncyCastle is present

`GonkaCryptography.cs` uses BouncyCastle.Cryptography 2.7.0 for:

- secp256k1 point multiplication and compressed public-key encoding;
- ECDSA signing with HMAC-SHA256 deterministic nonce generation (RFC 6979);
- curve-order arithmetic and low-S normalization;
- RIPEMD-160 of the SHA-256 public-key hash for requester addresses.

SHA-256, payload encoding, Bech32 encoding, and request handling already use
platform or first-party code. There is no other BouncyCastle runtime usage in
the repository. The signature is Base64-encoded, fixed-width 64-byte `r || s`.
Existing no-cost tests assert an exact address and deterministic signature.

## Platform replacement assessment

Microsoft documents that .NET elliptic-curve support depends on OS libraries;
Apple platforms do not support the additional named/explicit curves used here.
See [cross-platform cryptography](https://learn.microsoft.com/en-us/dotnet/standard/security/cross-platform-cryptography#ecdsa).
`ECDsa.SignHash` exposes no caller-selected deterministic nonce, so using the
same algorithm name does not establish RFC 6979 or byte-for-byte compatibility.
See [ECDsa API](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.ecdsa?view=net-10.0)
and [RFC 6979](https://www.rfc-editor.org/rfc/rfc6979).

Reproducible platform probe, using .NET 10 or later:

```sh
dotnet run --file scripts/probe-platform-cryptography.cs
```

Observed on macOS 27.0.0 / .NET 10.0.9:

- secp256k1: `PlatformNotSupportedException` for OID `1.3.132.0.10`;
- RIPEMD160: `CryptographicException`, algorithm not known.

The probe reports capability failures; its exit status is not a compatibility
gate. Linux, Windows, browser, mobile, and NativeAOT capability probes were not
executed in this assessment. Linux support for a curve alone would not prove
portable deterministic-signature compatibility.

## Alternatives and decision

| Option | Assessment |
|---|---|
| Replace directly with .NET `ECDsa` | Not a portable replacement: Apple curve support fails; deterministic signing and RIPEMD-160 remain unresolved. |
| Another third-party crypto package/native library | Changes the supplier; does not satisfy the no-third-party goal. |
| Copy/fork a subset of BouncyCastle | Preserves upstream code and obligations; ownership of the repository does not establish review or trust. |
| Write new secp256k1/RIPEMD-160 code | Requires specialist review of nonce generation, side channels, scalar validation, serialization, known vectors, and negative cases. Do not treat a short implementation as safer by default. |
| Caller-provided signer plus optional BouncyCastle adapter | Optional future design, not an active migration: a dependency-free HTTP SDK can accept a requester address and signing callback, with private-key convenience APIs isolated in an independently installed adapter. Requires an API migration plan and compatibility tests. |

The maintainer approved retaining BouncyCastle for the current private-key API
on 2026-10-03 after this review. No replacement or signer-adapter migration is
planned by this decision. This approval is scoped to Gonka's existing use and
does not authorize adding BouncyCastle to unrelated SDKs or certify a security
audit. Continue routine version and advisory checks.

If a migration is requested later, acceptance criteria are: exact existing vectors for both signature
input modes, low-S and 64-byte output, address derivation, invalid scalars and
keys, cross-platform/AOT coverage, and a captured-request test proving an
externally signed request has the same headers and payload bytes. Preserve the
current public API through an explicit migration/versioning decision.

## Validation recorded during review

The two existing `Cryptography_` tests passed with no skips.
`dotnet list src/libs/Gonka/Gonka.csproj package --vulnerable --include-transitive`
reported no known vulnerable packages from the configured feeds on 2026-10-03.
This result is advisory-database evidence, not proof that the implementation is
free of vulnerabilities. No credentialed/provider calls were run for this review.
