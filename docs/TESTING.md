# Integration Testing

## Overview

This project includes comprehensive integration tests that validate the complete HTTP Basic authentication pipeline in a realistic ASP.NET Core context.

## Test Architecture

The tests spin up a minimal ASP.NET Core host for isolation and validation:

1. **LocalTestServiceManager** - Hosts ASP.NET Core application on dynamic port (26000 + PID)
2. **TestAssemblySetup** - Manages host lifecycle (startup/shutdown)
3. **BasicAuthenticationTestController** - Provides test endpoints
4. **BasicAuthenticationIntegrationTests** - Validates HTTP behavior

### Why Integration Tests?

Integration tests validate the **complete authentication pipeline** including:
- ✅ Header parsing and validation
- ✅ Base64 encoding/decoding
- ✅ Credentials extraction and comparison
- ✅ WWW-Authenticate challenge generation
- ✅ HTTP status codes and response headers

This catches issues that unit tests cannot: middleware ordering, header case sensitivity, realm encoding, etc.

## Test Coverage

### Integration Tests (7 tests)

#### Public Endpoint (Unauthenticated Access)

**ProtectedEndpoint_WithoutCredentials_ReturnsOk**
```csharp
GET /basic/public
// No Authorization header

Expected: HTTP 200 OK
Response: "public-ok"
```
Validates unrestricted endpoint access without authentication.

#### Challenge Response (401 Unauthorized)

**ProtectedEndpoint_WithoutCredentials_ReturnsUnauthorized**
```csharp
GET /basic/protected
// No Authorization header

Expected: HTTP 401 Unauthorized
Headers: WWW-Authenticate: Basic realm="Test Realm"
```
Validates proper challenge response when credentials are missing.

#### Valid Authentication

**ProtectedEndpoint_WithValidCredentials_ReturnsOk**
```csharp
GET /basic/protected
Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M=
// base64("testuser:testpass")

Expected: HTTP 200 OK
Response: "protected-ok-testuser"
```
Validates successful authentication with correct credentials.

#### Invalid Username

**ProtectedEndpoint_WithInvalidUsername_ReturnsUnauthorized**
```csharp
GET /basic/protected
Authorization: Basic d3Jvbmd1c2VyOnRlc3RwYXNz
// base64("wronguser:testpass")

Expected: HTTP 401 Unauthorized
```
Validates rejection of incorrect username.

#### Invalid Password

**ProtectedEndpoint_WithInvalidPassword_ReturnsUnauthorized**
```csharp
GET /basic/protected
Authorization: Basic dGVzdHVzZXI6d3JvbmdwYXNz
// base64("testuser:wrongpass")

Expected: HTTP 401 Unauthorized
```
Validates rejection of incorrect password.

#### Malformed Authorization Header

**ProtectedEndpoint_WithMalformedAuthorizationHeader_ReturnsUnauthorized**
```csharp
GET /basic/protected
Authorization: Basic invalid-base64-data!!!

Expected: HTTP 401 Unauthorized
```
Validates error handling for non-Base64 data in Authorization header.

#### Empty Password Edge Case

**ProtectedEndpoint_WithEmptyPassword_ReturnsUnauthorized**
```csharp
GET /basic/protected
Authorization: Basic dGVzdHVzZXI6
// base64("testuser:")

Expected: HTTP 401 Unauthorized
```
Validates rejection of empty passwords.

## Test Configuration

### Test Credentials

| Username | Password |
|----------|----------|
| `testuser` | `testpass` |

### Test Environment

| Setting | Value |
|---------|-------|
| **Service Port** | Dynamic (26000 + Process ID) |
| **Realm** | `"Test Realm"` |
| **Encoding** | UTF-8 (RFC 7617) |
| **Validator** | `TestBasicAuthenticationValidator` (hardcoded) |

### Validator Implementation

The test uses a simple inline validator:

```csharp
class TestBasicAuthenticationValidator : IBasicAuthenticationValidator
{
    public Task<bool> ValidateAsync(string username, string password, 
        CancellationToken cancellationToken = default)
    {
        var isValid = username == "testuser" && password == "testpass";
        return Task.FromResult(isValid);
    }
}
```

## Running Tests

### Run All Tests

```bash
cd qckdev.AspNetCore.Authentication.Basic
dotnet test qckdev.AspNetCore.Authentication.Basic.sln -v minimal
```

### Run Specific Test

```bash
dotnet test --filter "FullyQualifiedName~BasicAuthenticationIntegrationTests.ProtectedEndpoint_WithValidCredentials"
```

### Run with Detailed Logging

```bash
dotnet test --logger "console;verbosity=detailed" --no-build
```

## Test Execution Flow

```
[AssemblyInitialize]
  ↓
  LocalTestServiceManager.StartIfNeeded()
    ├─ Create IHostBuilder
    ├─ Configure services (AddControllers, AddAuthentication)
    ├─ Configure middleware (UseRouting, UseAuthentication, UseEndpoints)
    ├─ Start host on dynamic port
    └─ Wait for service ready (polling, 10s timeout)
  ↓
[Test Methods Execute]
  ├─ Create HttpClient with base address from service
  ├─ Build request with headers (Authorization, etc.)
  ├─ Send request to live endpoint
  └─ Assert response status, headers, content
  ↓
[AssemblyCleanup]
  ↓
  LocalTestServiceManager.Stop()
    └─ Dispose host
```

## Multi-Framework Coverage

Tests execute against all supported frameworks:

- .NET Core 3.1 (EOL December 2022)
- .NET 5.0 (EOL May 2022)
- .NET 6.0 (LTS until November 2024)
- .NET 8.0 (LTS until November 2026)
- .NET 10.0 (Current, supported until November 2027)

Each framework runs **7 integration tests** independently.

## Key Validations

✅ **Header parsing** - Case-insensitive scheme matching  
✅ **Encoding** - Base64 decoding with UTF-8 charset  
✅ **Credentials extraction** - Splits on first `:` only  
✅ **Case sensitivity** - Username/password comparison is ordinal (case-sensitive)  
✅ **Error handling** - Base64 decode failures trigger 401  
✅ **Challenge response** - WWW-Authenticate header present and correctly formatted  
✅ **Identity mapping** - Authenticated username available via `User.Identity.Name`  

## Troubleshooting

### Test Hangs on Service Ready

**Symptom**: Test hangs for 10 seconds then fails  
**Cause**: Service not responding on expected port  
**Solution**: 
- Verify port is not in use: `netstat -ano | findstr :26xxx`
- Check firewall settings
- Ensure ASP.NET Core runtime installed

### Port Conflict

**Symptom**: "Address already in use" error  
**Cause**: Another process using the dynamic port  
**Solution**:
- Tests use different ports based on Process ID
- If running multiple test suites simultaneously, ensure different processes
- Manually free port: `netstat -ano | findstr :26xxx` → `taskkill /PID xxxx`

### Credential Not Recognized

**Symptom**: Valid credentials rejected with "Invalid username or password"  
**Cause**: Credentials don't match test validator exactly  
**Solution**:
- Verify credentials in test match `TestBasicAuthenticationValidator` logic
- Check Base64 encoding is correct
- Ensure no trailing whitespace in username/password

## Best Practices

1. **Always use Base64 encoding** - Never send credentials in plain text
2. **Use HTTPS in production** - Basic auth transmits credentials in header only
3. **Test custom validators** - Create integration tests for your own validator implementations
4. **Parallel execution** - Tests are thread-safe and work with parallel execution
5. **Check for timeouts** - Custom validators should implement timeouts to prevent hanging

## Related Documentation

- [Test Coverage Guide](../../TEST_COVERAGE.md) - Coverage across all projects
- [Framework Compatibility](COMPATIBILITY.md) - Supported frameworks and versions
- [Main README](../README.md) - Usage examples and quickstart

---

Last updated: March 1, 2026
