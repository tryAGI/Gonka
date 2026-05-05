# Microsoft.Extensions.AI Integration

!!! tip "Cross-SDK comparison"
    See the [centralized MEAI documentation](https://tryagi.github.io/docs/meai/) for feature matrices and comparisons across all tryAGI SDKs.

The Gonka SDK currently focuses on the generated direct REST client and the Gonka-specific request signing layer. A Microsoft.Extensions.AI adapter is not implemented yet.

## Installation

```bash
dotnet add package Gonka
```

## Usage

```csharp
using Gonka;

using var client = await GonkaClient.CreateFromEnvironmentAsync();
```

## Next Steps

- Check the [Examples](../index.md) for complete direct SDK usage
- See the [centralized MEAI docs](https://tryagi.github.io/docs/meai/) for cross-SDK comparisons
- Add an `IChatClient` adapter after the direct SDK surface stabilizes
