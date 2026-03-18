using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Authentication handler for basic authentication.
    /// </summary>
    /// <typeparam name="TOptions">The type of options.</typeparam>
    public class BasicAuthenticationHandler<TOptions> : AuthenticationHandler<TOptions>
        where TOptions : BasicAuthenticationOptions, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler{TOptions}"/> class.
        /// </summary>
        /// <param name="options">The options monitor.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="encoder">The URL encoder.</param>
        /// <param name="clock">The system clock.</param>
        /// <param name="validator">The basic authentication validator.</param>
        /// <exception cref="ArgumentNullException">Thrown when validator is null.</exception>
        public BasicAuthenticationHandler(
            IOptionsMonitor<TOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// Handles the authentication by extracting and validating basic credentials.
        /// </summary>
        /// <returns>An <see cref="AuthenticateResult"/> indicating success or failure.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                {
                    return AuthenticateResult.NoResult();
                }

                var authHeader = authHeaderValues.ToString();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return AuthenticateResult.NoResult();
                }

                if (!AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
                {
                    return AuthenticateResult.NoResult();
                }

                if (!BasicAuthenticationDefaults.AuthenticationScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.NoResult();
                }

                if (string.IsNullOrEmpty(headerValue.Parameter))
                {
                    return AuthenticateResult.Fail("Missing credentials");
                }

                var credentialBytes = Convert.FromBase64String(headerValue.Parameter!);
                var credentials = Options.Encoding!.GetString(credentialBytes).Split(new[] { ':' }, 2);

                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid credentials format");
                }

                var username = credentials[0];
                var password = credentials[1];

                if (!Options.AllowEmptyCredentials && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
                {
                    return AuthenticateResult.Fail("Empty credentials not allowed");
                }

                var validator = ResolveValidator();
                if (validator is null)
                {
                    return AuthenticateResult.Fail("Validator is not configured for this scheme");
                }

                var isValid = await validator.ValidateAsync(username, password, Scheme.Name, Context.RequestAborted);
                if (!isValid)
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Authentication failed");
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Handles the challenge response by adding the WWW-Authenticate header.
        /// </summary>
        /// <param name="properties">The authentication properties.</param>
        /// <returns>A completed task.</returns>
        protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{Options.Realm}\"");
            return Task.CompletedTask;
        }

        private IBasicAuthenticationValidator? ResolveValidator()
        {
            var validatorType = Options.ValidatorType;
            if (validatorType is null)
            {
                return null;
            }

            if (!typeof(IBasicAuthenticationValidator).IsAssignableFrom(validatorType))
            {
                return null;
            }

            return Context.RequestServices.GetService(validatorType) as IBasicAuthenticationValidator;
        }
    }
}
