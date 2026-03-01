using Microsoft.AspNetCore.Authentication;
using System.Text;

namespace qckdev.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Options for configuring Basic Authentication.
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the realm for basic authentication.
        /// This value is sent in the WWW-Authenticate response header.
        /// </summary>
        /// <remarks>
        /// RFC 7617: The realm parameter is optional but recommended to indicate
        /// the protection space to the user agent.
        /// </remarks>
        public string? Realm { get; set; } = "Application";

        /// <summary>
        /// Gets or sets whether to allow empty credentials (empty username or password).
        /// Defaults to false.
        /// </summary>
        public bool AllowEmptyCredentials { get; set; }

        /// <summary>
        /// Gets or sets the character encoding used for decoding base64 credentials.
        /// Defaults to UTF-8.
        /// </summary>
        public Encoding? Encoding { get; set; } = Encoding.UTF8;
    }
}
