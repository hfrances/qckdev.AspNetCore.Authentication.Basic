namespace qckdev.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Authentication options with embedded static credentials.
    /// Useful for simple scenarios where credentials are configured directly in options.
    /// </summary>
    public class CredentialsBasedOptions : BasicAuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the username for authentication.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        public string? Password { get; set; }
    }
}
