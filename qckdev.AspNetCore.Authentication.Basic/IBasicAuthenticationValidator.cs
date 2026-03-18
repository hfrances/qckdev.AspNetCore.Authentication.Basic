using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Provides the contract for validating basic authentication credentials.
    /// </summary>
    public interface IBasicAuthenticationValidator
    {
        /// <summary>
        /// Validates the provided credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="authenticationScheme">The authentication scheme requesting validation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// True if credentials are valid; otherwise false.
        /// </returns>
        Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default);
    }
}
