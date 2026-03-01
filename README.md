<a href="https://www.nuget.org/packages/qckdev.AspNetCore.Authentication.Basic"><img src="https://img.shields.io/nuget/v/qckdev.AspNetCore.Authentication.Basic.svg" alt="NuGet Version"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore.Authentication.Basic"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore.Authentication.Basic&metric=alert_status" alt="Quality Gate"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore.Authentication.Basic"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore.Authentication.Basic&metric=coverage" alt="Code Coverage"/></a>
<a><img src="https://hfrances.visualstudio.com/qckdev/_apis/build/status/qckdev.AspNetCore.Authentication.Basic?branchName=master" alt="Azure Pipelines Status"/></a>

# qckdev.AspNetCore.Authentication.Basic

Provides tools to configure Basic authentication for ASP.NET Core with extensible validation strategies.

## Overview

This library simplifies the setup of HTTP Basic Authentication in ASP.NET Core applications with a flexible, extensible architecture. It supports:

- **Simple scenarios**: Static credentials embedded in options
- **Configuration-based**: Credentials from `appsettings.json`
- **Advanced scenarios**: Custom validators with database, LDAP, or external service integration
- **Generic options**: Extend `BasicAuthenticationOptions` for custom properties
- **Multiple schemes**: Support for multiple authentication schemes

## Installation

```bash
dotnet add package qckdev.AspNetCore.Authentication.Basic
```

## Quick Start

### Simple Usage with Static Credentials

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Authentication.Basic;

services
    .AddAuthentication("Basic")
    .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(opts =>
    {
        opts.Realm = "My API";
        opts.Username = "admin";
        opts.Password = "secretPassword123";
    });
```

Then use the `[Authorize(AuthenticationSchemes = "Basic")]` attribute on your controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Basic")]
public class ProtectedController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var username = User.Identity?.Name;
        return Ok(new { message = $"Hello, {username}!" });
    }
}
```

### Custom Validator with Database

Implement a custom validator to check credentials against a database:

```csharp
using qckdev.AspNetCore.Authentication.Basic;
using System.Threading;
using System.Threading.Tasks;

public class DatabaseValidator : IBasicAuthenticationValidator
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseValidator(IUserService userService, IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> ValidateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.GetUserAsync(username, cancellationToken);
            if (user == null) return false;

            return _passwordHasher.Verify(user.PasswordHash, password);
        }
        catch
        {
            return false;
        }
    }
}
```

Register it in your `Startup.cs` or `Program.cs`:

```csharp
services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IPasswordHasher, PasswordHasher>()
    .AddAuthentication("Basic")
    .AddBasicAuthentication<DatabaseValidator>(opts =>
    {
        opts.Realm = "Enterprise API";
    });
```

### Multiple Schemes

You can register multiple Basic authentication schemes with different validators:

```csharp
services
    .AddAuthentication("Basic")
    .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
        "ApiKey",
        opts =>
        {
            opts.Realm = "Legacy API";
            opts.Username = "api-key";
            opts.Password = "legacy-shared-secret";
        })
    .AddBasicAuthentication<DatabaseValidator>(
        "Database",
        opts =>
        {
            opts.Realm = "Enterprise API";
        });
```

Then use specific schemes on your controllers:

```csharp
[Authorize(AuthenticationSchemes = "ApiKey")]
public class LegacyController : ControllerBase { }

[Authorize(AuthenticationSchemes = "Database")]
public class EnterpriseController : ControllerBase { }
```

### Custom Options Class

Extend `BasicAuthenticationOptions` to add custom properties:

```csharp
public class CustomBasicOptions : BasicAuthenticationOptions
{
    public string? ApiVersion { get; set; }
    public int MaxLoginAttempts { get; set; } = 5;
}
```

Use it with your validator:

```csharp
services
    .AddAuthentication()
    .AddBasicAuthentication<DatabaseValidator, CustomBasicOptions>(opts =>
    {
        opts.Realm = "My API";
        opts.ApiVersion = "v1";
        opts.MaxLoginAttempts = 3;
    });
```

## Configuration Options

### BasicAuthenticationOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Realm` | `string?` | `"Application"` | The realm sent in the WWW-Authenticate header (RFC 7617) |
| `AllowEmptyCredentials` | `bool` | `false` | Whether to allow empty username or password |
| `Encoding` | `Encoding?` | `UTF-8` | Character encoding for decoding credentials |

### CredentialsBasedOptions (extends BasicAuthenticationOptions)

| Property | Type | Description |
|----------|------|-------------|
| `Username` | `string?` | The username for static authentication |
| `Password` | `string?` | The password for static authentication |

## Built-in Validators

### CredentialsBasedValidator

Uses static credentials from `CredentialsBasedOptions`. Performs case-sensitive string comparison.

```csharp
.AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(opts =>
{
    opts.Username = "admin";
    opts.Password = "password123";
});
```

## Custom Validator Implementation

Create a custom validator by implementing `IBasicAuthenticationValidator`:

```csharp
public interface IBasicAuthenticationValidator
{
    Task<bool> ValidateAsync(string username, string password, CancellationToken cancellationToken = default);
}
```

Example with external service:

```csharp
public class ExternalServiceValidator : IBasicAuthenticationValidator
{
    private readonly HttpClient _httpClient;

    public ExternalServiceValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var request = new { username, password };
        var response = await _httpClient.PostAsJsonAsync(
            "https://auth-service.example.com/validate",
            request,
            cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
```

## API Key Pattern

Use Basic Authentication as a simple API key mechanism:

```csharp
// Startup
services
    .AddAuthentication()
    .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
        "ApiKey",
        opts =>
        {
            opts.Realm = "API Key Authentication";
            opts.Username = "ApiKey"; // Fixed username
            opts.Password = "your-secret-api-key-token";
        });

// Usage: Pass as Authorization header
// Authorization: Basic QXBpS2V5OnlvdXItc2VjcmV0LWFwaS1rZXktdG9rZW4=
```

## Features

✅ Extensible validation strategy pattern  
✅ Multiple authentication schemes  
✅ Generic options class inheritance  
✅ Thread-safe credential validation  
✅ XML documentation  
✅ No external dependencies (uses only ASP.NET Core built-ins)  

## Best Practices

- **Production**: Always use HTTPS when transmitting Basic Authentication credentials
- **Password storage**: Hash and salt passwords, never store plain text
- **Validation**: Implement timeout and rate-limiting in custom validators
- **Realm**: Use descriptive realm names to help clients understand the protected resource
- **Claims**: Consider adding claims mapping in custom validators for authorization

## Testing

This library includes comprehensive integration tests covering credential validation, header parsing, and edge cases.

**7 integration tests** validate the complete authentication pipeline:
- Public endpoint access
- Challenge response (WWW-Authenticate)
- Valid and invalid credentials
- Malformed headers
- Edge cases (empty password, etc.)

For detailed testing documentation, see [Integration Testing Guide](docs/TESTING.md).

## Supported Frameworks

- .NET Core 3.1
- .NET 5.0
- .NET 6.0
- .NET 8.0
- .NET 10.0

## License

MIT License - See LICENSE file for details

## Contributing

Contributions are welcome! Please submit issues and pull requests on GitHub.
