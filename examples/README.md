# Examples

This folder contains runnable examples for `qckdev.AspNetCore.Authentication.Basic`.

Each example focuses on a core scenario from the package `README`.

## What You Will Find

- `BasicExample.StaticCredentials`: default `Basic` scheme with static credentials + Swagger integration.
- `BasicExample.CustomValidator`: custom validator with in-memory user service and password hasher.
- `BasicExample.MultipleSchemes`: two schemes (`ApiKey` and `Database`) with different validators.

## Common Endpoints

Most examples expose:

- `GET /auth/public` -> public endpoint.
- `GET /auth/protected` -> protected by default `Basic` scheme.

`BasicExample.MultipleSchemes` exposes dedicated endpoints per scheme:

- `GET /auth/apikey` -> requires `ApiKey` scheme.
- `GET /auth/database` -> requires `Database` scheme.

## 1) BasicExample.StaticCredentials

### What It Demonstrates

- Static credentials via `AddBasicAuthentication(opts => ...)`.
- Swagger UI with Basic security definition and requirement.
- Default `Basic` scheme authorization.

### Credentials

- Username: `admin`
- Password: `secretPassword123`

## 2) BasicExample.CustomValidator

### What It Demonstrates

- `IBasicAuthenticationValidator` implementation with DI dependencies.
- In-memory user service + password hasher composition.
- Validation through `AddBasicAuthentication<DatabaseValidator>(...)`.

### Credentials

- Username: `dbuser`
- Password: `dbpass123`

## 3) BasicExample.MultipleSchemes

### What It Demonstrates

- Multiple Basic schemes registered in the same API.
- `ApiKey` scheme using `CredentialsBasedValidator`.
- `Database` scheme using a custom validator.

### Credentials

- `ApiKey` endpoint:
  - Username: `ApiKey`
  - Password: `your-secret-api-key-token`
- `Database` endpoint:
  - Username: `enterprise.user`
  - Password: `enterprise-pass`

## Run

From each example folder:

```bash
dotnet run -f net8.0
```

Then call protected endpoints with an `Authorization: Basic ...` header or use Swagger in `BasicExample.StaticCredentials`.

## Suggested Learning Order

1. `BasicExample.StaticCredentials`
2. `BasicExample.CustomValidator`
3. `BasicExample.MultipleSchemes`
