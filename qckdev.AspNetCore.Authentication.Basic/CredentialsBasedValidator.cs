using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Basic authentication validator that uses static credentials from options.
    /// Credentials are compared using exact string matching (case-sensitive).
    /// </summary>
    public class CredentialsBasedValidator : IBasicAuthenticationValidator
    {
        private readonly IOptionsMonitor<CredentialsBasedOptions> _optionsMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialsBasedValidator"/> class.
        /// </summary>
        /// <param name="optionsMonitor">The options monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown when optionsMonitor is null.</exception>
        public CredentialsBasedValidator(IOptionsMonitor<CredentialsBasedOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        /// <summary>
        /// Validates credentials by comparing against the configured username and password.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        /// <param name="authenticationScheme">The authentication scheme requesting validation.</param>
        /// <param name="cancellationToken">Cancellation token (not used).</param>
        /// <returns>True if both username and password match the configured credentials; otherwise false.</returns>
        public Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default)
        {
            var options = _optionsMonitor.Get(authenticationScheme);

            if (string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password))
            {
                return Task.FromResult(false);
            }

            var isValid = string.Equals(options.Username, username, StringComparison.Ordinal) &&
                          string.Equals(options.Password, password, StringComparison.Ordinal);

            return Task.FromResult(isValid);
        }
    }
}
