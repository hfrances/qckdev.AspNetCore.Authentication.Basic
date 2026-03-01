# Framework Compatibility

This document describes the framework compatibility and security considerations for `qckdev.AspNetCore.Authentication.Basic`.

## Supported Frameworks

This package supports the following target frameworks:

| Framework | Status | Support Level |
|-----------|--------|---------------|
| .NET Standard 2.0 | ✅ Supported | Legacy compatibility |
| .NET Core 3.1 | ✅ Supported | End of Life (EOL) - December 2022 |
| .NET 5.0 | ✅ Supported | End of Life (EOL) - May 2022 |
| .NET 6.0 | ✅ Supported | LTS - Supported until November 2024 |
| .NET 8.0 | ✅ Supported | LTS - Supported until November 2026 |
| .NET 10.0 | ✅ Supported | Current - Supported until November 2027 |

## Package Versions

The library uses ASP.NET Core authentication base classes which are distributed differently depending on the target framework:

| Framework | Strategy | Notes |
|-----------|----------|-------|
| `netstandard2.0` | `Microsoft.AspNetCore.Authentication` 2.3.9 | NuGet package (series 2.3.x) |
| `netcoreapp3.1` | Shared framework reference | Via `FrameworkReference Microsoft.AspNetCore.App` |
| `net5.0` | Shared framework reference | Via `FrameworkReference Microsoft.AspNetCore.App` |
| `net6.0` | Shared framework reference | Via `FrameworkReference Microsoft.AspNetCore.App` |
| `net8.0` | Shared framework reference | Via `FrameworkReference Microsoft.AspNetCore.App` |
| `net10.0` | Shared framework reference | Via `FrameworkReference Microsoft.AspNetCore.App` |

### Why `FrameworkReference` for net3.1+?

From ASP.NET Core 2.1 onwards, Microsoft no longer distributes `Microsoft.AspNetCore.*` packages as standalone NuGet packages for modern frameworks. They are part of the **shared framework** (`Microsoft.AspNetCore.App`), which ships with the .NET SDK. Using `FrameworkReference` instead of `PackageReference` for these targets:

- ✅ Avoids referencing legacy/deprecated NuGet packages
- ✅ Eliminates dependency vulnerability overrides
- ✅ Always resolves to the version installed on the target machine
- ✅ Reduces NuGet package size

### Why series `2.3.x` for `netstandard2.0`?

The `2.3.x` series is the active maintenance branch Microsoft created to keep `Microsoft.AspNetCore.*` packages working in `netstandard2.0` libraries. It supersedes the deprecated `2.1.x` / `2.2.x` series:

| Series | Status | Notes |
|--------|--------|-------|
| `2.1.x` | ⚠️ Deprecated | EOL, known vulnerabilities |
| `2.2.x` | ⚠️ Deprecated | EOL, known vulnerabilities |
| `2.3.x` | ✅ Active | Actively maintained, `netstandard2.0` compatible |

## Security Considerations

### Vulnerability Mitigations

The `netstandard2.0` target uses version `2.3.9` of `Microsoft.AspNetCore.Authentication`, which addresses known vulnerabilities in older versions:

| Package | Version | CVE / Reason |
|---------|---------|--------------|
| `Microsoft.AspNetCore.Authentication` | 2.3.9 | Updated dependency chains resolve transitive vulnerabilities |
| `System.Text.Encodings.Web` | Latest for 2.3.x | Addressed via transitive dependencies |

### Version Selection Strategy

The package versions were selected using the **Minimum Viable Product (MVP)** approach:
- ✅ Uses the **minimum version** required to address known vulnerabilities
- ✅ Avoids unnecessary updates that might introduce breaking changes
- ✅ Maintains compatibility with older frameworks for legacy support
- ✅ Regular security audits using `dotnet list package --vulnerable`

## Package Version Analysis

### .NET Standard 2.0

| Package | Current | Latest for Framework | What's Missing |
|---------|---------|---------------------|----------------|
| `Microsoft.AspNetCore.Authentication` | **2.3.9** | 2.3.9 | ✅ Using latest for `netstandard2.0` |

**Notes**:
- `2.3.9` is the latest version of the `2.x` series supporting `netstandard2.0`
- Versions `3.0+` of `Microsoft.AspNetCore.*` target .NET Core 3.0+ exclusively
- No known security vulnerabilities in `2.3.9`

### .NET Core 3.1+ (via FrameworkReference)

| Component | Strategy | Notes |
|-----------|----------|-------|
| ASP.NET Core | `FrameworkReference` (shared framework) | Resolves to the shared framework version installed on the machine |

**Recommendation**: 
- ⚠️ .NET Core 3.1 is EOL since December 2022. Plan migration to .NET 6.0 LTS or later.
- ✅ .NET 6.0, 8.0, and 10.0 are actively supported.

## Encoding

All schemes in this library use UTF-8 encoding by default as specified in [RFC 7617](https://tools.ietf.org/html/rfc7617):

> The user's name, and the password, MAY contain any character that is allowed in a UTF-8 encoded string.

The default encoding can be customized via the `BasicAuthenticationOptions.Encoding` property if needed.

---

For more information about supported frameworks and long-term support (LTS) status, visit the [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
